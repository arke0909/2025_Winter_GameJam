using System;
using Lib.Dependencies;
using Lib.ObjectPool.RunTime;
using Lib.Utiles;
using UnityEngine;
using UnityEngine.UI;
using Work.Code.Events;

namespace Work.Code.Effects
{
    public class ParticleSpawnManager : MonoBehaviour
    {
        [SerializeField] private EventChannelSO particleChannel;
        
        [Inject] private PoolManagerMono _poolManager;
        
        private void Awake()
        {
            particleChannel.AddListener<PlayUIParticleEvent>(HandlePlayParticleEvent);
        }

        private void OnDestroy()
        {
            particleChannel.RemoveListener<PlayUIParticleEvent>(HandlePlayParticleEvent);
        }

        private void HandlePlayParticleEvent(PlayUIParticleEvent evt)
        {
            var particle = _poolManager.Pop<PoolingEffect>(evt.ParticleItem);
            particle.transform.position = new Vector3(evt.Pos.x, evt.Pos.y, 0);
        }
    }
}