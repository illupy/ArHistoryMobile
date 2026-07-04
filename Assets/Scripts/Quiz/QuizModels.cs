using System;
using System.Collections.Generic;

[Serializable]
public class QuizDetailResponse
{
    public long id;
    public string title;
    public long lessonId;
    public List<QuestionItem> questions;
}

[Serializable]
public class QuestionItem
{
    public long id;
    public string question;
    public List<AnswerItem> answers;
}

[Serializable]
public class AnswerItem
{
    public long id;
    public string content;
    public bool correct;
}