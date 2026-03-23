using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LessonApiService : MonoBehaviour
{
    public string baseUrl = "http://10.0.2.2:8080/api"; 
    // đổi lại theo IP máy backend của bạn nếu test bằng điện thoại thật

    public IEnumerator GetLessonByMarker(string markerCode, System.Action<string> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/lessons/by-marker/{markerCode}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
}