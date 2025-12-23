using UnityEngine;
using Work.Code.MatchSystem;

namespace Work.Code.Items
{
    [CreateAssetMenu(fileName = "BreakLock", menuName = "SO/Item/Node/BreakLock", order = 0)]
    public class BreakLockNodeSo : EffectNodeSO
    {
        public override void Execute(MatchSystem.MatchSystem ms, NodeData data)
        {
            ms.BreakLock();
        }
    }
}