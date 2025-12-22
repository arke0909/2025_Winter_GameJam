using System;
using Lib.Dependencies;
using Lib.ObjectPool.RunTime;
using Lib.Utiles;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Work.Code.Events;
using Work.Code.Manager;
using Work.Code.Supply;

namespace Work.Code.Test
{
    public class TestSupplies : MonoBehaviour
    {
        [SerializeField] private EventChannelSO supplyChannel;
        [SerializeField] private EventChannelSO effectChannel;
        [SerializeField] private int gold;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private PoolItemSO particle;
        [SerializeField] private Image someImage;
        
        
        
        private void Update()
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
                supplyChannel.InvokeEvent(SupplyEvents.SupplyEvent.Initializer(SupplyType.Gold, 10));
            if (Keyboard.current.wKey.wasPressedThisFrame)
                supplyChannel.InvokeEvent(SupplyEvents.SupplyEvent.Initializer(SupplyType.Carrot, 1));
            if (Keyboard.current.zKey.wasPressedThisFrame)
                effectChannel.InvokeEvent(ParticleEvents.PlayUIParticleEvent.Initializer(particle, someImage.transform.position));
        }

        [ContextMenu("SetGold")]
        private void SetRequestGold()
        {
            supplyChannel.InvokeEvent(SupplyEvents.SetRequestGoldEvent.Initializer(gold));
        }
    }
}