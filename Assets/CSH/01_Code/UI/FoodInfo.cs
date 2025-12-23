using CSH._01_Code.Events;
using Lib.Utiles;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.Code.Events;
using Work.Code.Food;
using Work.Code.Items;
using Work.Code.Manager;
using Work.Code.Supply;

namespace CSH._01_Code.UI
{
    public class FoodInfo : MonoBehaviour
    {
        [SerializeField] private EventChannelSO foodChannel;
        [SerializeField] private EventChannelSO supplyChannel;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameAndCountText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Button Sell;
        [SerializeField] private Button Use;
        private ItemTreeSO _itemTree;
        private FoodType _foodType;
        private int _count;
        private string _foodName;
        private int _price;

        public int Count => _count;
        
        public void Initialize(FoodDataSO data)
        {
            _itemTree = data.itemTree;
            _foodType = data.Type;
            _count = 0;
            name = data.Type.ToString();
            _foodName = data.Name;
            _price = data.Price;
            nameAndCountText.text = $"{_foodName} : {_count}개";
            descriptionText.text = data.Description;
            icon.sprite = data.Icon;
            priceText.text = $"{data.Price}G";

            Sell.onClick.AddListener(OnClickSell);
            Use.onClick.AddListener(OnClickUse);
#if !DEBUG
            gameObject.SetActive(false);
#endif
        }

        private void OnClickUse()
        {
            foodChannel.InvokeEvent(FoodEvents.FoodDecreaseEvent.Initializer(_foodType));
            ItemManager.Instance.SetData(_foodType, _itemTree);
        }

        private void OnClickSell()
        {
            foodChannel.InvokeEvent(FoodEvents.FoodDecreaseEvent.Initializer(_foodType));
            supplyChannel.InvokeEvent(SupplyEvents.SupplyEvent.Initializer(SupplyType.Gold, _price));
            GameManager.Instance.CheckGameOver();
        }

        public void AddFoodCount()
        {
            nameAndCountText.text = $"{_foodName} : {++_count}개";
            if (_count > 0 && !isActiveAndEnabled)
            {
                gameObject.SetActive(true);
            }
        }

        public void MinusFoodCount()
        {
            if (_count <= 0) return;
            nameAndCountText.text = $"{_foodName} : {--_count}개";
            if (_count <= 0 && isActiveAndEnabled)
            {
                gameObject.SetActive(false);
            }
        }


    }
}
