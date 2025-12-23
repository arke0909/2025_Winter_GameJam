using System;
using CSH._01_Code.UI;
using Lib.Dependencies;
using Lib.ObjectPool.RunTime;
using Lib.Utiles;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Work.Code.Core;
using Work.Code.Events;
using Work.Code.SoundSystem;
using Work.Code.Supply;

namespace Work.Code.Manager
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [Inject] private PoolManagerMono poolManager;
        [SerializeField] private PoolItemSO soundPlayer;
        [SerializeField] private SoundSO gameTheme;
        [SerializeField] private SoundSO coinSound;
        [SerializeField] private EventChannelSO gameChannel;
        [SerializeField] private EventChannelSO supplyChannel;

        [SerializeField] private FoodPanel foodPanel;
        
        [field: SerializeField] private int requestGold { get; set; }
        
        [SerializeField] private TextMeshProUGUI turnText;
        [SerializeField] private int maxTurnCount;
        [field: SerializeField] public int LeftTurnCount { get; private set; }
        
        [Inject] private UserSupplies _supplies;

        private void Start()
        {
            poolManager.Pop<SoundPlayer>(soundPlayer).PlaySound(gameTheme);
            supplyChannel.AddListener<SetRequestGoldEvent>(HandleSetRequestGold);
            gameChannel.AddListener<TurnAmountEvent>(HandleTurnAmount);
            _supplies.OnSupplyChanged += HandleSupplyChange;
            LeftTurnCount = maxTurnCount;
            turnText.SetText($"{LeftTurnCount}/{maxTurnCount}");
        }

        private void OnDestroy()
        {
            supplyChannel.RemoveListener<SetRequestGoldEvent>(HandleSetRequestGold);
            gameChannel.RemoveListener<TurnAmountEvent>(HandleTurnAmount);
            _supplies.OnSupplyChanged -= HandleSupplyChange;
        }

        private void HandleTurnAmount(TurnAmountEvent evt)
        {
            LeftTurnCount += evt.Value;
            turnText.SetText($"{LeftTurnCount}/{maxTurnCount}");
            CheckGameOver();
        }

        public void CheckGameOver()
        {
            if (LeftTurnCount <= 0)
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
            poolManager.Pop<SoundPlayer>(soundPlayer).PlaySound(coinSound);

            if (amount >= requestGold)
            {
                Debug.Log($"목표치 도달 {amount}");
                gameChannel.InvokeEvent(GameEvents.GameEndEvent.Initializer(SceneManager.GetActiveScene().name, true));
            }
        }

        
    }
}