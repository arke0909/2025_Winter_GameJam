using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Lib.Utiles;
using UnityEngine;
using Work.Code.Events;
using Work.Code.Supply;
using Random = UnityEngine.Random;

namespace Work.Code.MatchSystem
{
    public struct GetIngredientData
    {
        public SupplyType SupplyType;
        public int count;
    }

    public class MatchSystem : MonoBehaviour
    {
        [SerializeField] private EventChannelSO gameEventChannel;
        [SerializeField] private Node[] nodePrefabs;
        [SerializeField] private Node lockedNodePrefab;
        [SerializeField] private RectTransform nodeBoard;
        [SerializeField] private List<Vector2Int> lockedNode;
        [Range(0, 1f),SerializeField] private float icedNodeRate;

        public NodeData[,] DataMap { get; private set; }
        public Node[,] NodeMap { get; private set; }

        [field: SerializeField] public int MapWidth { get; private set; } = 8;
        [field: SerializeField] public int MapHeight { get; private set; } = 8;

        private float _nodeWidth, _nodeHeight, _widthTerm, _heightTerm;
        private bool _isSwapping;

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

        private void SetNodes()
        {
            for (int y = 0; y < MapHeight; y++)
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
                    node = CreateNode();
                    bool isIced = Random.value <= icedNodeRate;
                    NodeMap[y, x] = node;
                    node.Init(x, y, this, isIced);
                }
                
                node.SetPos(CalcNodePosX(x), CalcSpawnPosY(x), false);
                node.SetPos(CalcNodePosX(x), CalcNodePosY(y));
                DataMap[y, x] = new NodeData(node.NodeType);
                DataMap[y, x].SetPos(new Vector2Int(x, y));
            }
        }

        public async void TrySwapByDir(Node node, Vector2Int dir)
        {
            if (_isSwapping) return;

            int nx = node.X + dir.x;
            int ny = node.Y + dir.y;
            if (IsOutBound(nx, ny)) return;

            Node target = NodeMap[ny, nx];
            if (target == null) return;

            _isSwapping = true;

            bool success = await TrySwap(node, target);
            if (success)
                await ResolveBoard();

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

        private async UniTask ResolveBoard()
        {
            while (true)
            {
                bool isMatch = CheckMatch();
                if (!isMatch)
                    break;

                BreakAdjacentIce();
                GetIngredient();
                RemoveNode();

                await SortingNodeMap();
            }

            await FillEmptyMap();

            if (CheckMatch())
            {
                await ResolveBoard();
            }
        }

        private bool CheckMatch()
        {
            foreach (var set in _removeNodesDict.Values)
                set.Clear();

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

                gameEventChannel.InvokeEvent(
                    SupplyEvents.SupplyEvent.Initializer
                        ((SupplyType)kv.Key, kv.Value.Count));

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
                    Destroy(NodeMap[y, x].gameObject);
                    NodeMap[y, x] = null;
                }
            }
        }

        public void RemoveHorizontal(int y)
        {
            for (int x = 0; x < MapWidth; ++x)
            {
                AddRemoveNode(x, y);
            }
        }

        public void RemoveVertical(int x)
        {
            for (int y = 0; y < MapHeight; ++y)
            {
                AddRemoveNode(x, y);
            }
        }

        public void RemoveCross(int x, int y)
        {
            RemoveVertical(x);
            RemoveHorizontal(y);
        }

        private void BreakAdjacentIce()
        {
            foreach (var set in _removeNodesDict.Values)
            {
                foreach (var data in set)
                {
                    Vector2Int pos = data.Pos; // NodeData에 좌표 필요
                    TryBreakIce(pos.x + 1, pos.y);
                    TryBreakIce(pos.x - 1, pos.y);
                    TryBreakIce(pos.x, pos.y + 1);
                    TryBreakIce(pos.x, pos.y - 1);
                }
            }
        }

        private void TryBreakIce(int x, int y)
        {
            if (IsOutBound(x, y)) return;

            Node node = NodeMap[y, x];
            if (node != null && node.IsIced)
            {
                node.Unfreeze();
            }
            else if (node != null && node.TryGetComponent<LockedNode>(out LockedNode lockedNode))
            {
                if (lockedNode.DiscountCnt())
                {
                    AddRemoveNode(node.X, node.Y);
                }
            }
        }
        
        private async UniTask SortingNodeMap()
        {
            List<UniTask> moves = new();

            for (int x = 0; x < MapWidth; x++)
            {
                int writeY = MapHeight - 1;

                for (int y = MapHeight - 1; y >= 0; y--)
                {
                    Node node = NodeMap[y, x];
                    if (node == null) continue;

                    if (y != writeY)
                    {
                        NodeMap[writeY, x] = node;
                        NodeMap[y, x] = null;

                        DataMap[writeY, x].SetNodeType(DataMap[y, x].NodeType);
                        DataMap[y, x].SetNodeType(NodeType.Empty);

                        node.SetXY(x, writeY);
                        moves.Add(
                            node.SetPos(CalcNodePosX(x), CalcNodePosY(writeY))
                        );
                    }

                    writeY--;
                }
            }

            await UniTask.WhenAll(moves);
        }

        private async UniTask FillEmptyMap()
        {
            List<UniTask> moves = new();

            for (int x = 0; x < MapWidth; x++)
            {
                int spawnIndex = 0;

                for (int y = MapHeight - 1; y >= 0; y--)
                {
                    if (DataMap[y, x].NodeType != NodeType.Empty)
                        continue;

                    Node node = CreateNode();
                    bool isIced = Random.value <= icedNodeRate;
                    NodeMap[y, x] = node;
                    node.Init(x, y, this, isIced);

                    float spawnY = CalcNodePosY(-1 - spawnIndex);
                    node.SetPos(CalcNodePosX(x), spawnY, false);

                    moves.Add(
                        node.SetPos(CalcNodePosX(x), CalcNodePosY(y))
                    );

                    DataMap[y, x].SetNodeType(node.NodeType);
                    spawnIndex++;
                }
            }

            await UniTask.WhenAll(moves);
        }

        private void SwapNodeType(NodeData a, NodeData b)
        {
            NodeType t = a.NodeType;
            a.SetNodeType(b.NodeType);
            b.SetNodeType(t);
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


        private bool IsOutBound(int x, int y)
            => x < 0 || x >= MapWidth || y < 0 || y >= MapHeight;
    }
}