using System;
using Lib.ObjectPool.RunTime;
using Lib.Utiles;
using UnityEngine;
using UnityEngine.SceneManagement;
using Work.Code.SoundSystem;

namespace CSH._01_Code.UI
{
    public class TitleManager : MonoBehaviour
    {
        [SerializeField] private PoolManagerMono poolManager;
        [SerializeField] private PoolItemSO soundPlayer;
        [SerializeField] private SoundSO titleBGM;

        private void Start()
        {
            poolManager.Pop<SoundPlayer>(soundPlayer).PlaySound(titleBGM);
        }
    }
}