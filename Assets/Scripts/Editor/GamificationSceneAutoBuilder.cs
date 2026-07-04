using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamificationSceneAutoBuilder
{
    [MenuItem("Tools/AR History/Build Gamification Scene UI")]
    public static void BuildGamificationSceneUI()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "GamificationScene")
        {
            Debug.LogWarning("Hãy mở GamificationScene trước khi chạy script này.");
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

        GameObject bg = CreateUIObject("Background", canvasGO.transform);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.93f, 0.95f, 0.90f, 1f);
        StretchFull(bg.GetComponent<RectTransform>());

        GameObject card = CreateUIObject("MainCard", canvasGO.transform);
        Image cardImg = card.AddComponent<Image>();
        cardImg.color = new Color(1f, 1f, 1f, 0.96f);
        RectTransform cardRT = card.GetComponent<RectTransform>();
        cardRT.sizeDelta = new Vector2(950, 1650);
        cardRT.anchoredPosition = Vector2.zero;

        GameObject title = CreateTMPText("TitleText", card.transform, "Gamification", 54, FontStyles.Bold);
        SetRect(title.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -90), new Vector2(760, 90));

        GameObject instruction = CreateTMPText("InstructionText", card.transform, "Hướng dẫn", 30, FontStyles.Normal);
        SetRect(instruction.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -170), new Vector2(780, 120));

        GameObject selectedToken = CreateTMPText("SelectedTokenText", card.transform, "Đang chọn: chưa có", 28, FontStyles.Bold);
        SetRect(selectedToken.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -250), new Vector2(760, 70));

        GameObject tokenBtn1 = CreateButton(card.transform, "TokenButton1", "Token 1", new Vector2(220, 90), new Vector2(-250, 340));
        GameObject tokenBtn2 = CreateButton(card.transform, "TokenButton2", "Token 2", new Vector2(220, 90), new Vector2(0, 340));
        GameObject tokenBtn3 = CreateButton(card.transform, "TokenButton3", "Token 3", new Vector2(220, 90), new Vector2(250, 340));

        GameObject mapPanel = CreateUIObject("MapPanel", card.transform);
        Image mapImg = mapPanel.AddComponent<Image>();
        mapImg.color = new Color(0.83f, 0.88f, 0.93f, 1f);
        RectTransform mapRT = mapPanel.GetComponent<RectTransform>();
        mapRT.anchorMin = new Vector2(0.5f, 1f);
        mapRT.anchorMax = new Vector2(0.5f, 1f);
        mapRT.pivot = new Vector2(0.5f, 1f);
        mapRT.sizeDelta = new Vector2(760, 650);
        mapRT.anchoredPosition = new Vector2(0, -420);

        GameObject zoneBtn1 = CreateButton(mapPanel.transform, "ZoneButton1", "Zone 1", new Vector2(180, 80), new Vector2(-200, 120));
        GameObject zoneBtn2 = CreateButton(mapPanel.transform, "ZoneButton2", "Zone 2", new Vector2(180, 80), new Vector2(0, 300));
        GameObject zoneBtn3 = CreateButton(mapPanel.transform, "ZoneButton3", "Zone 3", new Vector2(180, 80), new Vector2(220, 500));

        GameObject feedback = CreateTMPText("FeedbackText", card.transform, "Feedback...", 28, FontStyles.Normal);
        TMP_Text fbTMP = feedback.GetComponent<TMP_Text>();
        fbTMP.alignment = TextAlignmentOptions.TopLeft;
        SetRect(feedback.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -1160), new Vector2(760, 160));

        GameObject checkBtn = CreateButton(card.transform, "CheckButton", "Kiểm tra", new Vector2(260, 90), new Vector2(-220, 1400));
        GameObject resetBtn = CreateButton(card.transform, "ResetButton", "Làm lại", new Vector2(260, 90), new Vector2(0, 1400));
        GameObject homeBtn = CreateButton(card.transform, "HomeButton", "Về Home", new Vector2(260, 90), new Vector2(220, 1400));

        GameObject apiGO = new GameObject("GamificationApiManager");
        GamificationApiService api = apiGO.AddComponent<GamificationApiService>();

        GameObject managerGO = new GameObject("GamificationSceneManager");
        GamificationSceneManager manager = managerGO.AddComponent<GamificationSceneManager>();

        manager.titleText = title.GetComponent<TextMeshProUGUI>();
        manager.instructionText = instruction.GetComponent<TextMeshProUGUI>();
        manager.selectedTokenText = selectedToken.GetComponent<TextMeshProUGUI>();
        manager.feedbackText = feedback.GetComponent<TextMeshProUGUI>();

        manager.tokenButton1 = tokenBtn1.GetComponent<Button>();
        manager.tokenButton2 = tokenBtn2.GetComponent<Button>();
        manager.tokenButton3 = tokenBtn3.GetComponent<Button>();

        manager.tokenButtonText1 = tokenBtn1.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        manager.tokenButtonText2 = tokenBtn2.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        manager.tokenButtonText3 = tokenBtn3.transform.Find("Text").GetComponent<TextMeshProUGUI>();

        manager.zoneButton1 = zoneBtn1.GetComponent<Button>();
        manager.zoneButton2 = zoneBtn2.GetComponent<Button>();
        manager.zoneButton3 = zoneBtn3.GetComponent<Button>();

        manager.zoneButtonText1 = zoneBtn1.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        manager.zoneButtonText2 = zoneBtn2.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        manager.zoneButtonText3 = zoneBtn3.transform.Find("Text").GetComponent<TextMeshProUGUI>();

        manager.checkButton = checkBtn.GetComponent<Button>();
        manager.resetButton = resetBtn.GetComponent<Button>();
        manager.homeButton = homeBtn.GetComponent<Button>();
        manager.gamificationApiService = api;

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Đã build xong GamificationScene UI.");
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

        GameObject txt = CreateTMPText("Text", btn.transform, label, 28, FontStyles.Bold);
        TMP_Text tmp = txt.GetComponent<TMP_Text>();
        tmp.color = Color.white;
        StretchFull(txt.GetComponent<RectTransform>());

        return btn;
    }
}