using UnityEngine;
using Vuforia;

public class MarkerDetectedTrigger : MonoBehaviour
{
    [SerializeField] private ARPreviewSceneManager arPreviewSceneManager;

    private ObserverBehaviour observerBehaviour;
    private bool triggered = false;

    private void Start()
    {
        Debug.Log("MarkerDetectedTrigger Start");
        observerBehaviour = GetComponent<ObserverBehaviour>();

        if (observerBehaviour != null)
        {
            Debug.Log("ObserverBehaviour found");
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }
        else
        {
            Debug.LogError("ObserverBehaviour NOT found");
        }
    }

    private void OnDestroy()
    {
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        Debug.Log("Target status: " + status.Status);

        if (triggered) return;

        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            Debug.Log("Marker detected, starting lesson flow");
            triggered = true;

            LessonDetailResponse mockLesson = new LessonDetailResponse
            {
                id = 1,
                title = "Trận Bạch Đằng",
                description = "Khám phá chiến thắng Bạch Đằng qua trải nghiệm AR.",
                assets = new System.Collections.Generic.List<LessonAssetItem>
                {
                    new LessonAssetItem { id = 1, type = "TEXT", content = "Bối cảnh lịch sử trước trận Bạch Đằng.", orderIndex = 1 },
                    new LessonAssetItem { id = 2, type = "TEXT", content = "Diễn biến chính của trận đánh trên sông.", orderIndex = 2 },
                    new LessonAssetItem { id = 3, type = "TEXT", content = "Kết quả và ý nghĩa lịch sử của chiến thắng.", orderIndex = 3 }
                }
            };

            if (arPreviewSceneManager == null)
            {
                Debug.LogError("ARPreviewSceneManager is NULL");
                return;
            }

            arPreviewSceneManager.OnLessonDetected(mockLesson);
            arPreviewSceneManager.OnPreviewVoiceCompleted();
        }
    }
}