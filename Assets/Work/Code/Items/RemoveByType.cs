using UnityEngine;
using Work.Code.MatchSystem;

namespace Work.Code.Items
{
    [CreateAssetMenu(fileName = "RemoveByType", menuName = "SO/Item/Node/RemoveByType", order = 0)]
    public class RemoveByType : EffectNodeSO
    {
        public override void Execute(MatchSystem.MatchSystem ms, NodeData data)
        {
            ms.RemoveNodeByType(data.NodeType);
        }
    }
}