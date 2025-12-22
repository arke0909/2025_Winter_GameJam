using Lib.Utiles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CSH._01_Code.UI
{
    public class FoodInfo : MonoBehaviour
    {
        [SerializeField] private EventChannelSO supplyChannel;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Button Sell;
        [SerializeField] private Button Use;
        private FoodType foodType;
        private int _count;

        private void Awake()
        {
            _count = 0;
        }

        public void AddFoodCount()
        {

            countText.text = $"{++_count}°³";
        }

        public void MinusFoodCount()
        {
            if (_count <= 0) return;
            countText.text = $"{--_count}°³";
        }
    }
}
