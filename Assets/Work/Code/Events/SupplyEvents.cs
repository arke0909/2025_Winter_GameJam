using Lib.Utiles;
using Work.Code.Supply;

namespace Work.Code.Events
{
    public static class SupplyEvents
    {
        public static readonly SupplyEvent SupplyEvent = new SupplyEvent();
        public static readonly SetRequestGoldEvent SetRequestGoldEvent = new SetRequestGoldEvent();
        public static readonly GoldSuccessEvent GoldSuccessEvent = new GoldSuccessEvent();
    }
    
    /// <summary>
    /// 자원 추가, 소모 이벤트.
    /// -를 붙이면 소모, 아니라면 추가이다.
    /// </summary>
    public class SupplyEvent : GameEvent
    {
        public SupplyType SupplyType;
        public int Amount;

        public SupplyEvent Initializer(SupplyType supplyType, int amount)
        {
            SupplyType = supplyType;
            Amount = amount;
            return this;
        }
    }

    /// <summary>
    /// 목표 골드를 설정하는 이벤트
    /// </summary>
    public class SetRequestGoldEvent : GameEvent
    {
        public int RequestGold;

        public SetRequestGoldEvent Initializer(int requestGold)
        {
            RequestGold = requestGold;
            return this;
        }
    }
    
    /// <summary>
    /// 목표 골드에 도달한 이벤트
    /// </summary>
    public class GoldSuccessEvent : GameEvent
    {
        
    }
}