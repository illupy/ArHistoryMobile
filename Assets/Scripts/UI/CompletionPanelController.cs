using UnityEngine;

public class CompletionPanelController : MonoBehaviour
{
    public GameObject quizButton;
    public GameObject gamificationButton;

    public void Setup(bool hasQuiz, bool hasGamification)
    {
        if (quizButton != null)
            quizButton.SetActive(hasQuiz);

        if (gamificationButton != null)
            gamificationButton.SetActive(hasGamification);
    }
}