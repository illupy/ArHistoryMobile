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
                Debug.Log("Lesson response: " + json);

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

                    // Tạm thời lấy audio preview từ description hoặc field mock
                    // Nếu backend chưa có previewAudioUrl, bạn có thể mock sau.
                    string previewAudioUrl = ExtractPreviewAudioUrl(lesson);

                    previewAudioPlayer.PlayPreviewAudio(previewAudioUrl);
                }
                else
                {
                    Debug.LogError("Không parse được lesson hoặc response không hợp lệ.");
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

    public void ResetLoadState()
    {
        hasLoaded = false;
    }

    private string ExtractPreviewAudioUrl(LessonDetailResponse lesson)
    {
        // Giai đoạn đầu: có thể tạm hard-code hoặc lấy từ asset đầu tiên kiểu AUDIO
        if (lesson != null && lesson.assets != null)
        {
            foreach (var asset in lesson.assets)
            {
                if (asset.type == "AUDIO" && !string.IsNullOrEmpty(asset.fileUrl))
                {
                    return asset.fileUrl;
                }
            }
        }

        return null;
    }
}