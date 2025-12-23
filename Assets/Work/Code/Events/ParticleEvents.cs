using Lib.ObjectPool.RunTime;
using Lib.Utiles;
using UnityEngine;

namespace Work.Code.Events
{
    public static class ParticleEvents
    {
        public static readonly PlayUIParticleEvent PlayUIParticleEvent = new PlayUIParticleEvent();
    }

    /// <summary>
    /// 위치는 그냥 transform.position 넣으면 됨
    /// </summary>
    public class PlayUIParticleEvent : GameEvent
    {
        public PoolItemSO ParticleItem;
        public Vector3 Pos;
        public Quaternion Rot;
        
        public PlayUIParticleEvent Initializer(PoolItemSO particleItem, Vector3 pos, Quaternion rot = default)
        {
            ParticleItem = particleItem;
            Pos = pos;
            Rot = rot == default ? Quaternion.identity : rot;
            return this;
        }
    }
}