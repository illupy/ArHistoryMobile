using Newtonsoft.Json;
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

    public QuizApiService quizApiService;

    private QuizDetailResponse currentQuiz;
    private int currentIndex = 0;
    private int score = 0;

    private void Start()
    {
        BindButtons();
        LoadQuiz();
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

    private void LoadQuiz()
    {
        if (LessonSessionStore.Instance == null || LessonSessionStore.Instance.CurrentLesson == null)
        {
            questionText.text = "Không tìm thấy bài học hiện tại.";
            return;
        }

        long lessonId = LessonSessionStore.Instance.CurrentLesson.id;

        StartCoroutine(quizApiService.GetQuizByLessonId(
            lessonId,
            json =>
            {
                ApiResponse<QuizDetailResponse> response =
                    JsonConvert.DeserializeObject<ApiResponse<QuizDetailResponse>>(json);

                if (response != null && response.success && response.data != null)
                {
                    currentQuiz = response.data;
                    currentIndex = 0;
                    score = 0;

                    if (QuizSessionStore.Instance != null)
                    {
                        QuizSessionStore.Instance.SetQuiz(currentQuiz);
                    }

                    ShowQuestion();
                }
                else
                {
                    questionText.text = "Không tải được quiz.";
                }
            },
            error =>
            {
                questionText.text = "Lỗi tải quiz: " + error;
            }
        ));
    }

    private void ShowQuestion()
    {
        if (currentQuiz == null || currentQuiz.questions == null || currentQuiz.questions.Count == 0)
        {
            questionText.text = "Không có câu hỏi.";
            return;
        }

        if (currentIndex >= currentQuiz.questions.Count)
        {
            GoToResult();
            return;
        }

        var q = currentQuiz.questions[currentIndex];

        if (lessonTitleText != null)
            lessonTitleText.text = currentQuiz.title;

        if (questionIndexText != null)
            questionIndexText.text = $"Câu {currentIndex + 1}/{currentQuiz.questions.Count}";

        if (questionText != null)
            questionText.text = q.question;

        answerText1.text = q.answers.Count > 0 ? q.answers[0].content : "";
        answerText2.text = q.answers.Count > 1 ? q.answers[1].content : "";
        answerText3.text = q.answers.Count > 2 ? q.answers[2].content : "";
        answerText4.text = q.answers.Count > 3 ? q.answers[3].content : "";
    }

    private void SelectAnswer(int selectedIndex)
    {
        if (currentQuiz == null || currentIndex >= currentQuiz.questions.Count) return;

        var q = currentQuiz.questions[currentIndex];

        if (selectedIndex < q.answers.Count && q.answers[selectedIndex].correct)
        {
            score++;
        }

        currentIndex++;

        if (QuizSessionStore.Instance != null)
        {
            QuizSessionStore.Instance.Score = score;
            QuizSessionStore.Instance.CurrentQuestionIndex = currentIndex;
        }

        if (currentIndex >= currentQuiz.questions.Count)
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
}