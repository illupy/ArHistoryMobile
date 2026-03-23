using UnityEngine;
using Newtonsoft.Json;

public class ARLessonLoader : MonoBehaviour
{
    [SerializeField] private LessonApiService lessonApiService;
    [SerializeField] private LessonUIController lessonUIController;
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
                    lessonUIController.ShowLesson(
                        response.data.title,
                        response.data.description
                    );
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