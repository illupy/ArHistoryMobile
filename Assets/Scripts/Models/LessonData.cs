using System.Collections.Generic;

public class LessonData
{
    public long id;
    public string title;
    public string description;
    public bool hasQuiz;
    public bool hasGamification;
    public List<LessonStep> steps;
}