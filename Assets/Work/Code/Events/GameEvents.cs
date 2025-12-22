using Lib.Utiles;
using Work.Code.MatchSystem;

namespace Work.Code.Events
{
    public class GameEvents
    {
        public static readonly GetIngredientEvent GetIngredientEvent = new GetIngredientEvent();
        public static readonly TurnAmountEvent TurnAmountEvent = new TurnAmountEvent();
        public static readonly GameEndEvent GameEndEvent = new GameEndEvent();
    }
    
    public class GetIngredientEvent : GameEvent
    {
        public GetIngredientData data;

        public GetIngredientEvent Init(GetIngredientData data)
        {
            this.data = data;
            return this;
        }
    }

    /// <summary>
    /// 마찬가지로 +면 증가, -면 감소
    /// </summary>
    public class TurnAmountEvent : GameEvent
    {
        public int Value;

        public TurnAmountEvent Initializer(int value)
        {
            Value = value;
            return this;
        }
    }

    public class GameEndEvent : GameEvent
    {
        public bool IsSuccess;

        public GameEndEvent Initializer(bool isSuccess)
        {
            IsSuccess = isSuccess;
            return this;
        }
    }
}