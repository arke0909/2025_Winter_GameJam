using UnityEngine;
using UnityEngine.SceneManagement;

namespace CSH._01_Code.UI
{
    public class TitleManager : MonoBehaviour
    {
        [SerializeField] private GameObject creditPanel;
        public void StartBtn()
        {
            SceneManager.LoadScene(1);
        }

        public void CreditBtn()
        {
            creditPanel.SetActive(true);
        }

        public void CreditCloseBtn()
        {
            creditPanel.SetActive(false);
        }

        public void ExitBtn()
        {
            Application.Quit();
        }
    }
}