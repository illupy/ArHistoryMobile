using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LessonUIController : MonoBehaviour
{
    public TextMeshProUGUI lessonTitleText;
    public TextMeshProUGUI stepTitleText;
    public TextMeshProUGUI stepContentText;

    public void ShowLesson(string lessonTitle, string stepTitle, string stepContent)
    {
        if (lessonTitleText != null) lessonTitleText.text = lessonTitle;
        if (stepTitleText != null) stepTitleText.text = stepTitle;
        if (stepContentText != null)
        {
            stepContentText.text = Utils.CleanHtmlTags(stepContent);

            // Reset scroll position to top
            ScrollRect scrollRect = stepContentText.GetComponentInParent<ScrollRect>();
            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.velocity = Vector2.zero;
                scrollRect.verticalNormalizedPosition = 1f;
            }
        }
    }
}