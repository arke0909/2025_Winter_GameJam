using System;
using System.Collections.Generic;
using Lib.Dependencies;
using Lib.Utiles;
using UnityEngine;

namespace Work.Code.Supply
{
    public enum SupplyType
    {
        Gold,
        Tomato,
        Carrot,
        Corn,
        Paprika,
        Radish,
        SweetPotato
    }
    
    [Provide]
    public class UserSupplies : MonoBehaviour
    {
        [SerializeField] private EventChannelSO supplyChannelSO;
        
        public delegate void SupplyChanged(SupplyType supplyType, int amount);
        public event SupplyChanged OnSupplyChanged;

        private Dictionary<SupplyType, int> _suppliesCount;
        
        private void Awake()
        {
            _suppliesCount = new Dictionary<SupplyType, int>();
            foreach (SupplyType type in Enum.GetValues(typeof(SupplyType)))
            {
                _suppliesCount.Add(type, 0);
            }
            
            supplyChannelSO.AddListener<SupplyEvent>(HandleSupplyEvent);
        }
        
        private void OnDestroy()
        {
            supplyChannelSO.AddListener<SupplyEvent>(HandleSupplyEvent);

        }

        /// <summary>
        /// - 를 부이면 소모, 붙이지 않으면 증가
        /// </summary>
        private void HandleSupplyEvent(SupplyEvent evt)
        {
            _suppliesCount[evt.SupplyType] += evt.Amount;
            OnSupplyChanged?.Invoke(evt.SupplyType, _suppliesCount[evt.SupplyType]);
        }
        
        // SupplyCostSO이 요구하는 자원들이 충분한가
        public bool HasEnoughSupplies(SupplyCostSO cost)
        {
            foreach (var supply in cost.CostSupplies)
            {
                if (_suppliesCount[supply.type] < supply.amount) return false;
            }
            return true;
        }
    }
}