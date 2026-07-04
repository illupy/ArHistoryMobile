using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultSceneAutoBuilder
{
    [MenuItem("Tools/AR History/Build Result Scene UI")]
    public static void BuildResultSceneUI()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "ResultScene")
        {
            Debug.LogWarning("Hãy mở ResultScene trước khi chạy script này.");
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
        bgImg.color = new Color(0.92f, 0.96f, 0.90f, 1f);
        StretchFull(bg.GetComponent<RectTransform>());

        GameObject card = CreateUIObject("MainCard", canvasGO.transform);
        Image cardImg = card.AddComponent<Image>();
        cardImg.color = new Color(1f, 1f, 1f, 0.96f);
        RectTransform cardRT = card.GetComponent<RectTransform>();
        cardRT.sizeDelta = new Vector2(860, 1200);
        cardRT.anchoredPosition = Vector2.zero;

        GameObject title = CreateTMPText("ResultTitleText", card.transform, "Kết quả bài Quiz", 56, FontStyles.Bold);
        SetRect(title.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -130), new Vector2(700, 100));

        GameObject score = CreateTMPText("ScoreText", card.transform, "Điểm: 0/0", 46, FontStyles.Bold);
        SetRect(score.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -290), new Vector2(700, 90));

        GameObject summary = CreateTMPText("SummaryText", card.transform, "Bạn đã hoàn thành bài quiz.", 30, FontStyles.Normal);
        TMP_Text summaryTMP = summary.GetComponent<TMP_Text>();
        summaryTMP.alignment = TextAlignmentOptions.Center;
        SetRect(summary.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -500), new Vector2(700, 180));

        GameObject retryBtn = CreateButton(card.transform, "RetryButton", "Làm lại", new Vector2(420, 100), new Vector2(0, 760));
        GameObject homeBtn = CreateButton(card.transform, "HomeButton", "Về trang chủ", new Vector2(420, 100), new Vector2(0, 900));

        GameObject managerGO = new GameObject("ResultSceneManager");
        ResultSceneManager manager = managerGO.AddComponent<ResultSceneManager>();

        manager.resultTitleText = title.GetComponent<TextMeshProUGUI>();
        manager.scoreText = score.GetComponent<TextMeshProUGUI>();
        manager.summaryText = summary.GetComponent<TextMeshProUGUI>();
        manager.retryButton = retryBtn.GetComponent<Button>();
        manager.homeButton = homeBtn.GetComponent<Button>();

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Đã build xong ResultScene UI.");
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