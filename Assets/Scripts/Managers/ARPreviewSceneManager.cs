using UnityEngine;
using UnityEngine.SceneManagement;

public class ARPreviewSceneManager : MonoBehaviour
{
    public GameObject scanGuidePanel;

    public PreviewPanelController previewPanelController;

    public GameObject lessonPanel;
    public LessonUIController lessonUIController;

    public GameObject completionPanel;
    public CompletionPanelController completionPanelController;

    private LessonDetailResponse currentLesson;
    private int currentStepIndex = 0;

    private void Start()
    {
        ShowWaitingState();
    }

    public void ShowWaitingState()
    {
        if (scanGuidePanel != null) scanGuidePanel.SetActive(true);
        if (previewPanelController != null && previewPanelController.previewPanel != null)
            previewPanelController.previewPanel.SetActive(false);
        if (lessonPanel != null) lessonPanel.SetActive(false);
        if (completionPanel != null) completionPanel.SetActive(false);
    }

    public void OnLessonDetected(LessonDetailResponse lesson)
    {
        currentLesson = lesson;

        if (scanGuidePanel != null) scanGuidePanel.SetActive(false);

        if (previewPanelController != null)
        {
            // Bật luôn nút Start để test flow trước
            previewPanelController.ShowPreview(
                lesson.title,
                string.IsNullOrEmpty(lesson.description) ? "Khám phá bài học AR." : lesson.description,
                true
            );
        }
    }

    public void OnPreviewVoiceCompleted()
    {
        if (previewPanelController != null)
        {
            previewPanelController.EnableStartLesson();
        }
    }

    public void StartLesson()
    {
        if (currentLesson == null || currentLesson.assets == null || currentLesson.assets.Count == 0)
            return;

        currentStepIndex = 0;

        if (previewPanelController != null) previewPanelController.HidePreview();
        if (lessonPanel != null) lessonPanel.SetActive(true);
        if (completionPanel != null) completionPanel.SetActive(false);

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

        if (lessonUIController != null)
        {
            lessonUIController.ShowLesson(
                currentLesson.title,
                $"Bước {currentStepIndex + 1}",
                string.IsNullOrEmpty(step.content) ? "Không có nội dung." : step.content
            );
        }
    }

    public void ShowCompletion()
    {
        if (lessonPanel != null) lessonPanel.SetActive(false);
        if (completionPanel != null) completionPanel.SetActive(true);

        bool hasQuiz = true;
        bool hasGamification = false;

        if (completionPanelController != null)
        {
            completionPanelController.Setup(hasQuiz, hasGamification);
        }
    }

    public void GoHome()
    {
        SceneManager.LoadScene("HomeScene");
    }

    public void GoToQuiz()
    {
        SceneManager.LoadScene("QuizScene");
    }

    public void GoToGamification()
    {
        SceneManager.LoadScene("GamificationScene");
    }

    public void BackToScan()
    {
        currentStepIndex = 0;
        ShowWaitingState();
    }
}