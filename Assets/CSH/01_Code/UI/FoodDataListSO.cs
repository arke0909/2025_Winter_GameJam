using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using Work.Code.Supply;

[Serializable]
public struct FoodData
{
    public FoodType Type;
    public string Name;
    [TextArea]
    public string Description;
    public Supply[] Ingredients; // 재료 목록
    public int Price;
    public int Count;
}

namespace CSH._01_Code.UI
{
    [CreateAssetMenu(fileName = "FoodDataList", menuName = "SO/FoodDataList")]
    public class FoodDataListSO : ScriptableObject
    {
        public FoodData[] FoodDatas;
        private void Awake()
        {
            
        }
    }
}
