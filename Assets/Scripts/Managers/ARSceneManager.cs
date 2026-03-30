using UnityEngine;
using UnityEngine.SceneManagement;

public class ARSceneManager : MonoBehaviour
{
    public GameObject scanGuidePanel;
    public GameObject lessonPanel;
    public GameObject completionPanel;

    public LessonUIController lessonUIController;
    public CompletionPanelController completionPanelController;

    private int currentStepIndex = 0;

    // Mock dữ liệu tạm để test UI
    private string lessonTitle = "Trận Bạch Đằng";

    private string[] stepTitles =
    {
        "Bước 1",
        "Bước 2",
        "Bước 3"
    };

    private string[] stepContents =
    {
        "Giới thiệu bối cảnh của sự kiện lịch sử.",
        "Mô tả diễn biến chính của sự kiện.",
        "Kết quả và ý nghĩa của sự kiện."
    };

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

    public void StartLesson()
    {
        currentStepIndex = 0;

        scanGuidePanel.SetActive(false);
        lessonPanel.SetActive(true);
        completionPanel.SetActive(false);

        ShowCurrentStep();
    }

    public void NextStep()
    {
        if (currentStepIndex < stepContents.Length - 1)
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
        if (currentStepIndex > 0)
        {
            currentStepIndex--;
            ShowCurrentStep();
        }
    }

    private void ShowCurrentStep()
    {
        if (lessonUIController != null)
        {
            lessonUIController.ShowLesson(
                lessonTitle,
                stepTitles[currentStepIndex],
                stepContents[currentStepIndex]
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
}