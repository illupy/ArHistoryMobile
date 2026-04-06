using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HomeSceneAutoBuilder
{
    [MenuItem("Tools/AR History/Build Home Scene UI")]
    public static void BuildHomeSceneUI()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "HomeScene")
        {
            Debug.LogWarning("Hãy mở HomeScene trước khi chạy script này.");
            return;
        }

        Canvas existingCanvas = Object.FindFirstObjectByType<Canvas>();
        if (existingCanvas != null)
        {
            Debug.LogWarning("Scene đã có Canvas. Xóa Canvas cũ nếu muốn build lại.");
            return;
        }

        // Canvas
        GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        // EventSystem
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem",
                typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        // Background
        GameObject bg = CreateUIObject("Background", canvasGO.transform);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.96f, 0.93f, 0.85f, 1f);
        StretchFull(bg.GetComponent<RectTransform>());

        // MainPanel
        GameObject mainPanel = CreateUIObject("MainPanel", canvasGO.transform);
        Image panelImage = mainPanel.AddComponent<Image>();
        panelImage.color = new Color(1f, 1f, 1f, 0.92f);
        RectTransform panelRT = mainPanel.GetComponent<RectTransform>();
        panelRT.sizeDelta = new Vector2(900, 1400);
        panelRT.anchoredPosition = Vector2.zero;

        // Title
        GameObject titleGO = CreateTMPText("TitleText", mainPanel.transform, "AR History Learning", 72, FontStyles.Bold);
        SetRect(titleGO.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -170), new Vector2(800, 120));

        // Subtitle
        GameObject subGO = CreateTMPText("SubtitleText", mainPanel.transform, "Khám phá lịch sử bằng công nghệ AR", 36, FontStyles.Normal);
        SetRect(subGO.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -270), new Vector2(760, 100));

        // Start Button
        GameObject startBtn = CreateButton(mainPanel.transform, "StartScanButton", "Bắt đầu quét bài học", new Vector2(600, 120), new Vector2(0, -900));
        // Guide Button
        GameObject guideBtn = CreateButton(mainPanel.transform, "GuideButton", "Hướng dẫn sử dụng", new Vector2(600, 100), new Vector2(0, -1050));

        // Guide Panel
        GameObject guidePanel = CreateUIObject("GuidePanel", canvasGO.transform);
        Image guidePanelImage = guidePanel.AddComponent<Image>();
        guidePanelImage.color = new Color(0f, 0f, 0f, 0.65f);
        StretchFull(guidePanel.GetComponent<RectTransform>());

        GameObject guideCard = CreateUIObject("GuideCard", guidePanel.transform);
        Image guideCardImage = guideCard.AddComponent<Image>();
        guideCardImage.color = new Color(1f, 1f, 1f, 0.97f);
        RectTransform guideCardRT = guideCard.GetComponent<RectTransform>();
        guideCardRT.sizeDelta = new Vector2(860, 1100);
        guideCardRT.anchoredPosition = Vector2.zero;

        GameObject guideTitle = CreateTMPText("GuideTitle", guideCard.transform, "Hướng dẫn sử dụng", 56, FontStyles.Bold);
        SetRect(guideTitle.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -120), new Vector2(700, 100));

        string guideText =
            "1. Nhấn \"Bắt đầu quét bài học\".\n" +
            "2. Đưa ảnh marker vào giữa khung hình camera.\n" +
            "3. Khi quét thành công, ứng dụng sẽ hiển thị nội dung AR preview.\n" +
            "4. Nghe storytelling mở đầu và chọn bắt đầu bài học.\n" +
            "5. Hoàn thành bài học, sau đó làm quiz hoặc chơi tương tác nếu có.";

        GameObject guideContent = CreateTMPText("GuideContent", guideCard.transform, guideText, 34, FontStyles.Normal);
        TMP_Text guideContentTMP = guideContent.GetComponent<TMP_Text>();
        guideContentTMP.alignment = TextAlignmentOptions.TopLeft;
        SetRect(guideContent.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -430), new Vector2(720, 520));

        GameObject closeBtn = CreateButton(guideCard.transform, "CloseButton", "Đóng", new Vector2(360, 100), new Vector2(0, 420));

        guidePanel.SetActive(false);

        // Manager
        GameObject manager = new GameObject("HomeManager");
        var controller = manager.AddComponent<HomeUIController>();
        controller.guidePanel = guidePanel;

        // Hook buttons
        AddButtonEvent(startBtn, manager, "GoToARScan");
        AddButtonEvent(guideBtn, manager, "OpenGuide");
        AddButtonEvent(closeBtn, manager, "CloseGuide");

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Đã build xong HomeScene UI.");
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
        tmp.color = new Color(0.15f, 0.18f, 0.23f, 1f);
        return go;
    }

    private static GameObject CreateButton(Transform parent, string name, string label, Vector2 size, Vector2 pos)
    {
        GameObject btn = CreateUIObject(name, parent);
        Image img = btn.AddComponent<Image>();
        img.color = new Color(0.17f, 0.36f, 0.62f, 1f);
        Button button = btn.AddComponent<Button>();

        RectTransform rt = btn.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;

        GameObject txt = CreateTMPText("Text", btn.transform, label, 38, FontStyles.Bold);
        TMP_Text tmp = txt.GetComponent<TMP_Text>();
        tmp.color = Color.white;
        StretchFull(txt.GetComponent<RectTransform>());

        return btn;
    }

    private static void AddButtonEvent(GameObject buttonObj, GameObject targetObj, string methodName)
    {
        Button btn = buttonObj.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, () => { });
        btn.onClick.RemoveAllListeners();

        var so = new SerializedObject(btn);
        var onClick = so.FindProperty("m_OnClick.m_PersistentCalls.m_Calls");
        onClick.arraySize++;
        var call = onClick.GetArrayElementAtIndex(onClick.arraySize - 1);
        call.FindPropertyRelative("m_Target").objectReferenceValue = targetObj.GetComponent(methodName == "GoToARScan" || methodName == "OpenGuide" || methodName == "CloseGuide"
            ? typeof(HomeUIController)
            : typeof(MonoBehaviour));
        call.FindPropertyRelative("m_MethodName").stringValue = methodName;
        call.FindPropertyRelative("m_Mode").intValue = 1;
        so.ApplyModifiedProperties();
    }
}