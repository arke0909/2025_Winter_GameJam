using System;
using Lib.Dependencies;
using Lib.Utiles;
using UnityEngine;
using Work.Code.Events;
using Work.Code.Supply;

namespace Work.Code.Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private EventChannelSO gameChannel;
        [SerializeField] private EventChannelSO supplyChannel;
        [field: SerializeField] private int requestGold { get; set; }
        
        [field: SerializeField] private int leftTurnCount { get; set; }
        
        public event Action OnTurnZeroEvent;
        
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
            if(leftTurnCount <= 0)
                gameChannel.InvokeEvent(GameEvents.GameEndEvent.Initializer(false));
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
                gameChannel.InvokeEvent(GameEvents.GameEndEvent.Initializer(true));
            }
        }
    }
}