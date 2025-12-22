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
    
    /// <summary>
    /// 요구 자원 뭐있는지, 얼마있는지
    /// </summary>
    [CreateAssetMenu(fileName = "SupplyCost", menuName = "SO/Supply/Cost", order = 0)]
    public class SupplyCostSO : ScriptableObject
    {
        // 필요한 자원들 목록
        [field: SerializeField] public List<Supply> CostSupplies { get; private set; }
    }
}