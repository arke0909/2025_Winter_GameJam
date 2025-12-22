using DG.Tweening;
using UnityEngine;

namespace CSH._01_Code.UI
{
    public class CropsPanel : MonoBehaviour
    {
        private RectTransform rectTrm;
        private Vector2 originalPos;
        private bool isShow;
        private void Awake()
        {
            rectTrm = GetComponent<RectTransform>();
            originalPos = rectTrm.anchoredPosition;
            isShow = false;
        }


        public void TogglePanel()
        {
            if (isShow == false)
            {
                DOTween.To(() => rectTrm.anchoredPosition, x => rectTrm.anchoredPosition = x,
                    new Vector2(originalPos.x + 500, originalPos.y), 0.5f).SetEase(Ease.OutCirc);
            }
            else
            {
                DOTween.To(() => rectTrm.anchoredPosition, x => rectTrm.anchoredPosition = x,
                    new Vector2(originalPos.x, originalPos.y), 0.5f).SetEase(Ease.OutCirc);
            }
            isShow = !isShow;
        }


    }
}
