using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LessonApiService : MonoBehaviour
{

    public IEnumerator GetLessonByMarker(string markerCode, System.Action<string> onSuccess, System.Action<string> onError)
    {
        string url = $"{AppConfig.ApiBaseUrl}/lessons/by-marker/{markerCode}";

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