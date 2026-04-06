using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ARScanSceneAutoBuilder
{
    [MenuItem("Tools/AR History/Build ARScan Scene UI")]
    public static void BuildARScanSceneUI()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "ARScanScene")
        {
            Debug.LogWarning("Hãy mở ARScanScene trước khi chạy script này.");
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

        // ScanGuidePanel
        GameObject scanGuidePanel = CreatePanel(canvasGO.transform, "ScanGuidePanel", new Color(0f, 0f, 0f, 0.18f), true);
        GameObject guideText = CreateTMPText("GuideText", scanGuidePanel.transform, "Đưa thẻ bài học vào giữa khung hình để bắt đầu", 34, FontStyles.Bold);
        SetRect(guideText.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -130), new Vector2(860, 120));

        GameObject frame = CreateUIObject("ScanFrame", scanGuidePanel.transform);
        Image frameImg = frame.AddComponent<Image>();
        frameImg.color = new Color(1f, 1f, 1f, 0.08f);
        RectTransform frameRT = frame.GetComponent<RectTransform>();
        frameRT.sizeDelta = new Vector2(680, 680);
        frameRT.anchoredPosition = new Vector2(0, 100);

        GameObject backHomeBtn = CreateButton(scanGuidePanel.transform, "BackHomeButton", "Về trang chủ", new Vector2(420, 90), new Vector2(0, -760));

        // PreviewPanel
        GameObject previewPanel = CreateBottomCard(canvasGO.transform, "PreviewPanel", 1080, 620);
        previewPanel.SetActive(false);

        GameObject lessonTitle = CreateTMPText("LessonTitleText", previewPanel.transform, "Tên bài học", 48, FontStyles.Bold);
        SetRect(lessonTitle.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -90), new Vector2(880, 90));

        GameObject previewText = CreateTMPText("PreviewText", previewPanel.transform, "Mô tả preview bài học...", 30, FontStyles.Normal);
        TMP_Text previewTMP = previewText.GetComponent<TMP_Text>();
        previewTMP.alignment = TextAlignmentOptions.TopLeft;
        SetRect(previewText.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -260), new Vector2(860, 220));

        GameObject startLessonBtn = CreateButton(previewPanel.transform, "StartLessonButton", "Bắt đầu bài học", new Vector2(420, 90), new Vector2(-220, 220));
        GameObject skipVoiceBtn = CreateButton(previewPanel.transform, "SkipVoiceButton", "Bỏ qua voice", new Vector2(300, 90), new Vector2(200, 220));

        // LessonPanel
        GameObject lessonPanel = CreateBottomCard(canvasGO.transform, "LessonPanel", 1080, 700);
        lessonPanel.SetActive(false);

        GameObject lpTitle = CreateTMPText("LessonTitleText", lessonPanel.transform, "Tên bài học", 46, FontStyles.Bold);
        SetRect(lpTitle.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -80), new Vector2(900, 80));

        GameObject stepTitle = CreateTMPText("StepTitleText", lessonPanel.transform, "Bước 1", 34, FontStyles.Bold);
        SetRect(stepTitle.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -160), new Vector2(860, 70));

        GameObject stepContent = CreateTMPText("StepContentText", lessonPanel.transform, "Nội dung bài học...", 28, FontStyles.Normal);
        TMP_Text stepTMP = stepContent.GetComponent<TMP_Text>();
        stepTMP.alignment = TextAlignmentOptions.TopLeft;
        SetRect(stepContent.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -360), new Vector2(860, 280));

        GameObject previousBtn = CreateButton(lessonPanel.transform, "PreviousButton", "Trước", new Vector2(260, 90), new Vector2(-280, 250));
        GameObject nextBtn = CreateButton(lessonPanel.transform, "NextButton", "Tiếp", new Vector2(260, 90), new Vector2(0, 250));
        GameObject backToScanBtn = CreateButton(lessonPanel.transform, "BackToScanButton", "Về quét", new Vector2(260, 90), new Vector2(280, 250));

        // CompletionPanel
        GameObject completionPanel = CreatePanel(canvasGO.transform, "CompletionPanel", new Color(0f, 0f, 0f, 0.6f), true);
        completionPanel.SetActive(false);

        GameObject completionCard = CreateUIObject("CompletionCard", completionPanel.transform);
        Image ccImg = completionCard.AddComponent<Image>();
        ccImg.color = new Color(1f, 1f, 1f, 0.97f);
        RectTransform ccRT = completionCard.GetComponent<RectTransform>();
        ccRT.sizeDelta = new Vector2(880, 760);
        ccRT.anchoredPosition = Vector2.zero;

        GameObject completeTitle = CreateTMPText("CompletionTitleText", completionCard.transform, "Bạn đã hoàn thành bài học", 48, FontStyles.Bold);
        SetRect(completeTitle.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -120), new Vector2(760, 100));

        GameObject quizBtn = CreateButton(completionCard.transform, "QuizButton", "Làm Quiz", new Vector2(420, 100), new Vector2(0, 40));
        GameObject gameBtn = CreateButton(completionCard.transform, "GamificationButton", "Chơi tương tác", new Vector2(420, 100), new Vector2(0, 180));
        GameObject homeBtn = CreateButton(completionCard.transform, "HomeButton", "Về trang chủ", new Vector2(420, 100), new Vector2(0, 320));

        // Managers
        GameObject previewControllerGO = new GameObject("PreviewPanelController");
        var previewController = previewControllerGO.AddComponent<PreviewPanelController>();
        previewController.previewPanel = previewPanel;
        previewController.lessonTitleText = lessonTitle.GetComponent<TextMeshProUGUI>();
        previewController.previewText = previewText.GetComponent<TextMeshProUGUI>();
        previewController.startLessonButton = startLessonBtn;
        previewController.skipVoiceButton = skipVoiceBtn;

        GameObject lessonControllerGO = lessonPanel;
        var lessonController = lessonControllerGO.AddComponent<LessonUIController>();
        lessonController.lessonTitleText = lpTitle.GetComponent<TextMeshProUGUI>();
        lessonController.stepTitleText = stepTitle.GetComponent<TextMeshProUGUI>();
        lessonController.stepContentText = stepContent.GetComponent<TextMeshProUGUI>();

        var completionController = completionPanel.AddComponent<CompletionPanelController>();
        completionController.quizButton = quizBtn;
        completionController.gamificationButton = gameBtn;

        GameObject sceneManagerGO = new GameObject("ARPreviewSceneManager");
        var sceneManager = sceneManagerGO.AddComponent<ARPreviewSceneManager>();
        sceneManager.scanGuidePanel = scanGuidePanel;
        sceneManager.previewPanelController = previewController;
        sceneManager.lessonPanel = lessonPanel;
        sceneManager.lessonUIController = lessonController;
        sceneManager.completionPanel = completionPanel;
        sceneManager.completionPanelController = completionController;

        // Hook runtime buttons manually later in Inspector if needed
        // We keep this builder UI-focused to avoid fragile serialized event wiring.

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Đã build xong ARScanScene UI.");
    }

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static GameObject CreatePanel(Transform parent, string name, Color color, bool stretch)
    {
        GameObject go = CreateUIObject(name, parent);
        Image img = go.AddComponent<Image>();
        img.color = color;

        if (stretch)
        {
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        return go;
    }

    private static GameObject CreateBottomCard(Transform parent, string name, float width, float height)
    {
        GameObject go = CreateUIObject(name, parent);
        Image img = go.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.95f);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.sizeDelta = new Vector2(width, height);
        rt.anchoredPosition = new Vector2(0, 0);

        return go;
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
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;

        GameObject txt = CreateTMPText("Text", btn.transform, label, 32, FontStyles.Bold);
        TMP_Text tmp = txt.GetComponent<TMP_Text>();
        tmp.color = Color.white;
        RectTransform tr = txt.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero;
        tr.offsetMax = Vector2.zero;

        return btn;
    }

    private static void SetRect(RectTransform rt, Vector2 min, Vector2 max, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }
}