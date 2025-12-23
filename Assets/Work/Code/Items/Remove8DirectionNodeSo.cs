using UnityEngine;
using Work.Code.MatchSystem;

namespace Work.Code.Items
{
    [CreateAssetMenu(fileName = "Remove8Direction", menuName = "SO/Item/Node/Remove8Direction", order = 0)]
    public class Remove8DirectionNodeSo : EffectNodeSO
    {
        public override void Execute(MatchSystem.MatchSystem ms, NodeData data)
        {
            ms.Remove8Direction(data.Pos.x, data.Pos.y);
        }
    }
}