using CSH._01_Code.Events;
using Lib.Utiles;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.Code.Food;

namespace CSH._01_Code.UI
{
    public class FoodInfo : MonoBehaviour
    {
        [SerializeField] private EventChannelSO foodChannel;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameAndCountText;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Button Sell;
        [SerializeField] private Button Use;
        private FoodType _foodType;
        private int _count;
        private string _foodName;

        public void Initialize(FoodDataSO data)
        {
            _foodType = data.Type;
            name = data.Type.ToString();
            _foodName = data.Name;
            descriptionText.text = data.Description;
            icon.sprite = data.Icon;
            priceText.text = $"{data.Price}G";

            Sell.onClick.AddListener(OnClickSell);
            Use.onClick.AddListener(OnClickUse);
        }

        private void OnClickUse()
        {

        }

        private void OnClickSell()
        {
            foodChannel.InvokeEvent(FoodEvents.FoodDecreaseEvent.Initializer(_foodType));

        }

        public void AddFoodCount()
        {
            nameAndCountText.text = $"{_foodName} : {++_count}°³";
            if (_count > 0)
            {
                gameObject.SetActive(true);
            }
        }

        public void MinusFoodCount()
        {
            if (_count <= 0) return;
            countText.text = $"{--_count}°³";
            if (_count <= 0)
            {
                gameObject.SetActive(false);
            }
        }


    }
}
