using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PrefabInspector
{
    [MenuItem("Tools/AR History/Inspect Button Prefab")]
    public static void Inspect()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogWarning("Canvas not found!");
            return;
        }

        Transform guidePanel = canvas.transform.Find("ScanGuidePanel");
        if (guidePanel == null)
        {
            Debug.LogWarning("ScanGuidePanel not found!");
            return;
        }

        Transform backHome = guidePanel.Find("BackHomeButton");
        if (backHome == null)
        {
            Debug.LogWarning("BackHomeButton not found under ScanGuidePanel!");
            return;
        }

        Debug.LogWarning("=== BACKHOME BUTTON INSPECT ===");
        RectTransform rt = backHome.GetComponent<RectTransform>();
        if (rt != null)
        {
            Debug.LogWarning("sizeDelta: " + rt.sizeDelta);
            Debug.LogWarning("anchoredPosition: " + rt.anchoredPosition);
            Debug.LogWarning("anchorMin: " + rt.anchorMin);
            Debug.LogWarning("anchorMax: " + rt.anchorMax);
            Debug.LogWarning("pivot: " + rt.pivot);
            Debug.LogWarning("localScale: " + rt.localScale);
        }

        Image img = backHome.GetComponent<Image>();
        if (img != null)
        {
            Debug.LogWarning("Image enabled: " + img.enabled);
            Debug.LogWarning("Image type: " + img.type);
            Debug.LogWarning("Image Sprite: " + (img.sprite != null ? img.sprite.name : "null"));
        }
    }

    [MenuItem("Tools/AR History/Toggle Play Mode")]
    public static void TogglePlay()
    {
        EditorApplication.isPlaying = !EditorApplication.isPlaying;
        Debug.LogWarning("Toggled Play Mode. Current isPlaying: " + EditorApplication.isPlaying);
    }

    [MenuItem("Tools/AR History/Pipeline - ARScanScene")]
    public static void PipelineARScanScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ARScanScene.unity");
        ARScanUIEnhancer.EnhanceARScanUI();
        Debug.LogWarning("Pipeline - ARScanScene completed successfully!");
    }

    [MenuItem("Tools/AR History/Pipeline - QuizScene")]
    public static void PipelineQuizScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/QuizScene.unity");
        AnswerButtonRestyler.RestyleButtons();
        QuizUIEnhancer.EnhanceUI();
        Debug.LogWarning("Pipeline - QuizScene completed successfully!");
    }

    [MenuItem("Tools/AR History/Pipeline - ResultScene")]
    public static void PipelineResultScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ResultScene.unity");
        ResultUIEnhancer.EnhanceResultUI();
        Debug.LogWarning("Pipeline - ResultScene completed successfully!");
    }
}