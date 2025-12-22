using Lib.Utiles;
using UnityEngine;
using Work.Code.Events;

namespace CSH._01_Code.Events
{
    public static class FoodEvents
    {
        public static readonly FoodIncreasEvent FoodIncreaseEvent = new FoodIncreasEvent();
        public static readonly FoodDecreasEvent FoodDecreaseEvent = new FoodDecreasEvent();
    }

    /// <summary>
    /// 음식 추가이벤트.
    /// </summary>
    public class FoodIncreasEvent : GameEvent
    {
        public FoodType FoodType;
        int Amount;
        public FoodIncreasEvent Initializer(FoodType type)
        {
            FoodType = type;
            Amount = 1;
            return this;
        }
    }

    public class FoodDecreasEvent : GameEvent
    {
        public FoodType FoodType;
        int Amount;
        public FoodDecreasEvent Initializer(FoodType type)
        {
            FoodType = type;
            Amount = -1;
            return this;
        }
    }
}

