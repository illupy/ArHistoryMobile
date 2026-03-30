using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeUIController : MonoBehaviour
{
    public GameObject guidePanel;

    public void GoToARScan()
    {
        SceneManager.LoadScene("ARScanScene");
    }

    public void OpenGuide()
    {
        if (guidePanel != null)
            guidePanel.SetActive(true);
    }

    public void CloseGuide()
    {
        if (guidePanel != null)
            guidePanel.SetActive(false);
    }
}