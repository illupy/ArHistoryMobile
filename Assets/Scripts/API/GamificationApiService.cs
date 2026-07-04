using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GamificationApiService : MonoBehaviour
{
    public IEnumerator GetScenarioByLessonId(long lessonId, System.Action<string> onSuccess, System.Action<string> onError)
    {
        string url = $"{AppConfig.ApiBaseUrl}/gamification/lesson/{lessonId}";

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