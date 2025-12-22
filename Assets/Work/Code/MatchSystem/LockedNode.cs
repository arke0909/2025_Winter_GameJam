using UnityEngine;

namespace Work.Code.MatchSystem
{
    public class LockedNode : Node
    {
        [SerializeField] private int cnt = 3;

        public bool DiscountCnt()
        {
            cnt--;

            return cnt == 0;
        }
    }
}