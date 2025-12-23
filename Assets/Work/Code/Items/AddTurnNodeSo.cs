using UnityEngine;
using Work.Code.MatchSystem;

namespace Work.Code.Items
{
    [CreateAssetMenu(fileName = "ItemTree", menuName = "SO/Item/Node/AddTurn", order = 0)]
    public class AddTurnNodeSo : EffectNodeSO
    {
        public override void Execute(MatchSystem.MatchSystem ms, NodeData data)
        {
            ms.AddTurn();
        }
    }
}