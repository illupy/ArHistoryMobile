using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultSceneManager : MonoBehaviour
{
    public TextMeshProUGUI resultTitleText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI summaryText;

    public Button retryButton;
    public Button homeButton;

    private void Start()
    {
        int score = 0;
        int total = 0;

        if (QuizSessionStore.Instance != null && QuizSessionStore.Instance.CurrentQuiz != null)
        {
            score = QuizSessionStore.Instance.Score;
            total = QuizSessionStore.Instance.CurrentQuiz.questions != null
                ? QuizSessionStore.Instance.CurrentQuiz.questions.Count
                : 0;
        }

        if (resultTitleText != null)
            resultTitleText.text = "Kết quả bài Quiz";

        if (scoreText != null)
            scoreText.text = $"Điểm: {score}/{total}";

        if (summaryText != null)
            summaryText.text = score == total
                ? "Xuất sắc! Bạn đã trả lời đúng toàn bộ câu hỏi."
                : "Bạn đã hoàn thành bài quiz. Hãy xem lại bài học để ghi nhớ tốt hơn.";

        if (retryButton != null)
        {
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(RetryQuiz);
        }

        if (homeButton != null)
        {
            homeButton.onClick.RemoveAllListeners();
            homeButton.onClick.AddListener(GoHome);
        }
    }

    private void RetryQuiz()
    {
        if (QuizSessionStore.Instance != null)
        {
            QuizSessionStore.Instance.ResetQuiz();
        }

        SceneManager.LoadScene("QuizScene");
    }

    private void GoHome()
    {
        if (QuizSessionStore.Instance != null)
        {
            QuizSessionStore.Instance.ResetQuiz();
        }

        SceneManager.LoadScene("HomeScene");
    }
}