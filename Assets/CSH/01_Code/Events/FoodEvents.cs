using Lib.Utiles;
using UnityEngine;
using Work.Code.Events;

namespace CSH._01_Code.Events
{
    public static class FoodEvents
    {
        public static readonly FoodIncreasEvent FoodIncreaseEvent = new FoodIncreasEvent();
        public static readonly FoodDecreasEvent FoodDecreaseEvent = new FoodDecreasEvent();
        public static readonly FoodMovingEvent FoodMovingEvent = new FoodMovingEvent();
    }

    /// <summary>
    /// 음식 추가이벤트.
    /// </summary>
    public class FoodIncreasEvent : GameEvent
    {
        public FoodType FoodType;
        public FoodIncreasEvent Initializer(FoodType type)
        {
            FoodType = type;
            return this;
        }
    }

    /// <summary>
    /// 음식 추가시 날아가는 이벤트.
    /// </summary>
    public class FoodMovingEvent : GameEvent
    {
        public FoodType FoodType;
        public Transform Start;
        public FoodMovingEvent Initializer(FoodType type, Transform start)
        {
            Start = start;
            FoodType = type;
            return this;
        }
    }

    public class FoodDecreasEvent : GameEvent
    {
        public FoodType FoodType;
        public FoodDecreasEvent Initializer(FoodType type)
        {
            FoodType = type;
            return this;
        }
    }
}

