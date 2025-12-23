using DG.Tweening;
using JetBrains.Annotations;
using Lib.Dependencies;
using Lib.ObjectPool.RunTime;
using Lib.Utiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Work.Code.Events;
using Work.Code.MatchSystem;
using Work.Code.SoundSystem;
using Work.Code.Supply;

namespace CSH._01_Code.UI
{
    public class CropsPanel : MonoBehaviour
    {
        [SerializeField] private PoolItemSO soundPlayer;
        [SerializeField] private SoundSO toggleUISound;
        [Inject] private PoolManagerMono poolManager;
        [SerializeField] private PoolItemSO movingCrop;
        [SerializeField] private EventChannelSO supplyChannel;
        [SerializeField] private Image[] cropImages;
        [SerializeField] private Transform movingCropsParent;
        private RectTransform rectTrm;
        private Vector2 originalPos;
        private bool isShow;
        
        private void Awake()
        {
            rectTrm = GetComponent<RectTransform>();
            originalPos = rectTrm.anchoredPosition;
            isShow = false;
            cropImages = GetComponentsInChildren<Image>().Where(img => img.gameObject != this.gameObject).ToArray();
            supplyChannel.AddListener<MatchSupplyEvent>(HandleMatchSupplyEvent);
        }

        private void OnDestroy()
        {
            supplyChannel.RemoveListener<MatchSupplyEvent>(HandleMatchSupplyEvent);
        }

        private void HandleMatchSupplyEvent(MatchSupplyEvent evt)
        {
            foreach (var a in evt.MatchedNodes)
            {
                if (a.NodeType > NodeType.SweetPotato) return;
                Transform target = cropImages[(int)a.NodeType].transform;
                var mc = poolManager.Pop<MovingImage>(movingCrop);
                mc.transform.SetParent(movingCropsParent);
                mc.SetImageAndMoveToTarget(cropImages[(int)a.NodeType].sprite, new Vector2(7.5f + (a.Pos.x * 100), -7.5f - (a.Pos.y * 100)), target);
            }
        }

        public void TogglePanel()
        {
            poolManager.Pop<SoundPlayer>(soundPlayer).PlaySound(toggleUISound);
            if (isShow == false)
            {
                DOTween.To(() => rectTrm.anchoredPosition, x => rectTrm.anchoredPosition = x,
                    new Vector2(originalPos.x + 500, originalPos.y), 0.5f).SetEase(Ease.OutCirc);
            }
            else
            {
                DOTween.To(() => rectTrm.anchoredPosition, x => rectTrm.anchoredPosition = x,
                    new Vector2(originalPos.x, originalPos.y), 0.5f).SetEase(Ease.OutCirc);
            }
            isShow = !isShow;
        }

       
    }
}
