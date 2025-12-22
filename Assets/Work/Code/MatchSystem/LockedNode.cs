using UnityEngine;
using UnityEngine.EventSystems;

namespace Work.Code.MatchSystem
{
    public class LockedNode : Node
    {
        [SerializeField] private int cnt = 3;

        public override void OnDrag(PointerEventData eventData)
        {
            
        }

        public bool DiscountCnt()
        {
            cnt--;

            return cnt == 0;
        }
    }
}