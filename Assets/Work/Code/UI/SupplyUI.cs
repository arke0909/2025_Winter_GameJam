using System;
using System.Collections.Generic;
using Lib.Dependencies;
using TMPro;
using UnityEngine;
using Work.Code.Supply;

namespace Work.Code.UI
{
    [Serializable]
    public struct SupplyText
    {
        public SupplyType type;
        public TextMeshProUGUI text;
    }
    public class SupplyUI : MonoBehaviour
    {
        [SerializeField] private List<SupplyText> supplyTextSetting;
        
        [Inject] private UserSupplies _userSupplies;

        private Dictionary<SupplyType, TextMeshProUGUI> _supplyTexts;

        private void Awake()
        {
            _supplyTexts = new Dictionary<SupplyType, TextMeshProUGUI>();
            foreach (var supplyText in supplyTextSetting)
            {
                _supplyTexts.Add(supplyText.type, supplyText.text);
            }
        }
        
        private void Start()
        {
            _userSupplies.OnSupplyChanged += HandleSupplyChange;
        }

        private void HandleSupplyChange(SupplyType supplyType, int value)
        {
            _supplyTexts[supplyType].SetText(value.ToString());
        }
    }
}