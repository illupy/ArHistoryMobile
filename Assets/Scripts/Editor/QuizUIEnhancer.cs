using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class QuizUIEnhancer : EditorWindow
{
    [MenuItem("Tools/AR History/Enhance Quiz UI")]
    public static void EnhanceUI()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "QuizScene")
        {
            Debug.LogWarning("Hãy mở QuizScene trước khi chạy script này.");
            return;
        }

        GameObject mainCard = GameObject.Find("Canvas/MainCard");
        if (mainCard == null)
        {
            Debug.LogError("Không tìm thấy Canvas/MainCard.");
            return;
        }

        GameObject managerGO = GameObject.Find("QuizSceneManager");
        if (managerGO == null)
        {
            Debug.LogError("Không tìm thấy QuizSceneManager.");
            return;
        }

        QuizSceneManager manager = managerGO.GetComponent<QuizSceneManager>();
        if (manager == null)
        {
            Debug.LogError("QuizSceneManager không có script QuizSceneManager.");
            return;
        }

        // Tải Font Assets
        string josefinPath = "Assets/Layer Lab/GUI Pro-FantasyRPG/ResourcesData/Fonts/JosefinSans-Bold SDF.asset";
        string alataPath = "Assets/Layer Lab/GUI Pro-FantasyRPG/ResourcesData/Fonts/Alata-Regular SDF.asset";
        string fallbackPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";
        
        TMP_FontAsset josefinFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(josefinPath);
        TMP_FontAsset alataFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(alataPath);
        TMP_FontAsset fallbackFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fallbackPath);

        if (josefinFont == null) Debug.LogWarning($"Không tải được font Josefin tại {josefinPath}");
        if (alataFont == null) Debug.LogWarning($"Không tải được font Alata tại {alataPath}");
        if (fallbackFont == null) Debug.LogWarning($"Không tải được font LiberationSans tại {fallbackPath}");

        // Cấu hình LiberationSans làm Fallback để hiển thị các ký tự tiếng Việt (đ, ế, ả, ủ,...)
        if (fallbackFont != null)
        {
            if (josefinFont != null)
            {
                if (josefinFont.fallbackFontAssetTable == null)
                    josefinFont.fallbackFontAssetTable = new System.Collections.Generic.List<TMP_FontAsset>();
                if (!josefinFont.fallbackFontAssetTable.Contains(fallbackFont))
                {
                    josefinFont.fallbackFontAssetTable.Add(fallbackFont);
                    EditorUtility.SetDirty(josefinFont);
                }
            }
            if (alataFont != null)
            {
                if (alataFont.fallbackFontAssetTable == null)
                    alataFont.fallbackFontAssetTable = new System.Collections.Generic.List<TMP_FontAsset>();
                if (!alataFont.fallbackFontAssetTable.Contains(fallbackFont))
                {
                    alataFont.fallbackFontAssetTable.Add(fallbackFont);
                    EditorUtility.SetDirty(alataFont);
                }
            }
        }

        Undo.RegisterCompleteObjectUndo(mainCard, "Enhance Quiz UI");
        Undo.RegisterCompleteObjectUndo(managerGO, "Enhance Quiz UI");

        // 1. Cải tiến LessonTitleText
        GameObject titleGO = GameObject.Find("Canvas/MainCard/LessonTitleText");
        if (titleGO != null)
        {
            var tmp = titleGO.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                if (josefinFont != null) tmp.font = josefinFont;
                tmp.fontSize = 52f;
                // Màu vàng Gold sang trọng
                tmp.color = new Color(1.0f, 0.82f, 0.35f, 1f);
                // Bật shadow/outline để chữ nổi bật trên nền tối
                tmp.fontStyle = FontStyles.Bold;
                EditorUtility.SetDirty(tmp);
            }
        }

        // 2. Cải tiến QuestionIndexText
        GameObject indexGO = GameObject.Find("Canvas/MainCard/QuestionIndexText");
        if (indexGO != null)
        {
            var tmp = indexGO.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                if (josefinFont != null) tmp.font = josefinFont;
                tmp.fontSize = 32f;
                // Màu xanh dương nhạt ánh bạc
                tmp.color = new Color(0.85f, 0.90f, 1.0f, 0.85f);
                tmp.fontStyle = FontStyles.Bold;
                EditorUtility.SetDirty(tmp);
            }
        }

        // 3. Cải tiến QuestionText
        GameObject questionGO = GameObject.Find("Canvas/MainCard/Scroll View/Viewport/Content/QuestionText");
        if (questionGO != null)
        {
            var tmp = questionGO.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                if (alataFont != null) tmp.font = alataFont;
                tmp.fontSize = 38f;
                // Màu trắng sáng tinh khôi
                tmp.color = new Color(0.96f, 0.97f, 0.98f, 1f);
                tmp.fontStyle = FontStyles.Normal;
                EditorUtility.SetDirty(tmp);
            }
        }

        // 4. Cải tiến Scroll View Background sang kính mờ tối (Glassmorphism dark overlay)
        GameObject scrollViewGO = GameObject.Find("Canvas/MainCard/Scroll View");
        if (scrollViewGO != null)
        {
            var img = scrollViewGO.GetComponent<Image>();
            if (img != null)
            {
                // Xóa sprite mặc định để có nền phẳng tinh tế
                img.sprite = null;
                // Nền đen mờ 30% cực sang
                img.color = new Color(0f, 0f, 0f, 0.3f);
                EditorUtility.SetDirty(img);
            }
        }

        // 5. Cải tiến Font cho các AnswerButtons
        for (int i = 1; i <= 4; i++)
        {
            GameObject btnGO = GameObject.Find($"Canvas/MainCard/AnswerButton{i}");
            if (btnGO != null)
            {
                Transform textChild = btnGO.transform.Find("Text");
                if (textChild != null)
                {
                    var tmp = textChild.GetComponent<TextMeshProUGUI>();
                    if (tmp != null)
                    {
                        if (josefinFont != null) tmp.font = josefinFont;
                        tmp.fontSize = 32f;
                        tmp.color = Color.white;
                        tmp.fontStyle = FontStyles.Bold;
                        EditorUtility.SetDirty(tmp);
                    }
                }
            }
        }

        // 6. Nâng cấp HomeButton thành nút premium (CYAN) đồng bộ phong cách
        GameObject oldHomeBtn = GameObject.Find("Canvas/MainCard/HomeButton");
        if (oldHomeBtn != null)
        {
            Undo.DestroyObjectImmediate(oldHomeBtn);
        }

        // Tải prefab HomeButton premium từ GUI Pro-FantasyRPG để hiển thị dạng chữ nhật sang trọng
        string skyPrefabPath = "Assets/Layer Lab/GUI Pro-FantasyRPG/Prefabs/Prefabs_Component_Buttons/Button_Rectangle01_BlueGray.prefab";
        
        GameObject homePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(skyPrefabPath);

        if (homePrefab != null)
        {
            GameObject newHome = (GameObject)PrefabUtility.InstantiatePrefab(homePrefab, mainCard.transform);
            newHome.name = "HomeButton";

            // Chuyển chế độ sang Sliced để bo viền hoàn hảo
            var img = newHome.GetComponent<Image>();
            if (img != null)
            {
                img.type = Image.Type.Sliced;
                EditorUtility.SetDirty(img);
            }

            // Thiết lập RectTransform khớp vị trí cũ
            RectTransform rt = newHome.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(420f, 90f);
            rt.anchoredPosition = new Vector2(0f, -1485f);

            // Xử lý Text child
            Transform textChild = newHome.transform.Find("Text");
            if (textChild != null)
            {
                var oldText = textChild.GetComponent<Text>();
                if (oldText != null) DestroyImmediate(oldText);

                var tmp = textChild.gameObject.GetComponent<TextMeshProUGUI>();
                if (tmp == null)
                {
                    tmp = textChild.gameObject.AddComponent<TextMeshProUGUI>();
                }
                tmp.text = "Về trang chủ";
                if (josefinFont != null) tmp.font = josefinFont;
                tmp.fontSize = 30f;
                tmp.fontStyle = FontStyles.Bold;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = Color.white;

                // Stretch Full
                RectTransform textRT = textChild.GetComponent<RectTransform>();
                textRT.anchorMin = Vector2.zero;
                textRT.anchorMax = Vector2.one;
                textRT.offsetMin = Vector2.zero;
                textRT.offsetMax = Vector2.zero;

                manager.homeButton = newHome.GetComponent<Button>();
                
                // Cấu hình TextPressed
                var textPressed = newHome.GetComponent<TextPressed>();
                if (textPressed != null)
                {
                    textPressed.UnitsToMove = -5f;
                    FieldInfo field = typeof(TextPressed).GetField("gameObject", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                    {
                        field.SetValue(textPressed, textRT);
                    }
                }
            }
            Debug.Log("Đã tạo HomeButton premium SKY-BLUE mới dạng Sliced.");
        }
        else
        {
            Debug.LogError($"Không tải được prefab HomeButton tại {skyPrefabPath}");
        }

        EditorUtility.SetDirty(mainCard);
        EditorUtility.SetDirty(managerGO);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("--- ĐÃ NÂNG CẤP TOÀN BỘ UI & FONT CỦA QUIZ SCENE LÊN PREMIUM PHÂN GIẢI CAO ---");
    }
}
