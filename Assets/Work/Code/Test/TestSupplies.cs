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
        [ContextMenu("SetGold")]
        private void SetRequestGold()
        {
            supplyChannel.InvokeEvent(SupplyEvents.SetRequestGoldEvent.Initializer(gold));
        }
    }
}