namespace Work.Code.MatchSystem
{
    public enum NodeType
    {
        Tomato = 0,
        Carrot = 1,
        Corn = 2,
        Paprika = 3,
        SweetPotato = 4,
        Radish = 5,
        Ingredient  = 6,
        Iced = 7,
        Locked = 8,
        Empty = 9,
        End = 10
    }
    
    public class NodeData
    {
        public NodeType NodeType { get; private set; }

        public NodeData(NodeType nodeType)
        {
            NodeType = nodeType;
        }
        
        public void SetNodeType(NodeType nodeType) => NodeType = nodeType;
    }
}