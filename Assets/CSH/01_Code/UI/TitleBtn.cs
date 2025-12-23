using UnityEngine;

namespace CSH._01_Code.UI
{
    public class TitleBtn : MonoBehaviour
    {
        public void OnClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}