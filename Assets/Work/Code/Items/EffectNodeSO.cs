using UnityEngine;
using Work.Code.MatchSystem;

namespace Work.Code.Items
{
    public abstract class EffectNodeSO : ScriptableObject
    {
        public abstract void Execute(MatchSystem.MatchSystem ms, NodeData data);
    }
}