using System;
using Lib.Dependencies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.Code.Manager;
using Work.Code.Supply;

namespace Work.Code.UI
{
    public class SupplyDataUI : MonoBehaviour
    {
        [SerializeField] private bool isFood = false;
        [SerializeField] private SupplyType supplyType;
        [SerializeField] private FoodType foodType;
        [SerializeField] private TextMeshProUGUI countText;

        [Inject] private SupplyAndFoodCounter _counter;

        private void Start()
        {
            if (isFood)
            {
                countText.SetText(_counter.GetFoodCount(foodType).ToString());
            }
            else
            {
                countText.SetText(_counter.GetSupplyCount(supplyType).ToString());
            }
        }

        public void SetUp()
        {
            if (isFood)
            {
                countText.SetText(_counter.GetFoodCount(foodType).ToString());
            }
            else
            {
                countText.SetText(_counter.GetSupplyCount(supplyType).ToString());
            }
        }
    }
}