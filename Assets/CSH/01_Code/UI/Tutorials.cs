using UnityEngine;
namespace CSH._01_Code.UI
{
    public class Tutorials : MonoBehaviour
    {
        [SerializeField] private Sprite[] tutorialSprites;
        private bool isShowing = false;
        [SerializeField] private int idx = 0;

        private void Awake()
        {
            isShowing = false;
            idx = 0;
            ShowTutorial(idx);
        }

        public void Next()
        {
            ShowTutorial(Mathf.Clamp(++idx, 0, 7));
        }

        public void Previous()
        {
            ShowTutorial(Mathf.Clamp(--idx, 0, 7));
        }
        public void ShowTutorial(int index)
        {
            if (index <= 0 || index > tutorialSprites.Length)
            {
                idx = 0;

            }
            else
            {
                idx = index;
            }
                // Assuming there's a UI Image component to display the tutorial sprite
                var tutorialImage = GetComponent<UnityEngine.UI.Image>();
            if (tutorialImage != null)
            {
                tutorialImage.sprite = tutorialSprites[idx];
            }
            else
            {
                Debug.LogError("No Image component found on the GameObject.");
            }
        }

        public void TogglePanel()
        {
            if(isShowing)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
            isShowing = !isShowing;
        }

    }
}