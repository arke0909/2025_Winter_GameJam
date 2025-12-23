using UnityEngine;
using Work.Code.MatchSystem;

namespace Work.Code.Items
{
    [CreateAssetMenu(fileName = "RemoveAll", menuName = "SO/Item/Node/RemoveAll", order = 0)]
    public class RemoveAllNodeSo : EffectNodeSO
    {
        public override void Execute(MatchSystem.MatchSystem ms, NodeData data)
        {
            ms.RemoveAllNode();
        }
    }
}