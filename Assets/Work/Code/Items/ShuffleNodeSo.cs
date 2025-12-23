using UnityEngine;
using Work.Code.MatchSystem;

namespace Work.Code.Items
{
    [CreateAssetMenu(fileName = "Shuffle", menuName = "SO/Item/Node/Shuffle", order = 0)]
    public class ShuffleNodeSo : EffectNodeSO
    {
        public override async void Execute(MatchSystem.MatchSystem ms, NodeData data)
        {
            await ms.ShuffleBoard();
        }
    }
}