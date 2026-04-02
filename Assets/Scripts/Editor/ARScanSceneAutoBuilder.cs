#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ARScanSceneAutoBuilder : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/AR History/Build ARScan Scene UI")]
    public static void BuildARScanScene()
    {
        var canvas = FindOrCreateCanvas();
        ClearChildren(canvas.transform);

        // TopBar
        var topBar = CreateUIObject("TopBar", canvas.transform);
        var topBarImage = topBar.AddComponent<Image>();
        topBarImage.color = new Color32(20, 20, 20, 120);
        var topBarRt = topBar.GetComponent<RectTransform>();
        topBarRt.anchorMin = new Vector2(0f, 1f);
        topBarRt.anchorMax = new Vector2(1f, 1f);
        topBarRt.pivot = new Vector2(0.5f, 1f);
        topBarRt.sizeDelta = new Vector2(0, 160);
        topBarRt.anchoredPosition = Vector2.zero;

        var topTitle = CreateTMPText("TopTitleText", topBar.transform, "Quét bài học AR", 44, FontStyles.Bold, Color.white, TextAlignmentOptions.Center);
        StretchFull(topTitle.rectTransform);
        topTitle.rectTransform.offsetMin = new Vector2(140, 20);
        topTitle.rectTransform.offsetMax = new Vector2(-140, -20);

        var backBtn = CreateButton(topBar.transform, "BackHomeButton", "←", new Color32(96, 76, 61, 220), new Vector2(110, 84), new Vector2(80, -80), false);
        SetTopLeft(backBtn.GetComponent<RectTransform>(), new Vector2(80, -80));

        // ScanGuidePanel
        var scanGuidePanel = CreateUIObject("ScanGuidePanel", canvas.transform);
        StretchFull(scanGuidePanel.GetComponent<RectTransform>());

        var scanFrame = CreateUIObject("ScanFrame", scanGuidePanel.transform);
        var frameImage = scanFrame.AddComponent<Image>();
        frameImage.color = new Color(1f, 1f, 1f, 0.05f);
        var frameRt = scanFrame.GetComponent<RectTransform>();
        frameRt.anchorMin = new Vector2(0.5f, 0.5f);
        frameRt.anchorMax = new Vector2(0.5f, 0.5f);
        frameRt.pivot = new Vector2(0.5f, 0.5f);
        frameRt.sizeDelta = new Vector2(720, 720);
        frameRt.anchoredPosition = new Vector2(0, 80);

        var guideText = CreateTMPText("GuideText", scanGuidePanel.transform, "Đưa thẻ bài học vào giữa khung hình", 42, FontStyles.Bold, Color.white, TextAlignmentOptions.Center);
        var guideTextRt = guideText.rectTransform;
        guideTextRt.anchorMin = new Vector2(0.5f, 0f);
        guideTextRt.anchorMax = new Vector2(0.5f, 0f);
        guideTextRt.pivot = new Vector2(0.5f, 0f);
        guideTextRt.sizeDelta = new Vector2(900, 100);
        guideTextRt.anchoredPosition = new Vector2(0, 220);

        // LessonPanel
        var lessonPanel = CreateUIObject("LessonPanel", canvas.transform);
        var lessonPanelImg = lessonPanel.AddComponent<Image>();
        lessonPanelImg.color = new Color32(255, 250, 242, 245);
        var lessonPanelRt = lessonPanel.GetComponent<RectTransform>();
        lessonPanelRt.anchorMin = new Vector2(0.5f, 0f);
        lessonPanelRt.anchorMax = new Vector2(0.5f, 0f);
        lessonPanelRt.pivot = new Vector2(0.5f, 0f);
        lessonPanelRt.sizeDelta = new Vector2(960, 640);
        lessonPanelRt.anchoredPosition = new Vector2(0, 80);

        var lessonTitle = CreateTMPText("LessonTitleText", lessonPanel.transform, "Tên bài học", 44, FontStyles.Bold, new Color32(41, 33, 28, 255), TextAlignmentOptions.Left);
        var lessonTitleRt = lessonTitle.rectTransform;
        lessonTitleRt.anchorMin = new Vector2(0f, 1f);
        lessonTitleRt.anchorMax = new Vector2(1f, 1f);
        lessonTitleRt.pivot = new Vector2(0.5f, 1f);
        lessonTitleRt.sizeDelta = new Vector2(-120, 80);
        lessonTitleRt.anchoredPosition = new Vector2(0, -40);
        lessonTitleRt.offsetMin = new Vector2(60, 0);
        lessonTitleRt.offsetMax = new Vector2(-60, 0);

        var stepTitle = CreateTMPText("StepTitleText", lessonPanel.transform, "Bước 1", 34, FontStyles.Bold, new Color32(110, 90, 70, 255), TextAlignmentOptions.Left);
        var stepTitleRt = stepTitle.rectTransform;
        stepTitleRt.anchorMin = new Vector2(0f, 1f);
        stepTitleRt.anchorMax = new Vector2(1f, 1f);
        stepTitleRt.pivot = new Vector2(0.5f, 1f);
        stepTitleRt.sizeDelta = new Vector2(-120, 60);
        stepTitleRt.anchoredPosition = new Vector2(0, -115);
        stepTitleRt.offsetMin = new Vector2(60, 0);
        stepTitleRt.offsetMax = new Vector2(-60, 0);

        var contentText = CreateTMPText("StepContentText", lessonPanel.transform, "Nội dung bài học sẽ hiển thị ở đây sau khi quét marker thành công.", 30, FontStyles.Normal, new Color32(70, 63, 57, 255), TextAlignmentOptions.TopLeft);
        var contentRt = contentText.rectTransform;
        contentRt.anchorMin = new Vector2(0f, 0f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.offsetMin = new Vector2(60, 160);
        contentRt.offsetMax = new Vector2(-60, -170);

        var prevBtn = CreateButton(lessonPanel.transform, "PreviousButton", "Trước", new Color32(196, 172, 136, 255), new Vector2(220, 90), new Vector2(180, 60), true);
        var nextBtn = CreateButton(lessonPanel.transform, "NextButton", "Tiếp", new Color32(74, 109, 124, 255), new Vector2(220, 90), new Vector2(420, 60), true);

        // CompletionPanel
        var completionPanel = CreateUIObject("CompletionPanel", canvas.transform);
        var completionImage = completionPanel.AddComponent<Image>();
        completionImage.color = new Color32(20, 20, 20, 180);
        StretchFull(completionPanel.GetComponent<RectTransform>());

        var completionCard = CreateUIObject("CompletionCard", completionPanel.transform);
        var completionCardImage = completionCard.AddComponent<Image>();
        completionCardImage.color = new Color32(255, 250, 242, 255);
        var completionCardRt = completionCard.GetComponent<RectTransform>();
        completionCardRt.anchorMin = new Vector2(0.5f, 0.5f);
        completionCardRt.anchorMax = new Vector2(0.5f, 0.5f);
        completionCardRt.pivot = new Vector2(0.5f, 0.5f);
        completionCardRt.sizeDelta = new Vector2(860, 700);
        completionCardRt.anchoredPosition = Vector2.zero;

        var completionTitle = CreateTMPText("CompletionTitleText", completionCard.transform, "Bạn đã hoàn thành bài học", 52, FontStyles.Bold, new Color32(41, 33, 28, 255), TextAlignmentOptions.Center);
        var completionTitleRt = completionTitle.rectTransform;
        completionTitleRt.anchorMin = new Vector2(0.5f, 1f);
        completionTitleRt.anchorMax = new Vector2(0.5f, 1f);
        completionTitleRt.pivot = new Vector2(0.5f, 1f);
        completionTitleRt.sizeDelta = new Vector2(700, 100);
        completionTitleRt.anchoredPosition = new Vector2(0, -70);

        var quizBtn = CreateButton(completionCard.transform, "QuizButton", "Làm Quiz", new Color32(74, 109, 124, 255), new Vector2(420, 110), new Vector2(0, -220), false);
        var gameBtn = CreateButton(completionCard.transform, "GamificationButton", "Chơi tương tác", new Color32(196, 172, 136, 255), new Vector2(420, 110), new Vector2(0, -360), false);
        var homeBtn = CreateButton(completionCard.transform, "HomeButton", "Về Trang chủ", new Color32(96, 76, 61, 255), new Vector2(420, 110), new Vector2(0, -500), false);
        completionPanel.SetActive(false);

        // EventSystem
        FindOrCreateEventSystem();

        // Scene manager object
        var sceneManagerGo = GameObject.Find("ARSceneManager") ?? new GameObject("ARSceneManager");
        var sceneManager = sceneManagerGo.GetComponent<ARSceneManager>() ?? sceneManagerGo.AddComponent<ARSceneManager>();
        sceneManager.scanGuidePanel = scanGuidePanel;
        sceneManager.lessonPanel = lessonPanel;
        sceneManager.completionPanel = completionPanel;

        var lessonUI = sceneManagerGo.GetComponent<LessonUIController>() ?? sceneManagerGo.AddComponent<LessonUIController>();
        lessonUI.lessonTitleText = lessonTitle;
        lessonUI.stepTitleText = stepTitle;
        lessonUI.stepContentText = contentText;
        sceneManager.lessonUIController = lessonUI;

        var completionCtrl = sceneManagerGo.GetComponent<CompletionPanelController>() ?? sceneManagerGo.AddComponent<CompletionPanelController>();
        completionCtrl.quizButton = quizBtn;
        completionCtrl.gamificationButton = gameBtn;
        sceneManager.completionPanelController = completionCtrl;

        // AR content presenter object
        var presenterGo = GameObject.Find("ARContentPresenter") ?? new GameObject("ARContentPresenter");
        var presenter = presenterGo.GetComponent<ARContentPresenter>() ?? presenterGo.AddComponent<ARContentPresenter>();
        var imageTarget = GameObject.Find("ImageTarget");
        if (imageTarget != null)
        {
            var root = imageTarget.transform.Find("ARContentRoot");
            if (root == null)
            {
                var rootGo = new GameObject("ARContentRoot");
                rootGo.transform.SetParent(imageTarget.transform, false);
                rootGo.transform.localPosition = Vector3.zero;
                rootGo.transform.localRotation = Quaternion.identity;
                rootGo.transform.localScale = Vector3.one;
                root = rootGo.transform;
            }
            presenter.arContentRoot = root;
        }
        sceneManager.arContentPresenter = presenter;

        // Hook buttons if methods exist
        HookButton(backBtn, sceneManagerGo, nameof(ARSceneManager.GoHome));
        HookButton(prevBtn, sceneManagerGo, nameof(ARSceneManager.PreviousStep));
        HookButton(nextBtn, sceneManagerGo, nameof(ARSceneManager.NextStep));
        HookButton(quizBtn, sceneManagerGo, nameof(ARSceneManager.GoToQuiz));
        HookButton(gameBtn, sceneManagerGo, nameof(ARSceneManager.GoToGamification));
        HookButton(homeBtn, sceneManagerGo, nameof(ARSceneManager.GoHome));

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("ARScanScene UI built successfully.");
    }

    static void HookButton(GameObject buttonGo, GameObject target, string methodName)
    {
        var button = buttonGo.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        var sceneManager = target.GetComponent<ARSceneManager>();
        if (sceneManager == null) return;
        button.onClick.AddListener(() => sceneManager.SendMessage(methodName));
    }

    static Canvas FindOrCreateCanvas()
    {
        var canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var go = new GameObject("Canvas");
            canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
        }

        var scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        return canvas;
    }

    static void FindOrCreateEventSystem()
    {
        var es = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (es == null)
        {
            var go = new GameObject("EventSystem");
            go.AddComponent<UnityEngine.EventSystems.EventSystem>();
            go.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    static void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }

    static GameObject CreateUIObject(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static void SetTopLeft(RectTransform rt, Vector2 anchoredPosition)
    {
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPosition;
    }

    static TextMeshProUGUI CreateTMPText(string name, Transform parent, string text, float fontSize, FontStyles style, Color color, TextAlignmentOptions alignment)
    {
        var go = CreateUIObject(name, parent);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.color = color;
        tmp.alignment = alignment;
        return tmp;
    }

    static GameObject CreateButton(Transform parent, string name, string text, Color bgColor, Vector2 size, Vector2 anchoredPosition, bool bottomAnchor)
    {
        var go = CreateUIObject(name, parent);
        var img = go.AddComponent<Image>();
        img.color = bgColor;
        var btn = go.AddComponent<Button>();
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        if (bottomAnchor)
        {
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(0f, 0f);
            rt.pivot = new Vector2(0f, 0f);
            rt.anchoredPosition = anchoredPosition;
        }
        else
        {
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = anchoredPosition;
        }

        var label = CreateTMPText("Text", go.transform, text, name == "BackHomeButton" ? 46 : 36, FontStyles.Bold, Color.white, TextAlignmentOptions.Center);
        StretchFull(label.rectTransform);
        return go;
    }
#endif
}
