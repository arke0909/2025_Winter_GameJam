using UnityEngine;
using Work.Code.MatchSystem;

namespace Work.Code.Items
{
    [CreateAssetMenu(fileName = "GetDouble", menuName = "SO/Item/Node/GetDouble", order = 0)]
    public class GetDoubleNodeSo : EffectNodeSO
    {
        public override void Execute(MatchSystem.MatchSystem ms, NodeData data)
        {
            ms.SetGetDouble(true);
        }
    }
}