using UnityEngine;
using Work.Code.MatchSystem;

namespace Work.Code.Items
{
    [CreateAssetMenu(fileName = "RemoveVertical", menuName = "SO/Item/Node/RemoveVertical", order = 0)]
    public class RemoveVerticalNodeSo : EffectNodeSO
    {
        public override void Execute(MatchSystem.MatchSystem ms, NodeData data)
        {
            ms.RemoveVertical(data.Pos.x);
        }
    }
}