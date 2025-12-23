using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lib.Dependencies;
using Lib.ObjectPool.RunTime;
using Lib.Utiles;
using UnityEngine;
using Work.Code.Events;
using Work.Code.SoundSystem;

namespace Work.Code.UI
{
    public class FoodMakeUI : MonoBehaviour
    {
        [Inject] private PoolManagerMono poolManager;
        [SerializeField] private PoolItemSO soundPlayer;
        [SerializeField] private SoundSO toggleUISound;
        private RectTransform _rectTrm;
        private Vector2 _originalPos;
        private bool _isShow;
        private List<FoodMakeButtonUI> _foodMakeButtons;
        
        private void Awake()
        {
            _rectTrm = GetComponent<RectTransform>();
            _originalPos = Vector2.zero;
            _rectTrm.anchoredPosition = new Vector2(_originalPos.x, _originalPos.y - 600);
            _isShow = false;
            _foodMakeButtons = GetComponentsInChildren<FoodMakeButtonUI>().ToList();
            
        }
        
        public void TogglePanel()
        {
            var sound = poolManager.Pop<SoundPlayer>(soundPlayer);
            sound.PlaySound(toggleUISound);
            if (!_isShow)
            {
                foreach (FoodMakeButtonUI btn in _foodMakeButtons)
                {
                    btn.EnableFor();
                }
                DOTween.To(() => _rectTrm.anchoredPosition, x => _rectTrm.anchoredPosition = x,
                    new Vector2(_originalPos.x, _originalPos.y), 0.5f).SetEase(Ease.OutCirc);
            }
            else
            {
                DOTween.To(() => _rectTrm.anchoredPosition, x => _rectTrm.anchoredPosition = x,
                    new Vector2(_originalPos.x, _originalPos.y - 600), 0.5f).SetEase(Ease.InCirc);
            }
            _isShow = !_isShow;
        }

    }
}