#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HomeSceneAutoBuilder : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/AR History/Build Home Scene UI")]
    public static void BuildHomeScene()
    {
        var canvas = FindOrCreateCanvas();
        ClearChildren(canvas.transform);

        // Background
        var bg = CreateUIObject("Background", canvas.transform);
        var bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color32(244, 239, 227, 255);
        StretchFull(bg.GetComponent<RectTransform>());

        // Main panel
        var panel = CreateUIObject("MainPanel", canvas.transform);
        var panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color32(255, 255, 255, 235);
        var panelRt = panel.GetComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0.5f, 0.5f);
        panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.pivot = new Vector2(0.5f, 0.5f);
        panelRt.sizeDelta = new Vector2(900, 1450);
        panelRt.anchoredPosition = Vector2.zero;

        // Logo placeholder
        var logoCircle = CreateUIObject("LogoCircle", panel.transform);
        var logoImage = logoCircle.AddComponent<Image>();
        logoImage.color = new Color32(96, 76, 61, 255);
        var logoRt = logoCircle.GetComponent<RectTransform>();
        logoRt.anchorMin = new Vector2(0.5f, 1f);
        logoRt.anchorMax = new Vector2(0.5f, 1f);
        logoRt.pivot = new Vector2(0.5f, 1f);
        logoRt.sizeDelta = new Vector2(140, 140);
        logoRt.anchoredPosition = new Vector2(0, -90);

        var logoText = CreateTMPText("LogoText", logoCircle.transform, "AR", 48, FontStyles.Bold, Color.white, TextAlignmentOptions.Center);
        StretchFull(logoText.rectTransform);

        var title = CreateTMPText("TitleText", panel.transform, "AR History Learning", 70, FontStyles.Bold, new Color32(41, 33, 28, 255), TextAlignmentOptions.Center);
        var titleRt = title.rectTransform;
        titleRt.anchorMin = new Vector2(0.5f, 1f);
        titleRt.anchorMax = new Vector2(0.5f, 1f);
        titleRt.pivot = new Vector2(0.5f, 1f);
        titleRt.sizeDelta = new Vector2(760, 120);
        titleRt.anchoredPosition = new Vector2(0, -260);

        var subtitle = CreateTMPText("SubtitleText", panel.transform, "Khám phá lịch sử bằng công nghệ AR", 34, FontStyles.Normal, new Color32(90, 82, 76, 255), TextAlignmentOptions.Center);
        var subtitleRt = subtitle.rectTransform;
        subtitleRt.anchorMin = new Vector2(0.5f, 1f);
        subtitleRt.anchorMax = new Vector2(0.5f, 1f);
        subtitleRt.pivot = new Vector2(0.5f, 1f);
        subtitleRt.sizeDelta = new Vector2(760, 100);
        subtitleRt.anchoredPosition = new Vector2(0, -370);

        // Illustration placeholder card
        var card = CreateUIObject("IllustrationCard", panel.transform);
        var cardImg = card.AddComponent<Image>();
        cardImg.color = new Color32(236, 227, 211, 255);
        var cardRt = card.GetComponent<RectTransform>();
        cardRt.anchorMin = new Vector2(0.5f, 1f);
        cardRt.anchorMax = new Vector2(0.5f, 1f);
        cardRt.pivot = new Vector2(0.5f, 1f);
        cardRt.sizeDelta = new Vector2(720, 420);
        cardRt.anchoredPosition = new Vector2(0, -520);

        var cardText = CreateTMPText("IllustrationText", card.transform, "Quét marker để bắt đầu bài học AR", 38, FontStyles.Bold, new Color32(96, 76, 61, 255), TextAlignmentOptions.Center);
        StretchFull(cardText.rectTransform);

        // Buttons
        var startBtn = CreateButton(panel.transform, "StartScanButton", "Bắt đầu quét bài học", new Color32(74, 109, 124, 255), new Vector2(620, 120), new Vector2(0, -1040));
        var guideBtn = CreateButton(panel.transform, "GuideButton", "Hướng dẫn sử dụng", new Color32(196, 172, 136, 255), new Vector2(620, 110), new Vector2(0, -1180));

        // Footer note
        var footer = CreateTMPText("FooterText", panel.transform, "Phiên bản demo cho đồ án tốt nghiệp", 26, FontStyles.Italic, new Color32(120, 112, 104, 255), TextAlignmentOptions.Center);
        var footerRt = footer.rectTransform;
        footerRt.anchorMin = new Vector2(0.5f, 0f);
        footerRt.anchorMax = new Vector2(0.5f, 0f);
        footerRt.pivot = new Vector2(0.5f, 0f);
        footerRt.sizeDelta = new Vector2(760, 60);
        footerRt.anchoredPosition = new Vector2(0, 40);

        // Guide Panel
        var guidePanel = CreateUIObject("GuidePanel", canvas.transform);
        var guidePanelImg = guidePanel.AddComponent<Image>();
        guidePanelImg.color = new Color32(20, 20, 20, 180);
        StretchFull(guidePanel.GetComponent<RectTransform>());

        var guideCard = CreateUIObject("GuideCard", guidePanel.transform);
        var guideCardImg = guideCard.AddComponent<Image>();
        guideCardImg.color = new Color32(255, 250, 242, 255);
        var guideCardRt = guideCard.GetComponent<RectTransform>();
        guideCardRt.anchorMin = new Vector2(0.5f, 0.5f);
        guideCardRt.anchorMax = new Vector2(0.5f, 0.5f);
        guideCardRt.pivot = new Vector2(0.5f, 0.5f);
        guideCardRt.sizeDelta = new Vector2(880, 900);
        guideCardRt.anchoredPosition = Vector2.zero;

        var guideTitle = CreateTMPText("GuideTitle", guideCard.transform, "Hướng dẫn sử dụng", 54, FontStyles.Bold, new Color32(41, 33, 28, 255), TextAlignmentOptions.Center);
        var guideTitleRt = guideTitle.rectTransform;
        guideTitleRt.anchorMin = new Vector2(0.5f, 1f);
        guideTitleRt.anchorMax = new Vector2(0.5f, 1f);
        guideTitleRt.pivot = new Vector2(0.5f, 1f);
        guideTitleRt.sizeDelta = new Vector2(760, 100);
        guideTitleRt.anchoredPosition = new Vector2(0, -70);

        var guideContent = CreateTMPText("GuideContent", guideCard.transform,
            "1. Nhấn Bắt đầu quét bài học\n2. Đưa marker vào giữa khung hình\n3. Giữ đủ sáng để camera nhận diện\n4. Quan sát mô hình AR và nghe thuyết minh\n5. Học xong, chọn Quiz hoặc Chơi tương tác nếu có", 34, FontStyles.Normal, new Color32(70, 63, 57, 255), TextAlignmentOptions.TopLeft);
        var guideContentRt = guideContent.rectTransform;
        guideContentRt.anchorMin = new Vector2(0.5f, 0.5f);
        guideContentRt.anchorMax = new Vector2(0.5f, 0.5f);
        guideContentRt.pivot = new Vector2(0.5f, 0.5f);
        guideContentRt.sizeDelta = new Vector2(700, 420);
        guideContentRt.anchoredPosition = new Vector2(0, -20);

        var closeGuideBtn = CreateButton(guideCard.transform, "CloseGuideButton", "Đóng", new Color32(96, 76, 61, 255), new Vector2(300, 100), new Vector2(0, -340));
        guidePanel.SetActive(false);

        // EventSystem
        FindOrCreateEventSystem();

        // HomeManager
        var homeManager = GameObject.Find("HomeManager") ?? new GameObject("HomeManager");
        var controller = homeManager.GetComponent<HomeUIController>() ?? homeManager.AddComponent<HomeUIController>();
        controller.guidePanel = guidePanel;

        // Hook buttons
        var startButtonComp = startBtn.GetComponent<Button>();
        startButtonComp.onClick.RemoveAllListeners();
        startButtonComp.onClick.AddListener(controller.GoToARScan);

        var guideButtonComp = guideBtn.GetComponent<Button>();
        guideButtonComp.onClick.RemoveAllListeners();
        guideButtonComp.onClick.AddListener(controller.OpenGuide);

        var closeButtonComp = closeGuideBtn.GetComponent<Button>();
        closeButtonComp.onClick.RemoveAllListeners();
        closeButtonComp.onClick.AddListener(controller.CloseGuide);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("HomeScene UI built successfully.");
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

    static GameObject CreateButton(Transform parent, string name, string text, Color bgColor, Vector2 size, Vector2 anchoredPosition)
    {
        var go = CreateUIObject(name, parent);
        var img = go.AddComponent<Image>();
        img.color = bgColor;
        var btn = go.AddComponent<Button>();
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = size;
        rt.anchoredPosition = anchoredPosition;

        var label = CreateTMPText("Text", go.transform, text, 40, FontStyles.Bold, Color.white, TextAlignmentOptions.Center);
        StretchFull(label.rectTransform);
        return go;
    }
#endif
}
