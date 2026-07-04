using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Core.UI.Editor
{
    public class SafeAreaTool : UnityEditor.EditorWindow
    {
        [MenuItem("Tools/Setup Safe Area for Active Scene")]
        public static void SetupSafeArea()
        {
            Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
            if (canvases.Length == 0)
            {
                Debug.LogWarning("No Canvas found in the active scene!");
                return;
            }

            foreach (Canvas canvas in canvases)
            {
                // Only process root canvases
                if (canvas.transform.parent != null) continue;

                foreach (Transform panel in canvas.transform)
                {
                    // Skip if not a panel or doesn't need safe area (heuristic: active/inactive panels)
                    // We check if it's a RectTransform
                    if (panel.GetComponent<RectTransform>() == null) continue;

                    // If the panel already has a SafeArea script or container, skip or continue
                    if (panel.GetComponent<Core.UI.SafeArea>() != null) continue;

                    Transform safeAreaContainer = panel.Find("SafeAreaContainer");
                    if (safeAreaContainer == null)
                    {
                        GameObject saGo = new GameObject("SafeAreaContainer", typeof(RectTransform));
                        Undo.RegisterCreatedObjectUndo(saGo, "Create SafeArea Container");
                        
                        saGo.transform.SetParent(panel, false);
                        safeAreaContainer = saGo.transform;
                        
                        RectTransform saRect = safeAreaContainer.GetComponent<RectTransform>();
                        saRect.anchorMin = Vector2.zero;
                        saRect.anchorMax = Vector2.one;
                        saRect.sizeDelta = Vector2.zero;
                        saRect.anchoredPosition = Vector2.zero;
                        
                        saGo.AddComponent<Core.UI.SafeArea>();
                    }

                    // Move children into SafeAreaContainer
                    // We need to collect children first, because moving them changes childCount
                    int childCount = panel.childCount;
                    Transform[] childrenToMove = new Transform[childCount];
                    for (int i = 0; i < childCount; i++)
                    {
                        childrenToMove[i] = panel.GetChild(i);
                    }

                    foreach (Transform child in childrenToMove)
                    {
                        if (child == safeAreaContainer) continue;
                        
                        // Skip backgrounds
                        string lowerName = child.name.ToLower();
                        if (lowerName == "bg" || lowerName == "background" || lowerName.Contains("bg_")) continue;

                        Undo.SetTransformParent(child, safeAreaContainer, "Move to SafeAreaContainer");
                    }
                }
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>Safe Area setup completed successfully!</color>");
        }

        [MenuItem("Tools/Undo Safe Area for Active Scene")]
        public static void UndoSafeArea()
        {
            Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                if (canvas.transform.parent != null) continue;

                foreach (Transform panel in canvas.transform)
                {
                    Transform safeAreaContainer = panel.Find("SafeAreaContainer");
                    if (safeAreaContainer != null)
                    {
                        // Move children back to panel
                        int childCount = safeAreaContainer.childCount;
                        Transform[] childrenToMove = new Transform[childCount];
                        for (int i = 0; i < childCount; i++)
                        {
                            childrenToMove[i] = safeAreaContainer.GetChild(i);
                        }

                        foreach (Transform child in childrenToMove)
                        {
                            Undo.SetTransformParent(child, panel, "Move back from SafeAreaContainer");
                        }

                        // Delete the container
                        Undo.DestroyObjectImmediate(safeAreaContainer.gameObject);
                    }
                }
            }
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("<color=yellow>Safe Area reverted successfully!</color>");
        }
    }
}
