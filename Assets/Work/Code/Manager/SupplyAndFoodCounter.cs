using System;
using System.Collections.Generic;
using CSH._01_Code.Events;
using Lib.Dependencies;
using Lib.Utiles;
using UnityEngine;
using Work.Code.Events;
using Work.Code.Supply;

namespace Work.Code.Manager
{
    [Provide]
    public class SupplyAndFoodCounter : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private EventChannelSO supplyChannel;
        [SerializeField] private EventChannelSO foodChannel;
        
        private Dictionary<SupplyType, int> _supplyCount = new Dictionary<SupplyType, int>();
        private Dictionary<FoodType, int> _foodType = new Dictionary<FoodType, int>();
        
        private void Awake()
        {
            foreach (SupplyType type in Enum.GetValues(typeof(SupplyType)))
            {
                _supplyCount.Add(type, 0);
            }
            foreach (FoodType type in Enum.GetValues(typeof(FoodType)))
            {
                _foodType.Add(type, 0);
            }
            supplyChannel.AddListener<SupplyEvent>(HandleSupply);
            foodChannel.AddListener<FoodIncreasEvent>(HandleFoodIncrease);
        }

        private void OnDestroy()
        {
            supplyChannel.RemoveListener<SupplyEvent>(HandleSupply);
            foodChannel.RemoveListener<FoodIncreasEvent>(HandleFoodIncrease);
        }

        private void HandleSupply(SupplyEvent evt)
        {
            if (evt.Amount > 0)
            {
                _supplyCount[evt.SupplyType] += evt.Amount;
            }
        }
        
        private void HandleFoodIncrease(FoodIncreasEvent evt)
        {
            _foodType[evt.FoodType]++;
        }

        public int GetSupplyCount(SupplyType type)
            => _supplyCount[type];

        public int GetFoodCount(FoodType type)
            => _foodType[type];
    }
}