using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class ResultUIEnhancer : EditorWindow
{
    [MenuItem("Tools/AR History/Enhance Result UI")]
    public static void EnhanceResultUI()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "ResultScene")
        {
            Debug.LogWarning("Hãy mở ResultScene trước khi chạy script này.");
            return;
        }

        GameObject mainCard = GameObject.Find("Canvas/MainCard");
        if (mainCard == null)
        {
            Debug.LogError("Không tìm thấy Canvas/MainCard.");
            return;
        }

        GameObject managerGO = GameObject.Find("ResultSceneManager");
        if (managerGO == null)
        {
            Debug.LogError("Không tìm thấy ResultSceneManager.");
            return;
        }

        ResultSceneManager manager = managerGO.GetComponent<ResultSceneManager>();
        if (manager == null)
        {
            Debug.LogError("ResultSceneManager không có script ResultSceneManager.");
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
            bool fontChanged = false;
            if (josefinFont != null)
            {
                if (josefinFont.fallbackFontAssetTable == null)
                    josefinFont.fallbackFontAssetTable = new System.Collections.Generic.List<TMP_FontAsset>();
                if (!josefinFont.fallbackFontAssetTable.Contains(fallbackFont))
                {
                    josefinFont.fallbackFontAssetTable.Add(fallbackFont);
                    EditorUtility.SetDirty(josefinFont);
                    fontChanged = true;
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
                    fontChanged = true;
                }
            }
            if (fontChanged)
            {
                AssetDatabase.SaveAssets();
                Debug.Log("Đã lưu thiết lập Fallback Font cho tiếng Việt.");
            }
        }

        Undo.RegisterCompleteObjectUndo(mainCard, "Enhance Result UI");
        Undo.RegisterCompleteObjectUndo(managerGO, "Enhance Result UI");

        // 1. Cải tiến Background sang hình nền Rừng thần thoại
        GameObject bgGO = GameObject.Find("Canvas/Background");
        if (bgGO != null)
        {
            var img = bgGO.GetComponent<Image>();
            if (img != null)
            {
                string forestBgPath = "Assets/Layer Lab/GUI Pro-CasualGame/ResourcesData/Sprites/Demo/Demo_Background/Background_03.png";
                Sprite forestSprite = AssetDatabase.LoadAssetAtPath<Sprite>(forestBgPath);
                if (forestSprite != null)
                {
                    img.sprite = forestSprite;
                    img.color = Color.white;
                    EditorUtility.SetDirty(img);
                    Debug.Log("Đã gán background rừng thần thoại thành công.");
                }
                else
                {
                    Debug.LogWarning($"Không tải được background rừng tại {forestBgPath}");
                }
            }
        }

        // 2. Cải tiến MainCard sang kính mờ tối (Glassmorphism dark overlay) và nới rộng tỷ lệ
        var cardImg = mainCard.GetComponent<Image>();
        if (cardImg != null)
        {
            string panelPath = "Assets/Mini UI/Panels/Plain Panel/Plain Panel YELLOW GREEN.png";
            Sprite panelSprite = AssetDatabase.LoadAssetAtPath<Sprite>(panelPath);
            if (panelSprite != null)
            {
                cardImg.sprite = panelSprite;
                cardImg.type = Image.Type.Sliced;
                cardImg.color = new Color(0f, 0f, 0f, 0.56f);
                EditorUtility.SetDirty(cardImg);
            }
        }

        RectTransform cardRT = mainCard.GetComponent<RectTransform>();
        if (cardRT != null)
        {
            cardRT.sizeDelta = new Vector2(920f, 1500f);
            cardRT.anchoredPosition = Vector2.zero;
            EditorUtility.SetDirty(cardRT);
        }

        // 3. Cải tiến ResultTitleText
        GameObject titleGO = GameObject.Find("Canvas/MainCard/ResultTitleText");
        if (titleGO != null)
        {
            var tmp = titleGO.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.font = null;
                if (josefinFont != null) tmp.font = josefinFont;
                tmp.fontSize = 56f;
                // Màu vàng Gold sang trọng
                tmp.color = new Color(1.0f, 0.82f, 0.35f, 1f);
                tmp.fontStyle = FontStyles.Bold;
                EditorUtility.SetDirty(tmp);
            }

            var rt = titleGO.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0f, -180f);
                rt.sizeDelta = new Vector2(700f, 100f);
                EditorUtility.SetDirty(rt);
            }
        }

        // 4. Cải tiến ScoreText
        GameObject scoreGO = GameObject.Find("Canvas/MainCard/ScoreText");
        if (scoreGO != null)
        {
            var tmp = scoreGO.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.font = null;
                if (josefinFont != null) tmp.font = josefinFont;
                tmp.fontSize = 48f;
                // Màu xanh dương nhạt ánh bạc
                tmp.color = new Color(0.85f, 0.90f, 1.0f, 1.0f);
                tmp.fontStyle = FontStyles.Bold;
                EditorUtility.SetDirty(tmp);
            }

            var rt = scoreGO.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0f, -380f);
                rt.sizeDelta = new Vector2(700f, 90f);
                EditorUtility.SetDirty(rt);
            }
        }

        // 4.1 Thêm Particle System cho ScoreText (Hiệu ứng tỏa sáng lấp lánh làm nền)
        GameObject oldParticles = GameObject.Find("Canvas/MainCard/ScoreParticles");
        if (oldParticles != null) Undo.DestroyObjectImmediate(oldParticles);

        string particlePrefabPath = "Assets/Layer Lab/GUI Pro-FantasyRPG/Prefabs/Prefabs_DemoScene_Paticle/Particle_Shines_Glow.prefab";
        GameObject particlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(particlePrefabPath);
        if (particlePrefab != null && scoreGO != null)
        {
            GameObject newParticles = (GameObject)PrefabUtility.InstantiatePrefab(particlePrefab, mainCard.transform);
            newParticles.name = "ScoreParticles";
            
            RectTransform pRT = newParticles.GetComponent<RectTransform>();
            if (pRT == null) pRT = newParticles.AddComponent<RectTransform>();
            
            pRT.anchorMin = new Vector2(0.5f, 1f);
            pRT.anchorMax = new Vector2(0.5f, 1f);
            pRT.pivot = new Vector2(0.5f, 0.5f);
            pRT.anchoredPosition = new Vector2(0f, -380f); // Trùng tâm với ScoreText
            pRT.localScale = new Vector3(220f, 220f, 1f); // Phóng to vừa vặn với kích thước UI
            
            // Xếp trước ScoreText một chút để làm nền tỏa sáng tuyệt đẹp phía sau
            int scoreIndex = scoreGO.transform.GetSiblingIndex();
            newParticles.transform.SetSiblingIndex(scoreIndex);

            // Chuyển đổi đệ quy hệ thống hạt để hiển thị trong Canvas Overlay
            ConvertToUIParticles(newParticles);
            
            Debug.Log("Đã tạo hiệu ứng hạt lấp lánh ScoreParticles lung linh phía sau ScoreText.");
        }
        else
        {
            Debug.LogWarning($"Không tải được prefab hạt tại {particlePrefabPath}");
        }

        // 5. Cải tiến SummaryText
        GameObject summaryGO = GameObject.Find("Canvas/MainCard/SummaryText");
        if (summaryGO != null)
        {
            var tmp = summaryGO.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.font = null;
                if (alataFont != null) tmp.font = alataFont;
                tmp.fontSize = 36f;
                // Màu trắng sáng tinh khôi
                tmp.color = new Color(0.96f, 0.97f, 0.98f, 1f);
                tmp.fontStyle = FontStyles.Normal;
                EditorUtility.SetDirty(tmp);
            }

            var rt = summaryGO.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0f, -620f);
                rt.sizeDelta = new Vector2(750f, 240f);
                EditorUtility.SetDirty(rt);
            }
        }

        // 6. Xóa các nút cũ để thay thế bằng phiên bản premium Sliced
        GameObject oldRetry = GameObject.Find("Canvas/MainCard/RetryButton");
        if (oldRetry != null) Undo.DestroyObjectImmediate(oldRetry);

        GameObject oldHome = GameObject.Find("Canvas/MainCard/HomeButton");
        if (oldHome != null) Undo.DestroyObjectImmediate(oldHome);

        // Tải các prefab và sprite nút premium từ GUI Pro-FantasyRPG
        string bluePrefabPath = "Assets/Layer Lab/GUI Pro-FantasyRPG/Prefabs/Prefabs_Component_Buttons/Button_Rectangle01_Blue.prefab";
        string skyPrefabPath = "Assets/Layer Lab/GUI Pro-FantasyRPG/Prefabs/Prefabs_Component_Buttons/Button_Rectangle01_BlueGray.prefab";

        GameObject btnPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(bluePrefabPath);
        GameObject skyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(skyPrefabPath);

        if (btnPrefab != null)
        {
            // A. Tạo RetryButton (Màu xanh lam đậm BLUE)
            GameObject newRetry = (GameObject)PrefabUtility.InstantiatePrefab(btnPrefab, mainCard.transform);
            newRetry.name = "RetryButton";

            Image retryImg = newRetry.GetComponent<Image>();
            if (retryImg != null)
            {
                retryImg.type = Image.Type.Sliced;
                EditorUtility.SetDirty(retryImg);
            }

            RectTransform retryRT = newRetry.GetComponent<RectTransform>();
            retryRT.anchorMin = new Vector2(0.5f, 1f);
            retryRT.anchorMax = new Vector2(0.5f, 1f);
            retryRT.pivot = new Vector2(0.5f, 1f);
            retryRT.sizeDelta = new Vector2(460f, 100f);
            retryRT.anchoredPosition = new Vector2(0f, -1080f);

            Transform retryTextChild = newRetry.transform.Find("Text");
            if (retryTextChild != null)
            {
                var oldText = retryTextChild.GetComponent<Text>();
                if (oldText != null) DestroyImmediate(oldText);

                var tmp = retryTextChild.gameObject.GetComponent<TextMeshProUGUI>();
                if (tmp == null) tmp = retryTextChild.gameObject.AddComponent<TextMeshProUGUI>();
                tmp.text = "Làm lại";
                if (josefinFont != null) tmp.font = josefinFont;
                tmp.fontSize = 32f;
                tmp.fontStyle = FontStyles.Bold;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = Color.white;

                RectTransform tRT = retryTextChild.GetComponent<RectTransform>();
                tRT.anchorMin = Vector2.zero;
                tRT.anchorMax = Vector2.one;
                tRT.offsetMin = Vector2.zero;
                tRT.offsetMax = Vector2.zero;

                manager.retryButton = newRetry.GetComponent<Button>();

                var textPressed = newRetry.GetComponent<TextPressed>();
                if (textPressed != null)
                {
                    textPressed.UnitsToMove = -5f;
                    FieldInfo field = typeof(TextPressed).GetField("gameObject", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null) field.SetValue(textPressed, tRT);
                }
            }

            // B. Tạo HomeButton (Màu xanh trời SKY từ prefab premium tương ứng)
            GameObject newHome = (skyPrefab != null) 
                ? (GameObject)PrefabUtility.InstantiatePrefab(skyPrefab, mainCard.transform)
                : (GameObject)PrefabUtility.InstantiatePrefab(btnPrefab, mainCard.transform);
            newHome.name = "HomeButton";

            var img = newHome.GetComponent<Image>();
            if (img != null)
            {
                img.type = Image.Type.Sliced;
                EditorUtility.SetDirty(img);
            }

            RectTransform homeRT = newHome.GetComponent<RectTransform>();
            homeRT.anchorMin = new Vector2(0.5f, 1f);
            homeRT.anchorMax = new Vector2(0.5f, 1f);
            homeRT.pivot = new Vector2(0.5f, 1f);
            homeRT.sizeDelta = new Vector2(460f, 100f);
            homeRT.anchoredPosition = new Vector2(0f, -1220f);

            Transform homeTextChild = newHome.transform.Find("Text");
            if (homeTextChild != null)
            {
                var oldText = homeTextChild.GetComponent<Text>();
                if (oldText != null) DestroyImmediate(oldText);

                var tmp = homeTextChild.gameObject.GetComponent<TextMeshProUGUI>();
                if (tmp == null) tmp = homeTextChild.gameObject.AddComponent<TextMeshProUGUI>();
                tmp.text = "Về trang chủ";
                if (josefinFont != null) tmp.font = josefinFont;
                tmp.fontSize = 32f;
                tmp.fontStyle = FontStyles.Bold;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = Color.white;

                RectTransform tRT = homeTextChild.GetComponent<RectTransform>();
                tRT.anchorMin = Vector2.zero;
                tRT.anchorMax = Vector2.one;
                tRT.offsetMin = Vector2.zero;
                tRT.offsetMax = Vector2.zero;

                manager.homeButton = newHome.GetComponent<Button>();

                var textPressed = newHome.GetComponent<TextPressed>();
                if (textPressed != null)
                {
                    textPressed.UnitsToMove = -5f;
                    FieldInfo field = typeof(TextPressed).GetField("gameObject", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null) field.SetValue(textPressed, tRT);
                }
            }

            Debug.Log("Đã tạo hai nút RetryButton và HomeButton premium dạng Sliced thành công.");
        }
        else
        {
            Debug.LogError($"Không tải được prefab nút tại {bluePrefabPath}");
        }

        EditorUtility.SetDirty(mainCard);
        EditorUtility.SetDirty(managerGO);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("--- ĐÃ NÂNG CẤP TOÀN BỘ UI & FONT CỦA RESULT SCENE LÊN PREMIUM PHÂN GIẢI CAO ---");
    }

    private static void ConvertToUIParticles(GameObject go)
    {
        ParticleSystem ps = go.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var uiPS = go.GetComponent<UnityEngine.UI.Extensions.CasualGame.UIParticleSystem>();
            if (uiPS == null)
            {
                uiPS = go.AddComponent<UnityEngine.UI.Extensions.CasualGame.UIParticleSystem>();
            }
            // Thiết lập scaling mode là Hierarchy để hạt co giãn đúng tỉ lệ Canvas
            var main = ps.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
        }

        for (int i = 0; i < go.transform.childCount; i++)
        {
            ConvertToUIParticles(go.transform.GetChild(i).gameObject);
        }
    }
}
