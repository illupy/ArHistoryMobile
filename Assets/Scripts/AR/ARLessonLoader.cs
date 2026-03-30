using UnityEngine;
using Newtonsoft.Json;

public class ARLessonLoader : MonoBehaviour
{
    [SerializeField] private LessonApiService lessonApiService;
    [SerializeField] private ARSceneManager arSceneManager;
    [SerializeField] private string markerCode = "BACH_DANG_001";

    private bool hasLoaded = false;

    public void LoadLesson()
    {
        if (hasLoaded) return;
        hasLoaded = true;

        StartCoroutine(lessonApiService.GetLessonByMarker(
            markerCode,
            json =>
            {
                Debug.Log("Lesson response: " + json);

                ApiResponse<LessonDetailResponse> response =
                    JsonConvert.DeserializeObject<ApiResponse<LessonDetailResponse>>(json);

                if (response != null && response.success && response.data != null)
                {
                    arSceneManager.OnLessonLoaded(response.data);
                }
            },
            error =>
            {
                Debug.LogError("API Error: " + error);
                hasLoaded = false;
            }
        ));
    }
}