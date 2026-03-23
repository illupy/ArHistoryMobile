using UnityEngine;

public class MarkerLessonHandler : MonoBehaviour
{
    [SerializeField] private string markerCode = "BACH_DANG_001";

    public string GetMarkerCode()
    {
        return markerCode;
    }

    public void OnMarkerDetected()
    {
        Debug.Log("Detected marker: " + markerCode);
    }
}