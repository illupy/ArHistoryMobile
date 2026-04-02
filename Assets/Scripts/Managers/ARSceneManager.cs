using UnityEngine;
using UnityEngine.SceneManagement;

public class ARSceneManager : MonoBehaviour
{
    public GameObject scanGuidePanel;
    public GameObject lessonPanel;
    public GameObject completionPanel;

    public LessonUIController lessonUIController;
    public CompletionPanelController completionPanelController;
    public ARContentPresenter arContentPresenter;

    private LessonDetailResponse currentLesson;
    private int currentStepIndex = 0;

    // Tạm thời mock cờ chức năng
    private bool hasQuiz = true;
    private bool hasGamification = true;

    private void Start()
    {
        ShowWaitingState();
    }

    public void ShowWaitingState()
    {
        scanGuidePanel.SetActive(true);
        lessonPanel.SetActive(false);
        completionPanel.SetActive(false);
    }

    public void OnLessonLoaded(LessonDetailResponse lesson)
    {
        currentLesson = lesson;
        currentStepIndex = 0;

        scanGuidePanel.SetActive(false);
        lessonPanel.SetActive(true);
        completionPanel.SetActive(false);

        ShowCurrentStep();
    }

    public void NextStep()
    {
        if (currentLesson == null || currentLesson.assets == null || currentLesson.assets.Count == 0)
            return;

        if (currentStepIndex < currentLesson.assets.Count - 1)
        {
            currentStepIndex++;
            ShowCurrentStep();
        }
        else
        {
            ShowCompletionState();
        }
    }

    public void PreviousStep()
    {
        if (currentLesson == null || currentLesson.assets == null || currentLesson.assets.Count == 0)
            return;

        if (currentStepIndex > 0)
        {
            currentStepIndex--;
            ShowCurrentStep();
        }
    }

    private void ShowCurrentStep()
    {
        if (currentLesson == null || currentLesson.assets == null || currentLesson.assets.Count == 0)
            return;

        var step = currentLesson.assets[currentStepIndex];

        if (lessonUIController != null)
        {
            lessonUIController.ShowLesson(
                currentLesson.title,
                $"Bước {currentStepIndex + 1}",
                string.IsNullOrEmpty(step.content) ? "Không có nội dung cho bước này." : step.content
            );
        }
    }

    public void ShowCompletionState()
    {
        scanGuidePanel.SetActive(false);
        lessonPanel.SetActive(false);
        completionPanel.SetActive(true);

        if (completionPanelController != null)
        {
            completionPanelController.Setup(hasQuiz, hasGamification);
        }
    }

    public void GoToQuiz()
    {
        SceneManager.LoadScene("QuizScene");
    }

    public void GoToGamification()
    {
        SceneManager.LoadScene("GamificationScene");
    }

    public void GoHome()
    {
        SceneManager.LoadScene("HomeScene");
    }

    public void BackToScanGuide()
    {
        ShowWaitingState();
    }

    // Dùng tạm nếu muốn test UI mà chưa gọi API
    public void StartLessonMock()
    {
        LessonDetailResponse mockLesson = new LessonDetailResponse
        {
            id = 1,
            title = "Trận Bạch Đằng",
            description = "Bài học mô phỏng sự kiện lịch sử",
            content = "Tổng quan bài học",
            status = "PUBLISHED",
            assets = new System.Collections.Generic.List<LessonAssetItem>
            {
                new LessonAssetItem { id = 1, type = "TEXT", content = "Giới thiệu bối cảnh của sự kiện lịch sử.", orderIndex = 1 },
                new LessonAssetItem { id = 2, type = "TEXT", content = "Mô tả diễn biến chính của sự kiện.", orderIndex = 2 },
                new LessonAssetItem { id = 3, type = "TEXT", content = "Kết quả và ý nghĩa của sự kiện.", orderIndex = 3 }
            }
        };

        OnLessonLoaded(mockLesson);
    }
}