using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeUIController : MonoBehaviour
{
    public void GoToARScan()
    {
        SceneManager.LoadScene("ARScanScene");
    }
}