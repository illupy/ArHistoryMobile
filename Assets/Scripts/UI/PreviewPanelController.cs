using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreviewPanelController : MonoBehaviour
{
    public GameObject previewPanel;
    public TextMeshProUGUI lessonTitleText;
    public TextMeshProUGUI previewText;
    public GameObject startLessonButton;
    public GameObject skipVoiceButton;

    public void ShowPreview(string title, string description, bool canStartLesson = false)
    {
        if (previewPanel != null) previewPanel.SetActive(true);
        if (lessonTitleText != null) lessonTitleText.text = title;
        if (previewText != null) previewText.text = description;

        if (startLessonButton != null) startLessonButton.SetActive(canStartLesson);
        if (skipVoiceButton != null) skipVoiceButton.SetActive(true);
    }

    public void EnableStartLesson()
    {
        if (startLessonButton != null) startLessonButton.SetActive(true);
    }

    public void HidePreview()
    {
        if (previewPanel != null) previewPanel.SetActive(false);
    }
}