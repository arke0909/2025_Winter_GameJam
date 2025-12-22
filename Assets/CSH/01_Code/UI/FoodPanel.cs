using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum FoodType
{
    TomatoJuice = 0,
    Popcorn,
    CarrotSticks,
    BraisedRadish,
    GardenSalad,
    StirFriedRootVegetables,
    SweetPotatoPasta,
    CreamyVegetableStew,
    VegetableSkewers,
    SweetPotatoGnocchi,
    RainbowBibimbap,
    GrandHarvestFeast = 11
}

namespace CSH._01_Code.UI
{
    public class FoodPanel : MonoBehaviour
    {

        [SerializeField] private Transform content;
        //[SerializeField] private food
        private RectTransform rectTrm;
        private Vector2 originalPos;
        private bool isShow;
        private FoodInfo[] foodInfos;


        private void Awake()
        {
            rectTrm = GetComponent<RectTransform>();
            originalPos = rectTrm.anchoredPosition;
            isShow = false;
            foodInfos = content.GetComponentsInChildren<FoodInfo>();
        }

        private void OnDestroy()
        {
        }

        public void TogglePanel()
        {
            if (isShow == false)
            {
                DOTween.To(() => rectTrm.anchoredPosition, x => rectTrm.anchoredPosition = x,
                    new Vector2(originalPos.x - 500, originalPos.y), 0.5f).SetEase(Ease.OutCirc);
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