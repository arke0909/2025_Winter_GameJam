using System;
using CSH._01_Code.UI;
using Lib.Dependencies;
using Lib.Utiles;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Work.Code.Core;
using Work.Code.Events;
using Work.Code.Supply;

namespace Work.Code.Manager
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private EventChannelSO gameChannel;
        [SerializeField] private EventChannelSO supplyChannel;

        [SerializeField] private FoodPanel foodPanel;
        
        [field: SerializeField] private int requestGold { get; set; }
        
        [SerializeField] private TextMeshProUGUI turnText;
        [SerializeField] private int maxTurnCount;
        [field: SerializeField] private int leftTurnCount { get; set; }
        
        [Inject] private UserSupplies _supplies;

        private void Start()
        {
            supplyChannel.AddListener<SetRequestGoldEvent>(HandleSetRequestGold);
            gameChannel.AddListener<TurnAmountEvent>(HandleTurnAmount);
            _supplies.OnSupplyChanged += HandleSupplyChange;
        }

        private void OnDestroy()
        {
            supplyChannel.RemoveListener<SetRequestGoldEvent>(HandleSetRequestGold);
            gameChannel.RemoveListener<TurnAmountEvent>(HandleTurnAmount);
            _supplies.OnSupplyChanged -= HandleSupplyChange;
        }

        private void HandleTurnAmount(TurnAmountEvent evt)
        {
            leftTurnCount += evt.Value;
            turnText.SetText($"{leftTurnCount}/{maxTurnCount}");
            CheckGameOver();
        }

        public void CheckGameOver()
        {
            if (leftTurnCount <= 0)
            {
                if (_supplies.CanMakeAnyFood() || foodPanel.IsHaveAnyFood()) return;
                
                gameChannel.InvokeEvent(GameEvents.GameEndEvent.Initializer(SceneManager.GetActiveScene().name, false));
            }
        }
        
        private void HandleSetRequestGold(SetRequestGoldEvent evt)
        {
            requestGold = evt.RequestGold;
        }
        
        private void HandleSupplyChange(SupplyType supplyType, int amount)
        {
            if(supplyType != SupplyType.Gold) return;
            if (amount >= requestGold)
            {
                Debug.Log($"목표치 도달 {amount}");
                gameChannel.InvokeEvent(GameEvents.GameEndEvent.Initializer(SceneManager.GetActiveScene().name, true));
            }
        }
    }
}