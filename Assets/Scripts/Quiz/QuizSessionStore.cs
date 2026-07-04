using System.Collections.Generic;
using UnityEngine;

public class QuizSessionStore : MonoBehaviour
{
    public static QuizSessionStore Instance;

    public QuizDetailResponse CurrentQuiz;
    public int CurrentQuestionIndex = 0;
    public int Score = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetQuiz(QuizDetailResponse quiz)
    {
        CurrentQuiz = quiz;
        CurrentQuestionIndex = 0;
        Score = 0;
    }

    public void ResetQuiz()
    {
        CurrentQuestionIndex = 0;
        Score = 0;
    }
}