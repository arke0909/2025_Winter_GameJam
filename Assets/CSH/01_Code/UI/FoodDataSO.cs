using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using Work.Code.Supply;
namespace CSH._01_Code.UI
{
    [CreateAssetMenu(fileName = "FoodData", menuName = "SO/FoodDataList")]
    public class FoodData : ScriptableObject
    {
        public FoodType Type;
        public Sprite Icon;
        public string Name;
        [TextArea]
        public string Description;
        public Supply[] Ingredients; // 재료 목록
        public int Price;
    }



}
