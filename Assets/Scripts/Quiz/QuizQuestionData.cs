using System;
using System.Collections.Generic;

[Serializable]
public class QuizQuestionData
{
    public string question;
    public List<string> answers;
    public int correctIndex;
}