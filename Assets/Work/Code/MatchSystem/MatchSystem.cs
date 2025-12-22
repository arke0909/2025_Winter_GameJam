using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Lib.Utiles;
using UnityEngine;
using Work.Code.Events;
using Random = UnityEngine.Random;

namespace Work.Code.MatchSystem
{
    public struct GetIngredientData
    {
        public NodeType nodeType;
        public int count;
    }

    public class MatchSystem : MonoBehaviour
    {
        [SerializeField] private EventChannelSO gameEventChannel;
        [SerializeField] private Node[] nodePrefabs;
        [SerializeField] private Node icedNodePrefab, lockedNodePrefab;
        [SerializeField] private RectTransform nodeBoard;
        [SerializeField] private List<Vector2Int> icedNode, lockedNode;

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

        private Node CreateNode()
        {
            return Instantiate(nodePrefabs[Random.Range(0, nodePrefabs.Length)], nodeBoard);
        }

        private void SetNodes()
        {
            for (int y = 0; y < MapHeight; y++)
            for (int x = 0; x < MapWidth; x++)
            {
                Node node = CreateNode();
                NodeMap[y, x] = node;
                node.Init(x, y, this);
                node.SetPos(CalcNodePosX(x), CalcNodePosY(-1), false);
                node.SetPos(CalcNodePosX(x), CalcNodePosY(y));
                DataMap[y, x] = new NodeData(node.NodeType);
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
                ResolveBoard();

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

        private void ResolveBoard()
        {
            while (CheckMatch())
            {
                GetIngredientAndRemoveNode();
                SortingNodeMap();
                FillEmptyMap();
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
                    _removeNodesDict[DataMap[y, x].NodeType].Add(DataMap[y, x]);
                    hasMatch = true;
                }
            }

            return hasMatch;
        }

        private void GetIngredientAndRemoveNode()
        {
            foreach (var kv in _removeNodesDict)
            {
                if (kv.Value.Count == 0) continue;

                gameEventChannel.InvokeEvent(
                    GameEvents.GetIngredientEvent.Init(
                        new GetIngredientData { nodeType = kv.Key, count = kv.Value.Count }
                    )
                );

                foreach (var data in kv.Value)
                    data.SetNodeType(NodeType.Empty);
            }

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

        private void SortingNodeMap()
        {
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
                        node.SetPos(CalcNodePosX(x), CalcNodePosY(writeY));
                    }
                    writeY--;
                }
            }
        }

        private void FillEmptyMap()
        {
            for (int y = 0; y < MapHeight; y++)
            for (int x = 0; x < MapWidth; x++)
            {
                if (DataMap[y, x].NodeType != NodeType.Empty) continue;

                Node node = CreateNode();
                NodeMap[y, x] = node;
                node.Init(x, y, this);
                node.SetPos(CalcNodePosX(x), CalcNodePosY(-1), false);
                node.SetPos(CalcNodePosX(x), CalcNodePosY(y));
                DataMap[y, x].SetNodeType(node.NodeType);
            }
        }

        private void SwapNodeType(NodeData a, NodeData b)
        {
            NodeType t = a.NodeType;
            a.SetNodeType(b.NodeType);
            b.SetNodeType(t);
        }

        private bool HasMatchAt(int x, int y)
        {
            NodeType type = DataMap[y, x].NodeType;
            if (type == NodeType.Empty) return false;

            int count = 1;
            for (int i = x - 1; i >= 0 && DataMap[y, i].NodeType == type; i--) count++;
            for (int i = x + 1; i < MapWidth && DataMap[y, i].NodeType == type; i++) count++;
            if (count >= 3) return true;

            count = 1;
            for (int i = y - 1; i >= 0 && DataMap[i, x].NodeType == type; i--) count++;
            for (int i = y + 1; i < MapHeight && DataMap[i, x].NodeType == type; i++) count++;
            return count >= 3;
        }

        private bool IsOutBound(int x, int y)
            => x < 0 || x >= MapWidth || y < 0 || y >= MapHeight;
    }
}
