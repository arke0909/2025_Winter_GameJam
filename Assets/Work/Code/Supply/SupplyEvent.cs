using Lib.Utiles;

namespace Work.Code.Supply
{
    public class SupplyEvent : GameEvent
    {
        public SupplyType SupplyType { get; }
        public int Amount { get; }

        public SupplyEvent(SupplyType supplyType, int amount)
        {
            SupplyType = supplyType;
            Amount = amount;
        }
    }
}