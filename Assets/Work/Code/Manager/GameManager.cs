using Lib.Dependencies;
using Lib.Utiles;
using UnityEngine;
using Work.Code.Events;
using Work.Code.Supply;

namespace Work.Code.Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private EventChannelSO supplyChannel;
        [field: SerializeField] private int requestGold { get; set; }
        
        [Inject] private UserSupplies _supplies;

        private void Start()
        {
            supplyChannel.AddListener<SetRequestGoldEvent>(HandleSetRequestGold);
            _supplies.OnSupplyChanged += HandleSupplyChange;
        }

        private void OnDestroy()
        {
            supplyChannel.RemoveListener<SetRequestGoldEvent>(HandleSetRequestGold);
            _supplies.OnSupplyChanged -= HandleSupplyChange;
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
                supplyChannel.InvokeEvent(SupplyEvents.GoldSuccessEvent);
            }
        }
    }
}