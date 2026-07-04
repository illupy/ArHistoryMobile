using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizSceneAutoBuilder
{
    [MenuItem("Tools/AR History/Build Quiz Scene UI")]
    public static void BuildQuizSceneUI()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "QuizScene")
        {
            Debug.LogWarning("Hãy mở QuizScene trước khi chạy script này.");
            return;
        }

        Canvas existingCanvas = Object.FindFirstObjectByType<Canvas>();
        if (existingCanvas != null)
        {
            Debug.LogWarning("Scene đã có Canvas. Xóa Canvas cũ nếu muốn build lại.");
            return;
        }

        GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem",
                typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        // Background
        GameObject bg = CreateUIObject("Background", canvasGO.transform);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.96f, 0.93f, 0.85f, 1f);
        StretchFull(bg.GetComponent<RectTransform>());

        // MainCard
        GameObject mainCard = CreateUIObject("MainCard", canvasGO.transform);
        Image cardImg = mainCard.AddComponent<Image>();
        cardImg.color = new Color(1f, 1f, 1f, 0.96f);
        RectTransform cardRT = mainCard.GetComponent<RectTransform>();
        cardRT.sizeDelta = new Vector2(920, 1500);
        cardRT.anchoredPosition = Vector2.zero;

        GameObject lessonTitle = CreateTMPText("LessonTitleText", mainCard.transform, "Quiz bài học", 54, FontStyles.Bold);
        SetRect(lessonTitle.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -100), new Vector2(780, 90));

        GameObject qIndex = CreateTMPText("QuestionIndexText", mainCard.transform, "Câu 1/3", 30, FontStyles.Bold);
        SetRect(qIndex.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -180), new Vector2(700, 60));

        GameObject qText = CreateTMPText("QuestionText", mainCard.transform, "Nội dung câu hỏi...", 36, FontStyles.Normal);
        TMP_Text qTextTMP = qText.GetComponent<TMP_Text>();
        qTextTMP.alignment = TextAlignmentOptions.TopLeft;
        SetRect(qText.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -360), new Vector2(760, 180));

        GameObject answer1 = CreateButton(mainCard.transform, "AnswerButton1", "Đáp án 1", new Vector2(760, 100), new Vector2(0, 760));
        GameObject answer2 = CreateButton(mainCard.transform, "AnswerButton2", "Đáp án 2", new Vector2(760, 100), new Vector2(0, 620));
        GameObject answer3 = CreateButton(mainCard.transform, "AnswerButton3", "Đáp án 3", new Vector2(760, 100), new Vector2(0, 480));
        GameObject answer4 = CreateButton(mainCard.transform, "AnswerButton4", "Đáp án 4", new Vector2(760, 100), new Vector2(0, 340));

        GameObject homeBtn = CreateButton(mainCard.transform, "HomeButton", "Về trang chủ", new Vector2(420, 90), new Vector2(0, 140));

        GameObject managerGO = new GameObject("QuizSceneManager");
        QuizSceneManager manager = managerGO.AddComponent<QuizSceneManager>();

        manager.lessonTitleText = lessonTitle.GetComponent<TextMeshProUGUI>();
        manager.questionIndexText = qIndex.GetComponent<TextMeshProUGUI>();
        manager.questionText = qText.GetComponent<TextMeshProUGUI>();

        manager.answerButton1 = answer1.GetComponent<Button>();
        manager.answerButton2 = answer2.GetComponent<Button>();
        manager.answerButton3 = answer3.GetComponent<Button>();
        manager.answerButton4 = answer4.GetComponent<Button>();

        manager.answerText1 = answer1.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        manager.answerText2 = answer2.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        manager.answerText3 = answer3.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        manager.answerText4 = answer4.transform.Find("Text").GetComponent<TextMeshProUGUI>();

        manager.homeButton = homeBtn.GetComponent<Button>();

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Đã build xong QuizScene UI.");
    }

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static void SetRect(RectTransform rt, Vector2 min, Vector2 max, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    private static GameObject CreateTMPText(string name, Transform parent, string text, float fontSize, FontStyles style)
    {
        GameObject go = CreateUIObject(name, parent);
        TMP_Text tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.14f, 0.16f, 0.2f, 1f);
        return go;
    }

    private static GameObject CreateButton(Transform parent, string name, string label, Vector2 size, Vector2 pos)
    {
        GameObject btn = CreateUIObject(name, parent);
        Image img = btn.AddComponent<Image>();
        img.color = new Color(0.17f, 0.36f, 0.62f, 1f);
        btn.AddComponent<Button>();

        RectTransform rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = size;
        rt.anchoredPosition = new Vector2(pos.x, -pos.y);

        GameObject txt = CreateTMPText("Text", btn.transform, label, 30, FontStyles.Bold);
        TMP_Text tmp = txt.GetComponent<TMP_Text>();
        tmp.color = Color.white;
        StretchFull(txt.GetComponent<RectTransform>());

        return btn;
    }
}