using Lib.ObjectPool.RunTime;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Work.Code.SoundSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundPlayer : MonoBehaviour, IPoolable
    {
        [SerializeField] private AudioMixerGroup sfxGroup;
        [SerializeField] private AudioMixerGroup musicGroup;
        [SerializeField] private AudioSource audioSource;
        public GameObject GameObject => gameObject;
        [field:SerializeField] public PoolItemSO PoolItem { get; private set; }
        private Pool _myPool;
        
        public void SetUpPool(Pool pool) => _myPool = pool;

        public void ResetItem()
        {
            //do nothing
        }

        public void PlaySound(SoundSO data)
        {

            if (data.audioType == SoundSO.AudioTypes.SFX)
            {
                audioSource.outputAudioMixerGroup = sfxGroup;
            }
            else if (data.audioType == SoundSO.AudioTypes.Music)
            {
                audioSource.outputAudioMixerGroup = musicGroup;
            }

            audioSource.volume = data.volume;
            audioSource.pitch = data.pitch;
            if (data.randomizePitch)
            {
                audioSource.pitch += Random.Range(-data.randomPitchModifier, data.randomPitchModifier);
            }
            audioSource.clip = data.clip;

            audioSource.loop = data.loop;

            if (!data.loop)
            {
                float time = audioSource.clip.length + .2f;
                DisableSoundTimer(time);
            }
            audioSource.Play();
        }

        private async void DisableSoundTimer(float time)
        {
            await Awaitable.WaitForSecondsAsync(time);
            _myPool.Push(this);
        }

        public void StopAndGoToPool()
        {
            audioSource.Stop();
            _myPool.Push(this);
        }

    }
}