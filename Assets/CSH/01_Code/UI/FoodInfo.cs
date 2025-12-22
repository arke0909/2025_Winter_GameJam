using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CSH._01_Code.UI
{
    public class FoodInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Button Sell;
        [SerializeField] private Button Use;
        

        public void SetFoodIInfo(int count)
        {
            countText.text = $"{count}";
        }

        public void AddFoodCount(int v)
        {
            countText.text = $"{int.Parse(countText.text)}";
        }

    }
}
