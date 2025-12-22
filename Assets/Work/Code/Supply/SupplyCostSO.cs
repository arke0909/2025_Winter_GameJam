using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Work.Code.Supply
{
    [Serializable]
    public struct Supply
    {
        public SupplyType type;
        public int amount;
    }
    
    [CreateAssetMenu(fileName = "SupplyCost", menuName = "SO/Supply/Cost", order = 0)]
    public class SupplyCostSO : ScriptableObject
    {
        [field: SerializeField] public List<Supply> CostSupplies { get; private set; }
    }
}