using Lib.ObjectPool.RunTime;
using UnityEngine;
using UnityEngine.Serialization;

namespace Work.Code.SoundSystem
{
    public class ClickSound : MonoBehaviour
    {
        [SerializeField] private PoolManagerMono poolManager;
        [SerializeField] private PoolItemSO soundPlayer;
        [SerializeField] private SoundSO clickSound;

        public void PlayClickSound()
        {
            poolManager.Pop<SoundPlayer>(soundPlayer).PlaySound(clickSound);
        }
    }
}