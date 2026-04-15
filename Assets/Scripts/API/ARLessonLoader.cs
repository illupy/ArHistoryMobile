using UnityEngine;
using Newtonsoft.Json;

public class ARLessonLoader : MonoBehaviour
{
    [SerializeField] private LessonApiService lessonApiService;
    [SerializeField] private ARPreviewSceneManager arPreviewSceneManager;
    [SerializeField] private PreviewAudioPlayer previewAudioPlayer;
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
                ApiResponse<LessonDetailResponse> response =
                    JsonConvert.DeserializeObject<ApiResponse<LessonDetailResponse>>(json);

                if (response != null && response.success && response.data != null)
                {
                    LessonDetailResponse lesson = response.data;

                    if (LessonSessionStore.Instance != null)
                    {
                        LessonSessionStore.Instance.SetLesson(lesson, markerCode);
                    }

                    arPreviewSceneManager.OnLessonDetected(lesson);

                    if (previewAudioPlayer != null)
                    {
                        previewAudioPlayer.PlayPreviewAudio(lesson.previewAudioUrl);
                    }
                    else
                    {
                        arPreviewSceneManager.OnPreviewVoiceCompleted();
                    }
                }
                else
                {
                    hasLoaded = false;
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