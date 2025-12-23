using System;
using System.Collections.Generic;
using Lib.ObjectPool.RunTime;
using UnityEngine;
using Work.Code.Manager;
using Work.Code.SoundSystem;

namespace Work.Code.UI
{
    public class StageButtonController : MonoBehaviour
    {
        [SerializeField] private List<StageButton> buttons;
        [SerializeField] private PoolManagerMono poolManager;
        [SerializeField] private PoolItemSO soundPlayer;
        [SerializeField] private SoundSO bgm;
        
        private void Start()
        {
            poolManager.Pop<SoundPlayer>(soundPlayer).PlaySound(bgm);

            var data = StageManager.Instance.GetStageClearData();
            int idx = 0;
            for (; idx < data.Count; idx++)
            {
                if (!data[idx].IsClear) // 이 스테이지가 클리어되지 않았다면
                {
                    Debug.Log(idx);
                    buttons[idx].ToggleButtonClick(true); // 그거까지 열어준다
                    break;
                }
                buttons[idx].ToggleButtonClick(true);
            }
            idx++;
            for (; idx < data.Count; idx++)
            {
                buttons[idx].ToggleButtonClick(false); // 이후 잠금
            }
        }
    }
}