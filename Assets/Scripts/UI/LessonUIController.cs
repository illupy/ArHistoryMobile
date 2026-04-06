using TMPro;
using UnityEngine;

public class LessonUIController : MonoBehaviour
{
    public TextMeshProUGUI lessonTitleText;
    public TextMeshProUGUI stepTitleText;
    public TextMeshProUGUI stepContentText;

    public void ShowLesson(string lessonTitle, string stepTitle, string stepContent)
    {
        if (lessonTitleText != null) lessonTitleText.text = lessonTitle;
        if (stepTitleText != null) stepTitleText.text = stepTitle;
        if (stepContentText != null) stepContentText.text = stepContent;
    }
}