using Lib.Dependencies;
using UnityEngine;
using Work.Code.Core;
using Work.Code.Items;

namespace Work.Code.Manager
{
    public class ItemManager : MonoSingleton<ItemManager>
    {
        private ItemTreeSO _itemTree;
        private FoodType _foodType;

        [Inject] private MatchSystem.MatchSystem _ms;
        
        public void SetData(FoodType foodType, ItemTreeSO itemTree)
        {
            _foodType = foodType;
            _itemTree = itemTree;
            
            _ms.EnterTargetingMode(_itemTree);
        }
    }
}