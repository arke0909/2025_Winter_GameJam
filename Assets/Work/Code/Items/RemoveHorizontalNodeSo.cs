using UnityEngine;
using Work.Code.MatchSystem;

namespace Work.Code.Items
{
    [CreateAssetMenu(fileName = "RemoveHorizontal", menuName = "SO/Item/Node/RemoveHorizontal", order = 0)]
    public class RemoveHorizontalNodeSo : EffectNodeSO
    {
        public override void Execute(MatchSystem.MatchSystem ms, NodeData data)
        {
            ms.RemoveHorizontal(data.Pos.y);
        }
    }
}