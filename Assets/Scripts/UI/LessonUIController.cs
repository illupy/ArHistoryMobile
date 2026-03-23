using TMPro;
using UnityEngine;

public class LessonUIController : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    public void ShowLesson(string title, string description)
    {
        titleText.text = title;
        descriptionText.text = description;
    }
}