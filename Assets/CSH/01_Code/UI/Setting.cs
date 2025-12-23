using DG.Tweening;
using Lib.Dependencies;
using Lib.ObjectPool.RunTime;
using UnityEngine;
using UnityEngine.Audio;
using Work.Code.SoundSystem;
using static UnityEngine.Rendering.DebugUI;

namespace CSH._01_Code.UI
{
    public class Setting : MonoBehaviour
    {
        [SerializeField] private PoolItemSO soundPlayer;
        [SerializeField] private SoundSO toggleUISound;
        [Inject] private PoolManagerMono poolManager;
        [SerializeField] private AudioMixer audioMixer;
        private bool isShow = false;
        private float minVolume = -80f;
        public void TogglePanel()
        {
            poolManager.Pop<SoundPlayer>(soundPlayer).PlaySound(toggleUISound);
            if (!isShow)
            {
                transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            }
            else
            {
                transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
            }
            isShow = !isShow;
        }

        public void SetMasterVolume(float v)
        {
            float dB = v <= 0.0001f ? -80f : Mathf.Log10(v) * 20f;

            audioMixer.SetFloat("MasterVolume", dB);
            
        }
        public void SetSFXVolume(float v)
        {
            float dB = v <= 0.0001f ? -80f : Mathf.Log10(v) * 20f;

            audioMixer.SetFloat("SFXVolume", dB);
        }
        public void SetBGMVolume(float v)
        {
            float dB = v <= 0.0001f ? -80f : Mathf.Log10(v) * 20f;

            audioMixer.SetFloat("BGMVolume", dB);
        }
    }
}