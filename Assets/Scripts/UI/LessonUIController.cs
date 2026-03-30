using TMPro;
using UnityEngine;

public class LessonUIController : MonoBehaviour
{
    public TextMeshProUGUI lessonTitleText;
    public TextMeshProUGUI stepTitleText;
    public TextMeshProUGUI stepContentText;

    public void ShowLesson(string lessonTitle, string stepTitle, string stepContent)
    {
        lessonTitleText.text = lessonTitle;
        stepTitleText.text = stepTitle;
        stepContentText.text = stepContent;
    }
}