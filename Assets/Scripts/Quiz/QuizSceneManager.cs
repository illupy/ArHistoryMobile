using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizSceneManager : MonoBehaviour
{
    public TextMeshProUGUI lessonTitleText;
    public TextMeshProUGUI questionIndexText;
    public TextMeshProUGUI questionText;

    public Button answerButton1;
    public Button answerButton2;
    public Button answerButton3;
    public Button answerButton4;

    public TextMeshProUGUI answerText1;
    public TextMeshProUGUI answerText2;
    public TextMeshProUGUI answerText3;
    public TextMeshProUGUI answerText4;

    public Button homeButton;

    private List<QuizQuestionData> questions;
    private int currentIndex = 0;
    private int score = 0;

    private void Start()
    {
        // Nếu chưa có data từ store thì mock luôn
        if (QuizSessionStore.Instance == null)
        {
            Debug.LogWarning("QuizSessionStore chưa có, dùng mock data cục bộ.");
            questions = CreateMockQuestions();
        }
        else if (QuizSessionStore.Instance.Questions == null || QuizSessionStore.Instance.Questions.Count == 0)
        {
            questions = CreateMockQuestions();
            QuizSessionStore.Instance.SetQuestions(questions);
        }
        else
        {
            questions = QuizSessionStore.Instance.Questions;
            currentIndex = QuizSessionStore.Instance.CurrentQuestionIndex;
            score = QuizSessionStore.Instance.Score;
        }

        if (lessonTitleText != null)
            lessonTitleText.text = "Quiz bài học";

        BindButtons();
        ShowQuestion();
    }

    private void BindButtons()
    {
        answerButton1.onClick.RemoveAllListeners();
        answerButton2.onClick.RemoveAllListeners();
        answerButton3.onClick.RemoveAllListeners();
        answerButton4.onClick.RemoveAllListeners();

        answerButton1.onClick.AddListener(() => SelectAnswer(0));
        answerButton2.onClick.AddListener(() => SelectAnswer(1));
        answerButton3.onClick.AddListener(() => SelectAnswer(2));
        answerButton4.onClick.AddListener(() => SelectAnswer(3));

        if (homeButton != null)
        {
            homeButton.onClick.RemoveAllListeners();
            homeButton.onClick.AddListener(GoHome);
        }
    }

    private void ShowQuestion()
    {
        if (questions == null || questions.Count == 0)
        {
            questionText.text = "Không có câu hỏi.";
            return;
        }

        if (currentIndex >= questions.Count)
        {
            GoToResult();
            return;
        }

        QuizQuestionData q = questions[currentIndex];

        if (questionIndexText != null)
            questionIndexText.text = $"Câu {currentIndex + 1}/{questions.Count}";

        if (questionText != null)
            questionText.text = q.question;

        answerText1.text = q.answers.Count > 0 ? q.answers[0] : "";
        answerText2.text = q.answers.Count > 1 ? q.answers[1] : "";
        answerText3.text = q.answers.Count > 2 ? q.answers[2] : "";
        answerText4.text = q.answers.Count > 3 ? q.answers[3] : "";
    }

    private void SelectAnswer(int selectedIndex)
    {
        if (questions == null || currentIndex >= questions.Count) return;

        QuizQuestionData q = questions[currentIndex];

        if (selectedIndex == q.correctIndex)
        {
            score++;
        }

        currentIndex++;

        if (QuizSessionStore.Instance != null)
        {
            QuizSessionStore.Instance.CurrentQuestionIndex = currentIndex;
            QuizSessionStore.Instance.Score = score;
        }

        if (currentIndex >= questions.Count)
        {
            GoToResult();
        }
        else
        {
            ShowQuestion();
        }
    }

    private void GoToResult()
    {
        SceneManager.LoadScene("ResultScene");
    }

    private void GoHome()
    {
        SceneManager.LoadScene("HomeScene");
    }

    private List<QuizQuestionData> CreateMockQuestions()
    {
        return new List<QuizQuestionData>
        {
            new QuizQuestionData
            {
                question = "Trận Bạch Đằng gắn liền với chiến thắng chống quân xâm lược nào?",
                answers = new List<string> { "Quân Nguyên", "Quân Tống", "Quân Minh", "Quân Thanh" },
                correctIndex = 0
            },
            new QuizQuestionData
            {
                question = "Chiến thuật nổi bật trong trận Bạch Đằng là gì?",
                answers = new List<string> { "Dùng voi chiến", "Dùng cọc gỗ trên sông", "Đắp thành đất", "Phục kích trong rừng" },
                correctIndex = 1
            },
            new QuizQuestionData
            {
                question = "Ý nghĩa lớn của chiến thắng Bạch Đằng là gì?",
                answers = new List<string> { "Mở rộng lãnh thổ", "Kết thúc nội chiến", "Bảo vệ độc lập dân tộc", "Dời đô ra Thăng Long" },
                correctIndex = 2
            }
        };
    }
}