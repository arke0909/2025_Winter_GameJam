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

    public class MatchSystem : MonoBehaviour
    {
        [SerializeField] private EventChannelSO gameEventChannel;

        [SerializeField] private List<Vector2Int> icedNode, lockedNode;
        
        [field: SerializeField] public int MapWidth { get; private set; } = 8;
        [field: SerializeField] public int MapHeight { get; private set; } = 8;

        public Node[,] Map { get; private set; }

        private HashSet<Vector2Int> _nodePosSet = new HashSet<Vector2Int>();
        private Dictionary<NodeType, HashSet<Node>> _removeNodesDict = new Dictionary<NodeType, HashSet<Node>>();

        private void Awake()
        {
            Map = new Node[MapHeight, MapWidth];

            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    var pos = new Vector2Int(x, y);
                    
                    if (icedNode.Contains(pos))
                    {
                        Map[y, x] = new Node(NodeType.Iced);
                        continue;
                    }
                    
                    if (lockedNode.Contains(pos))
                    {
                        Map[y, x] = new Node(NodeType.Locked);
                        continue;
                    }
                    
                    Map[y, x] = new Node(NodeType.Empty);
                }
            }
            
            FillEmptyMap();
        }

        private void FillEmptyMap()
        {
            List<Node> emptyNodes = new List<Node>();
            
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    Node node = Map[y, x];
                    if (node.NodeType == NodeType.Empty)
                    {
                        int nodeType = Random.Range(0, (int)NodeType.Ingredient);
                        emptyNodes.Add(node);
                        node.SetNodeType((NodeType)nodeType);
                    }
                }
            }

            if (!CanMatch())
            {
                foreach (Node node in emptyNodes)
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
                    Node before = Map[y - 1, x];
                    Node node = Map[y, x];
                    Node next = Map[y + 1, x];

                    if (before.NodeType == node.NodeType && node.NodeType == next.NodeType)
                    {
                        // 위 아래가 같은 타입일 경우, 좌표를 담아준다.
                        Vector2Int pos = new Vector2Int(x, y);
                        _nodePosSet.Add(pos);
                        AddRemoveNode(before, node, next);
                    }
                }
            }

            // 가로축 계산
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 1; x < MapWidth - 1; x++)
                {
                    Node before = Map[y, x - 1];
                    Node node = Map[y, x];
                    Node next = Map[y, x + 1];

                    if (before.NodeType == node.NodeType && node.NodeType == next.NodeType)
                    {
                        // 위 아래가 같은 타입일 경우, 좌표를 담아준다.
                        Vector2Int pos = new Vector2Int(x, y);
                        AddRemoveNode(before, node, next);
                    }
                }
            }

            // 아이템 획득 및 노드 제거
            GetIngredientAndRemoveNode();
            // 아래 노드가 비었으면 아래로 이동 및 노드 빈 곳 채우기
            SortingNodeMap();
        }

        private void AddRemoveNode(params Node[] nodes)
        {
            _removeNodesDict.TryGetValue(nodes[1].NodeType, out HashSet<Node> removeNodes);

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
                    GetIngredientData data = new GetIngredientData{nodeType = kvp.Key, count = kvp.Value.Count };
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
                    Node targetNode = Map[y, x];
                    Node underNode = Map[y - 1, x];
                    
                    // 현재 아래 노드가 비어있다면 타입을 이동시키기
                    if (underNode.NodeType == NodeType.Empty)
                    {
                        underNode.SetNodeType(targetNode.NodeType);
                        targetNode.SetNodeType(NodeType.Empty);
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
                    Node current = Map[y, x];
        
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
        
                        if (nx < 0 || nx >= MapWidth || ny < 0 || ny >= MapHeight)
                            continue;
        
                        Node target = Map[ny, nx];
        
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
        
        private void SwapNodeType(Node a, Node b)
        {
            NodeType temp = a.NodeType;
            a.SetNodeType(b.NodeType);
            b.SetNodeType(temp);
        }
        
        private bool HasMatchAt(int x, int y)
        {
            NodeType type = Map[y, x].NodeType;

            // 가로 검사
            int count = 1;
            for (int i = x - 1; i >= 0 && Map[y, i].NodeType == type; i--) count++;
            for (int i = x + 1; i < MapWidth && Map[y, i].NodeType == type; i++) count++;
            if (count >= 3) return true;

            // 세로 검사
            count = 1;
            for (int i = y - 1; i >= 0 && Map[i, x].NodeType == type; i--) count++;
            for (int i = y + 1; i < MapHeight && Map[i, x].NodeType == type; i++) count++;
            if (count >= 3) return true;

            return false;
        }
    }
}