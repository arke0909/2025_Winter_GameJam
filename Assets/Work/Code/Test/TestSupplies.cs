using System;
using Lib.Dependencies;
using Lib.Utiles;
using UnityEngine;
using UnityEngine.InputSystem;
using Work.Code.Events;
using Work.Code.Manager;
using Work.Code.Supply;

namespace Work.Code.Test
{
    public class TestSupplies : MonoBehaviour
    {
        [SerializeField] private EventChannelSO supplyChannel;
        [SerializeField] private int gold;
        [SerializeField] private GameManager gameManager;
        
        
        
        private void Update()
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
                supplyChannel.InvokeEvent(SupplyEvents.SupplyEvent.Initializer(SupplyType.Gold, 10));
            if (Keyboard.current.wKey.wasPressedThisFrame)
                supplyChannel.InvokeEvent(SupplyEvents.SupplyEvent.Initializer(SupplyType.Carrot, 1));
        }

        [ContextMenu("SetGold")]
        private void SetRequestGold()
        {
            supplyChannel.InvokeEvent(SupplyEvents.SetRequestGoldEvent.Initializer(gold));
        }
    }
}