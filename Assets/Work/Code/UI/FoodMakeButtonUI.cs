using System;
using System.Text;
using CSH._01_Code.Events;
using Lib.Dependencies;
using Lib.ObjectPool.RunTime;
using Lib.Utiles;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Work.Code.Events;
using Work.Code.Food;
using Work.Code.SoundSystem;
using Work.Code.Supply;

namespace Work.Code.UI
{
    public class FoodMakeButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private EventChannelSO supplyChannel;
        [SerializeField] private EventChannelSO foodChannel;
        [SerializeField] private PoolItemSO soundPlayer;
        [SerializeField] private SoundSO cookingSound;
        [Inject] private PoolManagerMono poolManager;
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        [SerializeField] private Tooltip tooltip;
        [SerializeField] private Color disabledColor;
        [SerializeField] private FoodDataSO foodData;
        
        [Inject] private UserSupplies _userSupplies; 
        
        private static readonly string FOOD_FORMAT = "<color=#00FFAC>{0}</color> {1}.";
        private static readonly string FOOD_FORMAT_COMMA = "<color=#00FFAC>{0}</color> {1}, ";
        
        private void Awake()
        {
            _userSupplies.OnSupplyChanged += HandlesSupplyChange;
        }

        private void OnDestroy()
        {
            _userSupplies.OnSupplyChanged -= HandlesSupplyChange;
        }

        private void HandlesSupplyChange(SupplyType supplyType, int amount)
        {
            EnableFor();
        }

        public void Start()
        {
            string tooltipTxt = GetTooltipText();
            tooltip.SetText(tooltipTxt);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(tooltip != null )
                Invoke(nameof(ShowTooltip), tooltip.HoverDelay);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CancelInvoke();
            tooltip?.Hide();
        }

        public void EnableFor()
        {
            bool isEnoughSupplies = _userSupplies.HasEnoughSupplies(foodData.cost); // 만들 수 있는가?
            button.interactable = isEnoughSupplies;
            icon.color = isEnoughSupplies ? Color.white : disabledColor;
        }
        
        private void ShowTooltip()
        {
            tooltip.Show();
        }
        
        private string GetTooltipText()
        {
            StringBuilder tooltipBuilder = new StringBuilder();
            tooltipBuilder.Append($"{foodData.Name}\n");
        
            SupplyCostSO cost = foodData.cost;
            
        
            if (cost != null)
            {
                for (int i = 0; i < cost.CostSupplies.Count - 1; i++)
                {
                    tooltipBuilder.Append(string.Format(FOOD_FORMAT_COMMA, cost.CostSupplies[i].amount, cost.CostSupplies[i].type.ToString()));
                    if((i + 1) % 2 == 0) tooltipBuilder.AppendLine();
                }

                tooltipBuilder.Append(string.Format(FOOD_FORMAT,
                    cost.CostSupplies[^1].amount, cost.CostSupplies[^1].type.ToString()));
            }
            
            tooltipBuilder.AppendLine();
            tooltipBuilder.Append(foodData.Description);
            
            return tooltipBuilder.ToString();
        }

        public void MakeFood()
        {
            if(!_userSupplies.HasEnoughSupplies(foodData.cost))
                return; // 혹시 모르니깐
            var sound = poolManager.Pop<SoundPlayer>(soundPlayer);
            sound.PlaySound(cookingSound);
            foreach (var supply in foodData.cost.CostSupplies)
            {
                supplyChannel.InvokeEvent
                    (SupplyEvents.SupplyEvent.Initializer(supply.type, - supply.amount)); // 재료들 지우기
            }
            foodChannel.InvokeEvent(FoodEvents.FoodIncreaseEvent.Initializer(foodData.Type));
        }
    }
}