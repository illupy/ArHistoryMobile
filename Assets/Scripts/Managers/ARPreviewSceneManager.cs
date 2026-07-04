using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

public class ARPreviewSceneManager : MonoBehaviour
{
    public GameObject scanGuidePanel;

    public PreviewPanelController previewPanelController;

    public GameObject lessonPanel;
    public LessonStepUIController lessonStepUIController;

    public GameObject completionPanel;
    public CompletionPanelController completionPanelController;

    [Header("AR Objects - ẩn khi không cần để giảm lag")]
    public GameObject arCameraObject;

    private LessonDetailResponse currentLesson;
    private int currentStepIndex = 0;

    public PreviewModelPresenter previewModelPresenter;
    
    private PreviewAudioPlayer cachedAudioPlayer;

    private void Start()
    {
        cachedAudioPlayer = Object.FindFirstObjectByType<PreviewAudioPlayer>();
        ShowWaitingState();
    }

    public void ShowWaitingState()
    {
        // Bật lại AR camera để quét marker
        if (arCameraObject != null) arCameraObject.SetActive(true);

        // Bật lại Vuforia camera
        if (VuforiaBehaviour.Instance != null)
        {
            VuforiaBehaviour.Instance.enabled = true;
        }

        // Reset các trigger và load state để có thể quét lại
        MarkerDetectedTrigger[] triggers = Object.FindObjectsByType<MarkerDetectedTrigger>(FindObjectsSortMode.None);
        foreach (var trigger in triggers)
        {
            trigger.ResetTrigger();
        }

        ARLessonLoader loader = Object.FindFirstObjectByType<ARLessonLoader>();
        if (loader != null)
        {
            loader.ResetLoadState();
        }

        if (scanGuidePanel != null) scanGuidePanel.SetActive(true);
        if (previewPanelController != null && previewPanelController.previewPanel != null)
            previewPanelController.previewPanel.SetActive(false);
        if (lessonPanel != null) lessonPanel.SetActive(false);
        if (completionPanel != null) completionPanel.SetActive(false);
    }

    public void OnLessonDetected(LessonDetailResponse lesson, Transform previewAnchor)
    {
        currentLesson = lesson;

        // Bật AR camera để hiển thị model preview
        if (arCameraObject != null) arCameraObject.SetActive(true);

        // Ẩn hết các panel khác
        if (scanGuidePanel != null) scanGuidePanel.SetActive(false);
        if (lessonPanel != null) lessonPanel.SetActive(false);
        if (completionPanel != null) completionPanel.SetActive(false);

        if (previewModelPresenter != null)
        {
            previewModelPresenter.ShowPreviewModel(lesson.previewModelCode, previewAnchor);
        }

        if (previewPanelController != null)
        {
            previewPanelController.ShowPreview(
                lesson.title,
                string.IsNullOrEmpty(lesson.description) ? "Khám phá bài học AR." : lesson.description,
                false
            );
        }
    }

    public void OnPreviewVoiceCompleted()
    {
        if (cachedAudioPlayer != null)
        {
            cachedAudioPlayer.StopPreviewAudio();
        }

        if (previewPanelController != null)
        {
            previewPanelController.EnableStartLesson();
        }
    }

    public void StartLesson()
    {
        if (cachedAudioPlayer != null)
        {
            cachedAudioPlayer.StopPreviewAudio();
        }

        if (currentLesson == null || currentLesson.assets == null || currentLesson.assets.Count == 0)
            return;

        currentStepIndex = 0;

        // Ẩn hết các panel khác để giảm lag
        if (scanGuidePanel != null) scanGuidePanel.SetActive(false);
        if (previewPanelController != null) previewPanelController.HidePreview();
        if (completionPanel != null) completionPanel.SetActive(false);

        // Ẩn model 3D và AR camera phía sau
        if (previewModelPresenter != null) previewModelPresenter.ClearPreview();
        if (arCameraObject != null) arCameraObject.SetActive(false);

        // Tắt Vuforia camera để đỡ nóng máy tốn tài nguyên
        if (VuforiaBehaviour.Instance != null)
        {
            VuforiaBehaviour.Instance.enabled = false;
        }

        // Chỉ bật LessonPanel
        if (lessonPanel != null) lessonPanel.SetActive(true);

        ShowCurrentStep();
    }

    public void NextStep()
    {
        if (currentLesson == null || currentLesson.assets == null || currentLesson.assets.Count == 0) return;

        if (currentStepIndex < currentLesson.assets.Count - 1)
        {
            currentStepIndex++;
            ShowCurrentStep();
        }
        else
        {
            ShowCompletion();
        }
    }

    public void PreviousStep()
    {
        if (currentLesson == null || currentLesson.assets == null || currentLesson.assets.Count == 0) return;

        if (currentStepIndex > 0)
        {
            currentStepIndex--;
            ShowCurrentStep();
        }
    }

    private void ShowCurrentStep()
    {
        if (currentLesson == null || currentLesson.assets == null || currentLesson.assets.Count == 0) return;

        var step = currentLesson.assets[currentStepIndex];

        if (lessonStepUIController != null)
        {
            lessonStepUIController.ShowStep(
                currentLesson,
                step,
                currentStepIndex,
                currentLesson.assets.Count
            );
        }
    }

    public void ShowCompletion()
    {
        // Ẩn hết các panel khác
        if (scanGuidePanel != null) scanGuidePanel.SetActive(false);
        if (previewPanelController != null) previewPanelController.HidePreview();
        if (lessonPanel != null) lessonPanel.SetActive(false);

        // Ẩn model 3D và AR camera phía sau
        if (previewModelPresenter != null) previewModelPresenter.ClearPreview();
        if (arCameraObject != null) arCameraObject.SetActive(false);

        // Tắt Vuforia camera khi ở màn hình hoàn thành
        if (VuforiaBehaviour.Instance != null)
        {
            VuforiaBehaviour.Instance.enabled = false;
        }

        // Chỉ bật CompletionPanel
        if (completionPanel != null) completionPanel.SetActive(true);

        bool hasQuiz = currentLesson != null && currentLesson.hasQuiz;
        bool hasGamification = currentLesson != null && currentLesson.hasGamification;

        if (completionPanelController != null)
        {
            completionPanelController.Setup(hasQuiz, hasGamification);
        }
    }

    public void GoHome()
    {
        SceneTransitionManager.Instance.LoadScene("HomeScene");
    }

    public void GoToQuiz()
    {
        SceneTransitionManager.Instance.LoadScene("QuizScene");
    }

    public void GoToGamification()
    {
        SceneTransitionManager.Instance.LoadScene("GamificationScene");
    }

    public void BackToScan()
    {
        if (cachedAudioPlayer != null)
        {
            cachedAudioPlayer.StopPreviewAudio();
        }

        currentStepIndex = 0;

        if (previewModelPresenter != null)
        {
            previewModelPresenter.ClearPreview();
        }

        ShowWaitingState();
    }

    public void ToggleDetachModel()
    {
        if (previewModelPresenter != null)
        {
            previewModelPresenter.ToggleDetachCurrentModel();
        }
    }

    public void ResetModelToMarker()
    {
        if (previewModelPresenter != null)
        {
            previewModelPresenter.ResetCurrentModel();
        }
    }
}