using System.Collections.Generic;
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

    public struct NodeSwapData
    {
        public Node node;
        public bool isConform;
    }

    public class MatchSystem : MonoBehaviour
    {
        [SerializeField] private EventChannelSO gameEventChannel;
        [SerializeField] private Node[] nodePrefabs;
        [SerializeField] private Node icedNodePrefab, lockedNodePrefab;
        [SerializeField] private RectTransform nodeBoard;

        [SerializeField] private List<Vector2Int> icedNode, lockedNode;

        private MatchSystem _matchSystem;

        public float NodeBoardWidth { get; private set; }
        public float NodeBoardHeight { get; private set; }

        private float _nodeWidth, _nodeHeight;

        [field: SerializeField] public int MapWidth { get; private set; } = 8;
        [field: SerializeField] public int MapHeight { get; private set; } = 8;

        public NodeData[,] DataMap { get; private set; }
        public Node[,] NodeMap { get; private set; }

        private HashSet<Vector2Int> _nodePosSet = new HashSet<Vector2Int>();

        private Dictionary<NodeType, HashSet<NodeData>>
            _removeNodesDict = new Dictionary<NodeType, HashSet<NodeData>>();

        public void Awake()
        {
            DataMap = new NodeData[MapHeight, MapWidth];
            NodeMap = new Node[MapHeight, MapWidth];
            CalcBoardSize();
            SetNodes();
        }
        
        private void CalcBoardSize()
        {
            RectTransform nodeRt = nodePrefabs[0].transform as RectTransform;
            _nodeWidth = nodeRt.rect.width;
            _nodeHeight = nodeRt.rect.height;

            NodeBoardWidth = nodeBoard.rect.width;
            NodeBoardHeight = nodeBoard.rect.height;
        }

        private void SetNodes()
        {
            float remainWidth = NodeBoardWidth - _nodeWidth * MapWidth;
            float remainHeight = NodeBoardHeight - _nodeHeight * MapHeight;

            float widthTerm = remainWidth / MapWidth;
            float heightTerm = remainHeight / MapHeight;

            for (int x = MapWidth - 1; x >= 0; x--)
            {
                for (int y = MapHeight - 1; y >= 0; y--)
                {
                    var pos = new Vector2Int(x, y);

                    Node node;

                    if (icedNode.Contains(pos))
                    {
                        node = Instantiate(icedNodePrefab, nodeBoard);
                    }
                    else if (lockedNode.Contains(pos))
                    {
                        node = Instantiate(lockedNodePrefab, nodeBoard);
                    }
                    else
                    {
                        int idx = Random.Range(0, nodePrefabs.Length);
                        node = Instantiate(nodePrefabs[idx], nodeBoard);
                    }

                    NodeMap[y, x] = node;
                    node.Init(x, y, this);
                    node.SetPos(x * _nodeWidth + (x + 0.5f) * widthTerm, y * -_nodeHeight - (y + 0.5f) * heightTerm);
                    SetNodeData(x, y, node.NodeType);
                }
            }
        }

        private void SetNodeData(int x, int y, NodeType nodeType)
        {
            if (DataMap[y, x] == null)
            {
                DataMap[y, x] = new NodeData(nodeType);
            }
            else
            {
                DataMap[y, x].SetNodeType(nodeType);
            }
        }

        private void FillEmptyMap()
        {
            List<NodeData> emptyNodes = new List<NodeData>();

            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    NodeData nodeData = DataMap[y, x];
                    if (nodeData.NodeType == NodeType.Empty)
                    {
                        int nodeType = Random.Range(0, (int)NodeType.Ingredient);
                        emptyNodes.Add(nodeData);
                        nodeData.SetNodeType((NodeType)nodeType);
                    }
                }
            }

            if (!CanMatch())
            {
                foreach (NodeData node in emptyNodes)
                {
                    node.SetNodeType(NodeType.Empty);
                }

                FillEmptyMap();
            }
        }

        private void CheckMatch()
        {
            _nodePosSet.Clear();
            _removeNodesDict.Clear();

            // 세로축 계산
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 1; y < MapHeight - 1; y++)
                {
                    NodeData before = DataMap[y - 1, x];
                    NodeData nodeData = DataMap[y, x];
                    NodeData next = DataMap[y + 1, x];

                    if (before.NodeType == nodeData.NodeType && nodeData.NodeType == next.NodeType)
                    {
                        // 위 아래가 같은 타입일 경우, 좌표를 담아준다.
                        Vector2Int pos = new Vector2Int(x, y);
                        _nodePosSet.Add(pos);
                        AddRemoveNode(before, nodeData, next);
                    }
                }
            }

            // 가로축 계산
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 1; x < MapWidth - 1; x++)
                {
                    NodeData before = DataMap[y, x - 1];
                    NodeData nodeData = DataMap[y, x];
                    NodeData next = DataMap[y, x + 1];

                    if (before.NodeType == nodeData.NodeType && nodeData.NodeType == next.NodeType)
                    {
                        // 위 아래가 같은 타입일 경우, 좌표를 담아준다.
                        Vector2Int pos = new Vector2Int(x, y);
                        AddRemoveNode(before, nodeData, next);
                    }
                }
            }

            // 아이템 획득 및 노드 제거
            GetIngredientAndRemoveNode();
            // 아래 노드가 비었으면 아래로 이동 및 노드 빈 곳 채우기
            SortingNodeMap();
        }

        private void AddRemoveNode(params NodeData[] nodes)
        {
            _removeNodesDict.TryGetValue(nodes[1].NodeType, out HashSet<NodeData> removeNodes);

            if (removeNodes == null) return;

            foreach (var node in nodes)
            {
                removeNodes.Add(node);
            }
        }

        private void GetIngredientAndRemoveNode()
        {
            // 삭제될 노드들에서 종류마다 
            foreach (var kvp in _removeNodesDict)
            {
                // 각 노드 타입마다 삭제될 애들의 타입과 카운트를 이벤트로 발행
                foreach (var node in kvp.Value)
                {
                    // 아이템 획득
                    GetIngredientData data = new GetIngredientData { nodeType = kvp.Key, count = kvp.Value.Count };
                    gameEventChannel.InvokeEvent(GameEvents.GetIngredientEvent.Init(data));

                    node.SetNodeType(NodeType.Empty);
                }
            }
        }

        // 빈 노드가 있으면, 이동시키기
        private void SortingNodeMap()
        {
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 1; y < MapHeight; y++)
                {
                    NodeData targetNodeData = DataMap[y, x];
                    NodeData underNodeData = DataMap[y - 1, x];

                    // 현재 아래 노드가 비어있다면 타입을 이동시키기
                    if (underNodeData.NodeType == NodeType.Empty)
                    {
                        underNodeData.SetNodeType(targetNodeData.NodeType);
                        targetNodeData.SetNodeType(NodeType.Empty);
                    }
                }
            }

            FillEmptyMap();
        }

        private bool CanMatch()
        {
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    NodeData current = DataMap[y, x];

                    if (current.NodeType == NodeType.Empty ||
                        current.NodeType == NodeType.Iced ||
                        current.NodeType == NodeType.Locked)
                        continue;

                    Vector2Int[] directions =
                    {
                        Vector2Int.right,
                        Vector2Int.up
                    };

                    foreach (var dir in directions)
                    {
                        int nx = x + dir.x;
                        int ny = y + dir.y;

                        if (IsOutBound(nx, ny))
                            continue;

                        NodeData target = DataMap[ny, nx];

                        if (target.NodeType == NodeType.Empty ||
                            target.NodeType == NodeType.Iced ||
                            target.NodeType == NodeType.Locked)
                            continue;

                        SwapNodeType(current, target);

                        if (HasMatchAt(x, y) || HasMatchAt(nx, ny))
                        {
                            SwapNodeType(current, target);
                            return true;
                        }

                        SwapNodeType(current, target);
                    }
                }
            }

            return false;
        }
        
        public void TrySwapByDir(Node node, Vector2Int dir)
        {
            int x2 = node.X + dir.x;
            int y2 = node.Y + dir.y;

            if (IsOutBound(x2, y2))
                return;

            Node target = NodeMap[y2, x2];
            if (target == null)
                return;

            if (TrySwap(node, target))
            {
                CheckMatch();
            }
        }

        private void SwapNodeType(NodeData a, NodeData b)
        {
            NodeType temp = a.NodeType;
            a.SetNodeType(b.NodeType);
            b.SetNodeType(temp);
        }

        private bool HasMatchAt(int x, int y)
        {
            NodeType type = DataMap[y, x].NodeType;

            // 가로 검사
            int count = 1;
            for (int i = x - 1; i >= 0 && DataMap[y, i].NodeType == type; i--) count++;
            for (int i = x + 1; i < MapWidth && DataMap[y, i].NodeType == type; i++) count++;
            if (count >= 3) return true;

            // 세로 검사
            count = 1;
            for (int i = y - 1; i >= 0 && DataMap[i, x].NodeType == type; i--) count++;
            for (int i = y + 1; i < MapHeight && DataMap[i, x].NodeType == type; i++) count++;
            if (count >= 3) return true;

            return false;
        }
        
        private bool IsOutBound(int x, int y) => (x < 0 || x >= MapWidth || y < 0 || y >= MapHeight);

        public bool TrySwap(Node a, Node b)
        {
            SwapInternal(a, b);

            if (HasMatchAt(a.X, a.Y) || HasMatchAt(b.X, b.Y))
            {
                Debug.Log("매치됨");
                return true;
            }

            Debug.Log("매치안됨");
            SwapInternal(a, b);
            return false;
        }
        
        private void SwapInternal(Node a, Node b)
        {
            NodeMap[a.Y, a.X] = b;
            NodeMap[b.Y, b.X] = a;

            SwapNodeType(DataMap[a.Y, a.X], DataMap[b.Y, b.X]);

            int ax = a.X, ay = a.Y;
            a.SetXY(b.X, b.Y);
            b.SetXY(ax, ay);

            Vector2 apos = a.Rect.anchoredPosition;
            a.SetPos(b.Rect.anchoredPosition.x, b.Rect.anchoredPosition.y);
            b.SetPos(apos.x, apos.y);
        }
    }
}