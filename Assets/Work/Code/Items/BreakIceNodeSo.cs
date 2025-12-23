using UnityEngine;
using Work.Code.MatchSystem;

namespace Work.Code.Items
{
    [CreateAssetMenu(fileName = "BreakIce", menuName = "SO/Item/Node/BreakIce", order = 0)]
    public class BreakIceNodeSo : EffectNodeSO
    {
        public override void Execute(MatchSystem.MatchSystem ms, NodeData data)
        {
            ms.BreakIce();
        }
    }
}