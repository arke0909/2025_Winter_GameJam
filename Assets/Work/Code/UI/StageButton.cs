using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Work.Code.UI
{
    public class StageButton : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Color disabledColor;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void ToggleButtonClick(bool isOn)
        {
            _button.interactable = isOn;
            icon.color = isOn ? Color.white : disabledColor;
        }
        
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}