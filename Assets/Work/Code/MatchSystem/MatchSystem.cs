using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Lib.Dependencies;
using Lib.ObjectPool.RunTime;
using Lib.Utiles;
using UnityEngine;
using Work.Code.Events;
using Work.Code.Items;
using Work.Code.Manager;
using Work.Code.SoundSystem;
using Work.Code.Supply;
using Random = UnityEngine.Random;

namespace Work.Code.MatchSystem
{
    public struct GetIngredientData
    {
        public SupplyType SupplyType;
        public int count;
    }

    [Provide]
    public class MatchSystem : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private EventChannelSO supplyEventChannel;
        [SerializeField] private EventChannelSO gameEventChannel;
        [SerializeField] private EventChannelSO particleEventChannel;
        [SerializeField] private PoolItemSO soundPlayer;
        [SerializeField] private SoundSO swapSound;
        [SerializeField] private SoundSO removeSound;
        [SerializeField] private SoundSO iceBreakSound;
        [SerializeField] private SoundSO unlockSound;
        [Inject] private PoolManagerMono poolManager;
        [SerializeField] private PoolItemSO particleItem;
        [SerializeField] private PoolItemSO icedEffectItem;
        [SerializeField] private PoolItemSO lockedEffectItem;
        [SerializeField] private PoolItemSO lineEffectItem;
        [SerializeField] private PoolItemSO threeXthreeEffectItem;
        [SerializeField] private PoolItemSO nyanCatEffectItem;
        [SerializeField] private Node[] nodePrefabs;
        [SerializeField] private Node lockedNodePrefab;
        [SerializeField] private RectTransform nodeBoard;
        [SerializeField] private List<Vector2Int> lockedNode;
        [Range(0, 1f), SerializeField] private float icedNodeRate;
        [SerializeField] private int turnAddAmount = 2;

        private Vector2Int[] _eightDirection =
            { new(0, 1), new(1, 1), new(1, 0), new(1, -1), new(0, -1), new(-1, -1), new(-1, 0), new(-1, 1) };

        #region Board Region

        [field: SerializeField] public int MapWidth { get; private set; } = 8;
        [field: SerializeField] public int MapHeight { get; private set; } = 8;

        public Node[,] NodeMap { get; private set; }
        public NodeData[,] DataMap { get; private set; }

        private float _nodeWidth, _nodeHeight, _widthTerm, _heightTerm;
        private bool _isSwapping;
        private bool _isGetDouble;
        private int _getDoubleCnt;
        private ItemTreeSO _pendingItem;

        private readonly Dictionary<NodeType, HashSet<NodeData>> _removeNodesDict = new();


        private void Awake()
        {
            DataMap = new NodeData[MapHeight, MapWidth];
            NodeMap = new Node[MapHeight, MapWidth];

            for (int i = 0; i < (int)NodeType.Ingredient; i++)
                _removeNodesDict[(NodeType)i] = new HashSet<NodeData>();
            _removeNodesDict.Add(NodeType.Locked, new HashSet<NodeData>());

            CalcBoardSize();
            SetNodes();
        }

        private void CalcBoardSize()
        {
            RectTransform rt = nodePrefabs[0].transform as RectTransform;
            _nodeWidth = rt.rect.width;
            _nodeHeight = rt.rect.height;

            _widthTerm = (nodeBoard.rect.width - _nodeWidth * MapWidth) / MapWidth;
            _heightTerm = (nodeBoard.rect.height - _nodeHeight * MapHeight) / MapHeight;
        }

        private float CalcNodePosX(int x) => x * _nodeWidth + (x + 0.5f) * _widthTerm;
        private float CalcNodePosY(int y) => y * -_nodeHeight - (y + 0.5f) * _heightTerm;

        private float CalcSpawnPosY(int x)
        {
            int spawnY = -1;

            for (int y = 0; y < MapHeight; y++)
            {
                if (NodeMap[y, x] != null)
                {
                    spawnY = y - 1;
                    break;
                }
            }

            return CalcNodePosY(spawnY);
        }

        private Node CreateNode()
        {
            return Instantiate(nodePrefabs[Random.Range(0, nodePrefabs.Length)], nodeBoard);
        }

       private async void SetNodes()
       {
           for (int y = 0; y < MapHeight; y++)
           {
               for (int x = 0; x < MapWidth; x++)
               {
                   Vector2Int pos = new Vector2Int(x, y);
                   Node node;
       
                   if (lockedNode.Contains(pos))
                   {
                       node = Instantiate(lockedNodePrefab, nodeBoard);
                       NodeMap[y, x] = node;
                       node.Init(x, y, this, false);
                   }
                   else
                   {
                       int validIndex = GetValidNodeIndex(x, y);
                       node = Instantiate(nodePrefabs[validIndex], nodeBoard);
       
                       bool isIced = Random.value <= icedNodeRate;
                       NodeMap[y, x] = node;
                       node.Init(x, y, this, isIced);
                   }
       
                   node.SetPos(CalcNodePosX(x), CalcSpawnPosY(x), false);
       
                   DataMap[y, x] = new NodeData(node.NodeType);
                   DataMap[y, x].SetPos(new Vector2Int(x, y));
               }
           }
       
           for (int y = MapHeight - 1; y >= 0; y--)
           {
               List<UniTask> rowTasks = new List<UniTask>();
               for (int x = 0; x < MapWidth; x++)
               {
                   if (NodeMap[y, x] != null)
                   {
                       rowTasks.Add(NodeMap[y, x].SetPos(CalcNodePosX(x), CalcNodePosY(y)));
                   }
               }
               
               await UniTask.Delay(100); 
           }
       }

        private int GetValidNodeIndex(int x, int y)
        {
            List<int> validIndices = new List<int>();

            for (int i = 0; i < nodePrefabs.Length; i++)
            {
                NodeType typeToCheck = nodePrefabs[i].NodeType;
                bool isMatch = false;

                if (x >= 2)
                {
                    if (DataMap[y, x - 1].NodeType == typeToCheck &&
                        DataMap[y, x - 2].NodeType == typeToCheck)
                    {
                        isMatch = true;
                    }
                }

                if (y >= 2)
                {
                    if (DataMap[y - 1, x].NodeType == typeToCheck &&
                        DataMap[y - 2, x].NodeType == typeToCheck)
                    {
                        isMatch = true;
                    }
                }

                if (!isMatch)
                {
                    validIndices.Add(i);
                }
            }

            if (validIndices.Count == 0) return Random.Range(0, nodePrefabs.Length);

            return validIndices[Random.Range(0, validIndices.Count)];
        }

        #endregion

        #region SwapLogic

        private void SwapNodeType(NodeData a, NodeData b)
        {
            NodeType t = a.NodeType;
            a.SetNodeType(b.NodeType);
            b.SetNodeType(t);
        }

        public async void TrySwapByDir(Node node, Vector2Int dir)
        {
            if (_isSwapping) return;

            int nx = node.X + dir.x;
            int ny = node.Y + dir.y;
            if (IsOutBound(nx, ny)) return;

            Node target = NodeMap[ny, nx];
            if (target == null || target.TryGetComponent<LockedNode>(out LockedNode lockedNode)) return;

            _isSwapping = true;

            bool success = await TrySwap(node, target);
            if (success)
            {
                DiscountTurn();
                await ResolveBoard();
            }

            _isSwapping = false;
        }

        private async UniTask<bool> TrySwap(Node a, Node b)
        {
            await SwapInternal(a, b);

            if (HasMatchAt(a.X, a.Y) || HasMatchAt(b.X, b.Y))
                return true;

            await SwapInternal(a, b);
            return false;
        }

        private async UniTask SwapInternal(Node a, Node b)
        {
            poolManager.Pop<SoundPlayer>(soundPlayer).PlaySound(swapSound);

            NodeMap[a.Y, a.X] = b;
            NodeMap[b.Y, b.X] = a;

            SwapNodeType(DataMap[a.Y, a.X], DataMap[b.Y, b.X]);

            int ax = a.X, ay = a.Y;
            a.SetXY(b.X, b.Y);
            b.SetXY(ax, ay);

            Vector2 apos = a.Rect.anchoredPosition;

            await UniTask.WhenAll(
                a.SetPos(b.Rect.anchoredPosition.x, b.Rect.anchoredPosition.y),
                b.SetPos(apos.x, apos.y)
            );
        }

        #endregion

        #region MainLogic

        private async UniTask ResolveBoard()
        {
            while (true)
            {
                bool hasForcedRemoval = false;
                foreach (var set in _removeNodesDict.Values)
                {
                    if (set.Count > 0)
                    {
                        hasForcedRemoval = true;
                        break;
                    }
                }

                bool isMatch = CheckMatch();

                if (!isMatch && !hasForcedRemoval)
                    break;


                BreakAdjacentIce();
                GetIngredient();
                RemoveNode();

                foreach (var set in _removeNodesDict.Values)
                    set.Clear();

                await SortingNodeMap();
            }

            GameManager.Instance.CheckGameOver();
            await FillEmptyMap();


            if (CheckMatch())
            {
                await ResolveBoard();
            }
        }

        private bool CheckMatch()
        {
            bool hasMatch = false;

            for (int y = 0; y < MapHeight; y++)
            for (int x = 0; x < MapWidth; x++)
            {
                if (HasMatchAt(x, y))
                {
                    AddRemoveNode(x, y);
                    hasMatch = true;
                }
            }

            return hasMatch;
        }

        private void AddRemoveNode(int x, int y)
        {
            NodeType type = DataMap[y, x].NodeType;
            if (type == NodeType.Empty)
                return;
            _removeNodesDict[type].Add(DataMap[y, x]);
        }

        private void GetIngredient()
        {
            foreach (var kv in _removeNodesDict)
            {
                if (kv.Value.Count == 0) continue;

                int count = kv.Value.Count;

                if (count > 3)
                {
                    count += count - 3;
                }

                if (_isGetDouble)
                {
                    count *= 2;
                }

                if ((int)kv.Key <= 5)
                {
                    supplyEventChannel.InvokeEvent(
                        SupplyEvents.SupplyEvent.Initializer
                            ((SupplyType)kv.Key, count));
                    supplyEventChannel.InvokeEvent(
                        SupplyEvents.MatchSupplyEvent.Initializer(kv.Value));
                }

                foreach (var data in kv.Value)
                    data.SetNodeType(NodeType.Empty);
            }
        }

        private void RemoveNode()
        {

            for (int y = 0; y < MapHeight; y++)
            for (int x = 0; x < MapWidth; x++)
            {
                if (DataMap[y, x].NodeType == NodeType.Empty && NodeMap[y, x] != null)
                {
                    poolManager.Pop<SoundPlayer>(soundPlayer).PlaySound(removeSound);
                    PoolItemSO particlePoolItem = particleItem;

                    if (NodeMap[y, x].TryGetComponent<LockedNode>(out _))
                    {
                        particlePoolItem = lockedEffectItem;
                    }

                    particleEventChannel.InvokeEvent(
                        ParticleEvents.PlayUIParticleEvent.Initializer(particlePoolItem, NodeMap[y, x].CenterPos,
                            Quaternion.identity));
                    Destroy(NodeMap[y, x].gameObject);
                    NodeMap[y, x] = null;
                }
            }
        }

        private async UniTask SortingNodeMap()
        {
            bool changed = true;
            List<UniTask> moves = new List<UniTask>();

            while (changed)
            {
                changed = false;
                for (int y = MapHeight - 1; y >= 1; y--)
                {
                    for (int x = 0; x < MapWidth; x++)
                    {
                        if (NodeMap[y, x] == null && !IsLockedAt(x, y))
                        {
                            if (NodeMap[y - 1, x] != null && !IsLockedAt(x, y - 1))
                            {
                                MoveNodeInternal(x, y - 1, x, y, moves);
                                changed = true;
                            }
                            else
                            {
                                if (IsPathBlockedByLock(x, y))
                                {
                                    int[] sideOffsets = { -1, 1 };
                                    foreach (int dx in sideOffsets)
                                    {
                                        int nx = x + dx;
                                        int ny = y - 1;

                                        if (!IsOutBound(nx, ny) && NodeMap[ny, nx] != null && !IsLockedAt(nx, ny))
                                        {
                                            MoveNodeInternal(nx, ny, x, y, moves);
                                            changed = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (moves.Count > 0)
                await UniTask.WhenAll(moves);
        }

        private bool IsPathBlockedByLock(int x, int targetY)
        {
            for (int y = 0; y < targetY; y++)
            {
                if (IsLockedAt(x, y)) return true;
            }

            return false;
        }

        private void MoveNodeInternal(int fromX, int fromY, int toX, int toY, List<UniTask> moves)
        {
            Node node = NodeMap[fromY, fromX];
            NodeMap[toY, toX] = node;
            NodeMap[fromY, fromX] = null;

            DataMap[toY, toX].SetNodeType(DataMap[fromY, fromX].NodeType);
            DataMap[fromY, fromX].SetNodeType(NodeType.Empty);

            node.SetXY(toX, toY);
            moves.Add(node.SetPos(CalcNodePosX(toX), CalcNodePosY(toY)));
        }

        private bool IsLockedAt(int x, int y)
        {
            if (IsOutBound(x, y) || NodeMap[y, x] == null) return false;
            return NodeMap[y, x].TryGetComponent<LockedNode>(out _);
        }

        private async UniTask FillEmptyMap()
        {
            List<UniTask> moves = new();

            for (int x = 0; x < MapWidth; x++)
            {
                int emptyCountInColumn = 0;
                for (int y = 0; y < MapHeight; y++)
                {
                    if (DataMap[y, x].NodeType == NodeType.Empty) emptyCountInColumn++;
                }

                int currentSpawnOrder = 0; 
                for (int y = 0; y < MapHeight; y++)
                {
                    if (DataMap[y, x].NodeType != NodeType.Empty) continue;

                    int nodeIndex = GetSafeNonMatchingIndex(x, y);
                    Node node = Instantiate(nodePrefabs[nodeIndex], nodeBoard);
            
                    bool isIced = Random.value <= icedNodeRate;
                    NodeMap[y, x] = node;
                    node.Init(x, y, this, isIced);
                    DataMap[y, x].SetNodeType(node.NodeType);

                    float spawnY = CalcNodePosY(-(emptyCountInColumn - currentSpawnOrder));

                    node.SetPos(CalcNodePosX(x), spawnY, false);
                    moves.Add(node.SetPos(CalcNodePosX(x), CalcNodePosY(y)));

                    currentSpawnOrder++;
                }
            }

            await UniTask.WhenAll(moves);
        }

        private int GetSafeNonMatchingIndex(int x, int y)
        {
            List<int> safeIndices = new List<int>();
        
            for (int i = 0; i < nodePrefabs.Length; i++)
            {
                NodeType typeToCheck = nodePrefabs[i].NodeType;
                bool causesMatch = false;
        
                if (x >= 2)
                {
                    if (IsSameType(x - 1, y, typeToCheck) && IsSameType(x - 2, y, typeToCheck))
                        causesMatch = true;
                }
        
                if (!causesMatch && y >= 2)
                {
                    if (IsSameType(x, y - 1, typeToCheck) && IsSameType(x, y - 2, typeToCheck))
                        causesMatch = true;
                }
        
                if (!causesMatch)
                {
                    if (IsSameType(x - 1, y, typeToCheck) && IsSameType(x + 1, y, typeToCheck))
                        causesMatch = true;
                    if (IsSameType(x, y - 1, typeToCheck) && IsSameType(x, y + 1, typeToCheck))
                        causesMatch = true;
                }
        
                if (!causesMatch)
                {
                    safeIndices.Add(i);
                }
            }
        
            if (safeIndices.Count == 0) return Random.Range(0, nodePrefabs.Length);
        
            return safeIndices[Random.Range(0, safeIndices.Count)];
        }

        private bool HasMatchAt(int x, int y)
        {
            Node node = NodeMap[y, x];
            if (node == null) return false;
            if (node.IsIced) return false;

            NodeType type = DataMap[y, x].NodeType;

            int count = 1;

            for (int i = x - 1; i >= 0; i--)
            {
                Node n = NodeMap[y, i];
                if (n == null || n.IsIced) break;
                if (DataMap[y, i].NodeType != type) break;
                count++;
            }

            for (int i = x + 1; i < MapWidth; i++)
            {
                Node n = NodeMap[y, i];
                if (n == null || n.IsIced) break;
                if (DataMap[y, i].NodeType != type) break;
                count++;
            }

            if (count >= 3)
                return true;

            count = 1;

            for (int i = y - 1; i >= 0; i--)
            {
                Node n = NodeMap[i, x];
                if (n == null || n.IsIced) break;
                if (DataMap[i, x].NodeType != type) break;
                count++;
            }

            for (int i = y + 1; i < MapHeight; i++)
            {
                Node n = NodeMap[i, x];
                if (n == null || n.IsIced) break;
                if (DataMap[i, x].NodeType != type) break;
                count++;
            }

            return count >= 3;
        }

        private bool IsSameType(int x, int y, NodeType type)
        {
            if (IsOutBound(x, y)) return false;
            if (DataMap[y, x].NodeType == NodeType.Empty) return false;
    
            return DataMap[y, x].NodeType == type;
        }

        #endregion

        #region Item Function

        public async Task<bool> EnterTargetingMode(ItemTreeSO item)
        {
            if (_isSwapping) return false;
            if (item.isImmediately)
            {
                item.Execute(this, null);

                await ResolveBoard();
            }
            else
            {
                _pendingItem = item;
                SetNodeSelectImageState(true);
            }

            return true;
        }

        private void SetNodeSelectImageState(bool value)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    if(DataMap[y,x].NodeType == NodeType.Empty || DataMap[y,x].NodeType == NodeType.Locked || NodeMap[y,x].IsIced) continue;
                    
                    NodeMap[y,x].OnTargeting(value);
                }
            }
        }

        public async void OnNodeClicked(Node node)
        {
            if (_pendingItem == null || _isSwapping) return;

            ItemTreeSO item = _pendingItem;
            _pendingItem = null;
            _isSwapping = true;

            item.Execute(this, DataMap[node.Y, node.X]);
            SetNodeSelectImageState(false);
            
            await ResolveBoard();

            _isSwapping = false;
        }

        public async UniTask ShuffleBoard()
{
    if (_isSwapping) return;
    _isSwapping = true;

    List<Node> movableNodes = new List<Node>();
    List<Vector2Int> targetPositions = new List<Vector2Int>();

    for (int y = 0; y < MapHeight; y++)
    {
        for (int x = 0; x < MapWidth; x++)
        {
            Node node = NodeMap[y, x];
            if (node != null && !IsLockedAt(x, y))
            {
                movableNodes.Add(node);
                targetPositions.Add(new Vector2Int(x, y));

                DataMap[y, x].SetNodeType(NodeType.Empty);
                NodeMap[y, x] = null;
            }
        }
    }

    int safetyNet = 0;
    bool hasMatch;
    
    do
    {
        hasMatch = false;
        
        for (int i = 0; i < targetPositions.Count; i++)
        {
            int rnd = Random.Range(i, targetPositions.Count);
            (targetPositions[i], targetPositions[rnd]) = (targetPositions[rnd], targetPositions[i]);
        }

        for (int i = 0; i < movableNodes.Count; i++)
        {
            Vector2Int pos = targetPositions[i];
            DataMap[pos.y, pos.x].SetNodeType(movableNodes[i].NodeType);
        }

        if (CheckAnyMatchOnBoard()) 
        {
            hasMatch = true;
        }

        safetyNet++;
    } while (hasMatch && safetyNet < 100);

    List<UniTask> moveTasks = new List<UniTask>();

    for (int i = 0; i < movableNodes.Count; i++)
    {
        Node node = movableNodes[i];
        Vector2Int newPos = targetPositions[i];

        NodeMap[newPos.y, newPos.x] = node;
        DataMap[newPos.y, newPos.x].SetPos(newPos);
        node.SetXY(newPos.x, newPos.y);

        moveTasks.Add(node.SetPos(CalcNodePosX(newPos.x), CalcNodePosY(newPos.y)));
    }

    await UniTask.WhenAll(moveTasks);

    await ResolveBoard();

    _isSwapping = false;
}

private bool CheckAnyMatchOnBoard()
{
    for (int y = 0; y < MapHeight; y++)
    {
        for (int x = 0; x < MapWidth; x++)
        {
            NodeType type = DataMap[y, x].NodeType;
            if (type == NodeType.Empty || type == NodeType.Locked) continue;

            if (x < MapWidth - 2)
            {
                if (DataMap[y, x + 1].NodeType == type && DataMap[y, x + 2].NodeType == type)
                    return true;
            }

            if (y < MapHeight - 2)
            {
                if (DataMap[y + 1, x].NodeType == type && DataMap[y + 2, x].NodeType == type)
                    return true;
            }
        }
    }
    return false;
}

        public void SetGetDouble(bool value)
        {
            _isGetDouble = value;
            _getDoubleCnt = 7;
        }

        // 가로 한줄
        public void RemoveHorizontal(int y)
        {
            if (y < 0 || y >= MapHeight) return;

            for (int x = 0; x < MapWidth; ++x)
            {
                if (DataMap[y, x].NodeType == NodeType.Locked)
                {
                    var locked = NodeMap[y, x].GetComponent<LockedNode>();
                    if (locked.DiscountCnt())
                    {
                        AddRemoveNode(x, y);
                    }
                }
                else if (NodeMap[y, x].IsIced)
                {
                    NodeMap[y,x].Unfreeze();
                }
                else
                {
                    NodeMap[y, x].DestroyState();
                    AddRemoveNode(x, y);
                }
            }

            float posY = NodeMap[y, 0].CenterPos.y;
            Vector2 pos = new Vector2(0, posY);
            particleEventChannel.InvokeEvent(ParticleEvents.PlayUIParticleEvent.Initializer(lineEffectItem, pos));
        }

        // 세로 한줄
        public void RemoveVertical(int x)
        {
            if (x < 0 || x >= MapWidth) return;

            for (int y = 0; y < MapHeight; ++y)
            {
                if (DataMap[y, x].NodeType == NodeType.Locked)
                {
                    var locked = NodeMap[y, x].GetComponent<LockedNode>();
                    if (locked.DiscountCnt())
                    {
                        AddRemoveNode(x, y);
                    }
                }
                else if (NodeMap[y, x].IsIced)
                {
                    NodeMap[y,x].Unfreeze();
                }
                else
                {
                    NodeMap[y, x].DestroyState();
                    AddRemoveNode(x, y);
                }
            }

            float posX = NodeMap[0, x].CenterPos.x;
            Vector2 pos = new Vector2(posX, 0);
            particleEventChannel.InvokeEvent(ParticleEvents.PlayUIParticleEvent.Initializer(lineEffectItem, pos,
                Quaternion.Euler(new Vector3(0, 0, 90f))));
        }

        // 맵의 모든 한 타입을 제거
        public void RemoveNodeByType(NodeType type)
        {
            for (int y = 0; y < MapHeight; y++)
            for (int x = 0; x < MapWidth; x++)
            {
                if (NodeMap[y, x] != null && DataMap[y, x].NodeType == type)
                {
                    if (DataMap[y, x].NodeType == NodeType.Locked)
                    {
                        var locked = NodeMap[y, x].GetComponent<LockedNode>();
                        if (locked.DiscountCnt())
                        {
                            AddRemoveNode(x, y);
                        }
                    }
                    else if (NodeMap[y, x].IsIced)
                    {
                        NodeMap[y,x].Unfreeze();
                    }
                    else
                    {
                        NodeMap[y, x].DestroyState();
                        AddRemoveNode(x, y);
                    }
                }
            }
        }

        public void RemoveUnLockedNode()
        {
            for (int y = 0; y < MapHeight; y++)
            for (int x = 0; x < MapWidth; x++)
            {
                if (NodeMap[y, x] != null && !NodeMap[y, x].IsIced && DataMap[y, x].NodeType != NodeType.Locked)
                {
                    AddRemoveNode(x, y);
                }
            }
        }

        public void RemoveAllNode()
        {
            for (int y = 0; y < MapHeight; y++)
            for (int x = 0; x < MapWidth; x++)
            {
                AddRemoveNode(x, y);
            }

            Vector2 pos = new Vector2(15, 0);
            particleEventChannel.InvokeEvent(ParticleEvents.PlayUIParticleEvent.Initializer(nyanCatEffectItem, pos));
        }

        public void BreakIce()
        {
            for (int y = 0; y < MapHeight; y++)
            for (int x = 0; x < MapWidth; x++)
            {
                TryBreakGimmick(x, y);
            }
        }

        public void BreakLock()
        {
            for (int y = 0; y < MapHeight; y++)
            for (int x = 0; x < MapWidth; x++)
            {
                if(NodeMap[y,x].NodeType == NodeType.Locked)
                    AddRemoveNode(x, y);
            }
        }

        public void Remove8Direction(int x, int y)
        {
            if (!IsOutBound(x, y))
                AddRemoveNode(x, y);

            for (int i = 0; i < 8; i++)
            {
                int nx = x + _eightDirection[i].x;
                int ny = y + _eightDirection[i].y;

                if (IsOutBound(nx, ny))
                    continue;

                AddRemoveNode(nx, ny);
            }

            particleEventChannel.InvokeEvent(
                ParticleEvents.PlayUIParticleEvent.Initializer(threeXthreeEffectItem, NodeMap[y, x].CenterPos));
        }

        #endregion

        #region Gimmick Logic

        private void BreakAdjacentIce()
        {
            foreach (var set in _removeNodesDict.Values)
            {
                foreach (var data in set)
                {
                    if(data.NodeType == NodeType.Locked || NodeMap[data.Pos.y, data.Pos.x] == null || NodeMap[data.Pos.y, data.Pos.x].IsDestroyByItem) continue;
                    Vector2Int pos = data.Pos; // NodeData에 좌표 필요
                    TryBreakGimmick(pos.x + 1, pos.y, true);
                    TryBreakGimmick(pos.x - 1, pos.y, true);
                    TryBreakGimmick(pos.x, pos.y + 1, true);
                    TryBreakGimmick(pos.x, pos.y - 1, true);
                }
            }
        }

        private void TryBreakGimmick(int x, int y, bool targetIsEvery = false, bool targetIsIced = true)
        {
            if (IsOutBound(x, y)) return;

            Node node = NodeMap[y, x];

            if (targetIsEvery)
            {
                if (node != null && node.IsIced)
                {
                    poolManager.Pop<SoundPlayer>(soundPlayer).PlaySound(iceBreakSound);
                    particleEventChannel.InvokeEvent(
                        ParticleEvents.PlayUIParticleEvent.Initializer(icedEffectItem, node.CenterPos,
                            Quaternion.identity));
                    node.Unfreeze();
                }
                else if (node != null && !node.IsIced && node.TryGetComponent(out LockedNode lockedNode))
                {
                    if (lockedNode.DiscountCnt())
                    {
                        AddRemoveNode(node.X, node.Y);
                    }
                }

                return;
            }

            if (targetIsIced)
            {
                if (node != null && node.IsIced)
                {
                    poolManager.Pop<SoundPlayer>(soundPlayer).PlaySound(iceBreakSound);

                    particleEventChannel.InvokeEvent(
                        ParticleEvents.PlayUIParticleEvent.Initializer(icedEffectItem, node.CenterPos,
                            Quaternion.identity));
                    node.Unfreeze();
                }
            }
            else
            {
                if (node != null && !node.IsIced && node.TryGetComponent(out LockedNode lockedNode))
                {
                    if (lockedNode.DiscountCnt())
                    {
                        AddRemoveNode(node.X, node.Y);
                    }
                }
            }
        }

        #endregion

        private bool IsOutBound(int x, int y)
            => x < 0 || x >= MapWidth || y < 0 || y >= MapHeight;

        public void AddTurn()
        {
            gameEventChannel.InvokeEvent(GameEvents.TurnAmountEvent.Initializer(turnAddAmount));
        }

        private void DiscountTurn()
        {
            if (_isGetDouble)
            {
                _getDoubleCnt--;
                _isGetDouble = _getDoubleCnt != 0;
            }

            gameEventChannel.InvokeEvent(GameEvents.TurnAmountEvent.Initializer(-1));
        }
    }
}