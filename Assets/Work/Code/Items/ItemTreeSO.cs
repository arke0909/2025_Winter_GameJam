using System.Collections.Generic;
using UnityEngine;
using Work.Code.MatchSystem;

namespace Work.Code.Items
{
    [CreateAssetMenu(fileName = "ItemTree", menuName = "SO/Item/Tree", order = 0)]
    public class ItemTreeSO : ScriptableObject
    {
        public bool isImmediately;
        public List<EffectNodeSO> effectNodes;

        public void Execute(MatchSystem.MatchSystem ms, NodeData nodeData)
        {
            foreach (var node in effectNodes)
            {
                node.Execute(ms, nodeData);
            }
        }
    }
}