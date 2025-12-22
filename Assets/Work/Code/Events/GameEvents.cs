using Lib.Utiles;
using Work.Code.MatchSystem;

namespace Work.Code.Events
{
    public class GameEvents
    {
        public static GetIngredientEvent GetIngredientEvent = new GetIngredientEvent();
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
}