using System;
using DG.Tweening;
using Lib.Utiles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.Code.Events;

namespace Work.Code.UI
{
    public class GameEndUI : MonoBehaviour
    {
        [SerializeField] private EventChannelSO gameChannel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image backgroundImage;
        
        private RectTransform _rectTrm;
        private Vector2 _originalPos;
        private SupplyDataUI[] _supplyData;
        private void Awake()
        {
            _rectTrm = GetComponent<RectTransform>();
            _originalPos = Vector2.zero;
            _rectTrm.anchoredPosition = new Vector2(_originalPos.x, _originalPos.y - 2000);
            _supplyData = GetComponentsInChildren<SupplyDataUI>();
            backgroundImage.raycastTarget = false;
            
            gameChannel.AddListener<GameEndEvent>(HandleGameEnd);
        }

        private void OnDestroy()
        {
            gameChannel.RemoveListener<GameEndEvent>(HandleGameEnd);
        }

        private void HandleGameEnd(GameEndEvent evt)
        {
            backgroundImage.raycastTarget = true;
            titleText.SetText(evt.IsSuccess ? "성공" : "실패");
            foreach (var data in _supplyData)
            {
                data.SetUp();
            }
            DOTween.To(() => _rectTrm.anchoredPosition, x => _rectTrm.anchoredPosition = x,
                new Vector2(_originalPos.x, _originalPos.y), 0.5f).SetEase(Ease.OutCirc);
        }
    }
}