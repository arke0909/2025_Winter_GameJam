using System;
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
        private bool _isShow;
        
        private void Awake()
        {
            _rectTrm = GetComponent<RectTransform>();
            _originalPos = Vector2.zero;
            _rectTrm.anchoredPosition = new Vector2(_originalPos.x, _originalPos.y - 800);
            _isShow = false;
            
            gameChannel.AddListener<GameEndEvent>(HandleGameEnd);
        }

        private void OnDestroy()
        {
            gameChannel.RemoveListener<GameEndEvent>(HandleGameEnd);
        }

        private void HandleGameEnd(GameEndEvent evt)
        {
            
        }
    }
}