using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Work.Code.MatchSystem
{
    public class LockedNode : Node
    {
        [SerializeField] private Sprite[] lockedSprites;
        [SerializeField] private int cnt = 3;

        private Image _nodeImage;

        private void Awake()
        {
            _nodeImage = GetComponent<Image>();
        }

        public override void OnDrag(PointerEventData eventData)
        {
            
        }

        public bool DiscountCnt()
        {
            cnt--;
            bool isUnlock = cnt <= 0;
            
            if(!isUnlock)
                _nodeImage.sprite = lockedSprites[cnt - 1];
            
            return isUnlock;
        }
    }
}