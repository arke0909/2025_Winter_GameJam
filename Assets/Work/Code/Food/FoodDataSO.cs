using UnityEngine;
using Work.Code.Items;
using Work.Code.Supply;

namespace Work.Code.Food
{
    [CreateAssetMenu(fileName = "FoodData", menuName = "SO/FoodData", order = 0)]
    public class FoodDataSO : ScriptableObject
    {
        public ItemTreeSO itemTree;
        public FoodType Type;
        public Sprite Icon;
        public string Name;
        [TextArea]
        public string Description;
        public int Price;
        
        public SupplyCostSO cost;
    }
}