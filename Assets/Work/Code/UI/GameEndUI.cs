using System;
using DG.Tweening;
using Lib.Utiles;
using UnityEngine;
using Work.Code.Events;

namespace Work.Code.UI
{
    public class GameEndUI : MonoBehaviour
    {
        [SerializeField] private EventChannelSO gameChannel;
        
        private RectTransform _rectTrm;
        private Vector2 _originalPos;
        private SupplyDataUI[] _supplyData;
        private void Awake()
        {
            _rectTrm = GetComponent<RectTransform>();
            _originalPos = Vector2.zero;
            _rectTrm.anchoredPosition = new Vector2(_originalPos.x, _originalPos.y - 1000);
            _supplyData = GetComponentsInChildren<SupplyDataUI>();
            
            gameChannel.AddListener<GameEndEvent>(HandleGameEnd);
        }

        private void OnDestroy()
        {
            gameChannel.RemoveListener<GameEndEvent>(HandleGameEnd);
        }

        private void HandleGameEnd(GameEndEvent evt)
        {
            foreach (var data in _supplyData)
            {
                data.SetUp();
            }
            DOTween.To(() => _rectTrm.anchoredPosition, x => _rectTrm.anchoredPosition = x,
                new Vector2(_originalPos.x, _originalPos.y), 0.5f).SetEase(Ease.OutCirc);
        }
    }
}