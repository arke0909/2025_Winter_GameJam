using System;
using System.Collections.Generic;
using Lib.Dependencies;
using Lib.Utiles;
using UnityEngine;
using Work.Code.Events;
using Work.Code.Manager;

namespace Work.Code.Supply
{
    public enum SupplyType
    {
        Tomato = 0,
        Carrot = 1,
        Corn = 2,
        Paprika = 3,
        Radish = 4,
        SweetPotato = 5,
        Gold = 6
    }
    
    [Provide]
    public class UserSupplies : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private EventChannelSO supplyChannelSO;
        [SerializeField] private List<SupplyCostSO> allCosts;
        
        public delegate void SupplyChanged(SupplyType supplyType, int amount);
        public event SupplyChanged OnSupplyChanged;

        private Dictionary<SupplyType, int> _suppliesAmount; // 실제 재료 양
        
        private void Awake()
        {
            _suppliesAmount = new Dictionary<SupplyType, int>();
            foreach (SupplyType type in Enum.GetValues(typeof(SupplyType)))
            {
                _suppliesAmount.Add(type, 0);
            }
            
            supplyChannelSO.AddListener<SupplyEvent>(HandleSupplyEvent);
        }
        
        private void OnDestroy()
        {
            supplyChannelSO.RemoveListener<SupplyEvent>(HandleSupplyEvent);
        }

        /// <summary>
        /// - 를 붙이면 소모, 붙이지 않으면 증가
        /// </summary>
        private void HandleSupplyEvent(SupplyEvent evt)
        {
            _suppliesAmount[evt.SupplyType] += evt.Amount;
            OnSupplyChanged?.Invoke(evt.SupplyType, _suppliesAmount[evt.SupplyType]);
        }
        
        // SupplyCostSO이 요구하는 자원들이 충분한가
        public bool HasEnoughSupplies(SupplyCostSO cost)
        {
            foreach (var supply in cost.CostSupplies)
            {
                if (_suppliesAmount[supply.type] < supply.amount) return false;
            }
            return true;
        }

        public bool CanMakeAnyFood()
        {
            foreach (var cost in allCosts)
            {
                if(HasEnoughSupplies(cost)) return true;
            }
            
            return false;
        }
    }
}