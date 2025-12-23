using UnityEngine;
using Work.Code.MatchSystem;

namespace Work.Code.Items
{
    [CreateAssetMenu(fileName = "RemoveUnlock", menuName = "SO/Item/Node/RemoveUnlock", order = 0)]
    public class RemoveUnlockNodeSo : EffectNodeSO
    {
        public override void Execute(MatchSystem.MatchSystem ms, NodeData data)
        {
            ms.RemoveUnLockedNode();
        }
    }
}