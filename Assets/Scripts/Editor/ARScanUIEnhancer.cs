using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class ARScanUIEnhancer : EditorWindow
{
    [MenuItem("Tools/AR History/Enhance ARScan UI")]
    public static void EnhanceARScanUI()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "ARScanScene")
        {
            Debug.LogWarning("Hãy mở ARScanScene trước khi chạy script này.");
            return;
        }

        GameObject previewPanel = GameObject.Find("Canvas/PreviewPanel");
        if (previewPanel == null)
        {
            Debug.LogError("Không tìm thấy Canvas/PreviewPanel.");
            return;
        }

        GameObject managerGO = GameObject.Find("ARPreviewSceneManager");
        if (managerGO == null)
        {
            Debug.LogError("Không tìm thấy ARPreviewSceneManager.");
            return;
        }

        ARPreviewSceneManager manager = managerGO.GetComponent<ARPreviewSceneManager>();
        if (manager == null)
        {
            Debug.LogError("Không tìm thấy ARPreviewSceneManager component.");
            return;
        }

        PreviewPanelController controller = manager.previewPanelController;
        if (controller == null)
        {
            Debug.LogError("Không tìm thấy PreviewPanelController trên manager.");
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

        // Cấu hình LiberationSans làm Fallback để hiển thị các ký tự tiếng Việt có dấu
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
            }
        }

        Undo.RegisterCompleteObjectUndo(previewPanel, "Enhance ARScan UI");
        Undo.RegisterCompleteObjectUndo(controller, "Enhance ARScan UI");
        Undo.RegisterCompleteObjectUndo(managerGO, "Enhance ARScan UI");

        string bluePurplePrefab = "Assets/Layer Lab/GUI Pro-FantasyRPG/Prefabs/Prefabs_Component_Buttons/Button_Rectangle01_Purple.prefab";
        string skyPrefab = "Assets/Layer Lab/GUI Pro-FantasyRPG/Prefabs/Prefabs_Component_Buttons/Button_Rectangle01_BlueGray.prefab";
        string greyPrefab = "Assets/Layer Lab/GUI Pro-FantasyRPG/Prefabs/Prefabs_Component_Buttons/Button_Rectangle01_Gray.prefab";
        string orangePrefab = "Assets/Layer Lab/GUI Pro-FantasyRPG/Prefabs/Prefabs_Component_Buttons/Button_Rectangle01_Yellow.prefab";

        GameObject startPref = AssetDatabase.LoadAssetAtPath<GameObject>(bluePurplePrefab);
        GameObject skipPref = AssetDatabase.LoadAssetAtPath<GameObject>(skyPrefab);
        GameObject detachPref = AssetDatabase.LoadAssetAtPath<GameObject>(greyPrefab);
        GameObject resetPref = AssetDatabase.LoadAssetAtPath<GameObject>(orangePrefab);

        // --- 1. NÂNG CẤP PREVIEWPANEL (KHUNG CHỨA XEM TRƯỚC BÀI HỌC) ---
        var panelImg = previewPanel.GetComponent<Image>();
        if (panelImg != null)
        {
            string panelPath = "Assets/Mini UI/Panels/Plain Panel/Plain Panel BLUE PURPLE.png";
            Sprite panelSprite = AssetDatabase.LoadAssetAtPath<Sprite>(panelPath);
            if (panelSprite != null)
            {
                panelImg.sprite = panelSprite;
                panelImg.type = Image.Type.Sliced;
                panelImg.color = new Color(0f, 0f, 0f, 0.72f); // Màu đen mờ 72%
                EditorUtility.SetDirty(panelImg);
            }
        }

        RectTransform panelRT = previewPanel.GetComponent<RectTransform>();
        if (panelRT != null)
        {
            panelRT.anchorMin = new Vector2(0.5f, 0f);
            panelRT.anchorMax = new Vector2(0.5f, 0f);
            panelRT.pivot = new Vector2(0.5f, 0f);
            panelRT.anchoredPosition = new Vector2(0f, 40f);
            panelRT.sizeDelta = new Vector2(960f, 720f);
            EditorUtility.SetDirty(panelRT);
        }

        GameObject titleGO = GameObject.Find("Canvas/PreviewPanel/LessonTitleText");
        if (titleGO == null) titleGO = GameObject.Find("LessonTitleText");
        if (titleGO != null)
        {
            var tmp = titleGO.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.font = null;
                if (josefinFont != null) tmp.font = josefinFont;
                tmp.fontSize = 48f;
                tmp.color = new Color(1.0f, 0.82f, 0.35f, 1f); // Vàng Gold
                tmp.fontStyle = FontStyles.Bold;
                tmp.alignment = TextAlignmentOptions.Center;
                EditorUtility.SetDirty(tmp);
            }

            var rt = titleGO.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0f, -80f);
                rt.sizeDelta = new Vector2(860f, 80f);
                EditorUtility.SetDirty(rt);
            }
        }

        GameObject textGO = GameObject.Find("Canvas/PreviewPanel/PreviewText");
        if (textGO == null) textGO = GameObject.Find("PreviewText");
        if (textGO != null)
        {
            var tmp = textGO.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.font = null;
                if (alataFont != null) tmp.font = alataFont;
                tmp.fontSize = 30f;
                tmp.color = new Color(0.96f, 0.97f, 0.98f, 1f);
                tmp.fontStyle = FontStyles.Normal;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.textWrappingMode = TextWrappingModes.Normal;
                EditorUtility.SetDirty(tmp);
            }

            var rt = textGO.GetComponent<RectTransform>();
            if (rt != null && (rt.parent == null || rt.parent.name != "Content"))
            {
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0f, -240f);
                rt.sizeDelta = new Vector2(860f, 180f);
                EditorUtility.SetDirty(rt);
            }
        }

        GameObject oldStart = GameObject.Find("Canvas/PreviewPanel/StartLessonButton");
        GameObject oldSkip = GameObject.Find("Canvas/PreviewPanel/SkipVoiceButton");
        GameObject oldDetach = GameObject.Find("Canvas/PreviewPanel/DetachButton");
        GameObject oldReset = GameObject.Find("Canvas/PreviewPanel/ResetModelButton");

        if (oldStart != null) Undo.DestroyObjectImmediate(oldStart);
        if (oldSkip != null) Undo.DestroyObjectImmediate(oldSkip);
        if (oldDetach != null) Undo.DestroyObjectImmediate(oldDetach);
        if (oldReset != null) Undo.DestroyObjectImmediate(oldReset);

        if (startPref != null && skipPref != null && detachPref != null && resetPref != null)
        {
            // A. Tạo StartLessonButton (BLUE PURPLE)
            GameObject newStart = (GameObject)PrefabUtility.InstantiatePrefab(startPref, previewPanel.transform);
            newStart.name = "StartLessonButton";
            SetupPremiumButton(newStart, "Bắt đầu bài học", josefinFont, new Vector2(-230f, -460f));
            Button startBtn = newStart.GetComponent<Button>();
            controller.startLessonButton = newStart;
            UnityEditor.Events.UnityEventTools.AddPersistentListener(startBtn.onClick, manager.StartLesson);
            EditorUtility.SetDirty(startBtn);

            // B. Tạo SkipVoiceButton (SKYBLUE)
            GameObject newSkip = (GameObject)PrefabUtility.InstantiatePrefab(skipPref, previewPanel.transform);
            newSkip.name = "SkipVoiceButton";
            SetupPremiumButton(newSkip, "Bỏ qua voice", josefinFont, new Vector2(230f, -460f));
            Button skipBtn = newSkip.GetComponent<Button>();
            controller.skipVoiceButton = newSkip;
            UnityEditor.Events.UnityEventTools.AddPersistentListener(skipBtn.onClick, manager.OnPreviewVoiceCompleted);
            EditorUtility.SetDirty(skipBtn);

            // C. Tạo DetachButton (GREY)
            GameObject newDetach = (GameObject)PrefabUtility.InstantiatePrefab(detachPref, previewPanel.transform);
            newDetach.name = "DetachButton";
            SetupPremiumButton(newDetach, "Tách mô hình", josefinFont, new Vector2(-230f, -590f));
            Button detachBtn = newDetach.GetComponent<Button>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(detachBtn.onClick, manager.ToggleDetachModel);
            EditorUtility.SetDirty(detachBtn);

            // D. Tạo ResetModelButton (ORANGE)
            GameObject newReset = (GameObject)PrefabUtility.InstantiatePrefab(resetPref, previewPanel.transform);
            newReset.name = "ResetModelButton";
            SetupPremiumButton(newReset, "Đặt lại mô hình", josefinFont, new Vector2(230f, -590f));
            Button resetBtn = newReset.GetComponent<Button>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(resetBtn.onClick, manager.ResetModelToMarker);
            EditorUtility.SetDirty(resetBtn);
        }

        // --- 2. NÂNG CẤP LESSONPANEL (KHUNG BÀI HỌC TỪNG BƯỚC) ---
        GameObject lessonPanel = GameObject.Find("Canvas/LessonPanel");
        if (lessonPanel != null)
        {
            var panelImg2 = lessonPanel.GetComponent<Image>();
            if (panelImg2 != null)
            {
                panelImg2.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Mini UI/Panels/Plain Panel/Plain Panel BLUE PURPLE.png");
                panelImg2.type = Image.Type.Sliced;
                panelImg2.color = new Color(0f, 0f, 0f, 0.72f);
                EditorUtility.SetDirty(panelImg2);
            }

            RectTransform lRT = lessonPanel.GetComponent<RectTransform>();
            if (lRT != null)
            {
                lRT.anchorMin = new Vector2(0.5f, 0f);
                lRT.anchorMax = new Vector2(0.5f, 0f);
                lRT.pivot = new Vector2(0.5f, 0f);
                lRT.anchoredPosition = new Vector2(0f, 40f);
                lRT.sizeDelta = new Vector2(960f, 720f);
                EditorUtility.SetDirty(lRT);
            }

            GameObject lessonTitleGO = GameObject.Find("Canvas/LessonPanel/Header/LessonTitleText");
            if (lessonTitleGO != null)
            {
                var tmp = lessonTitleGO.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.font = null;
                    if (josefinFont != null) tmp.font = josefinFont;
                    tmp.fontSize = 40f;
                    tmp.color = new Color(1.0f, 0.82f, 0.35f, 1f); // Gold
                    tmp.fontStyle = FontStyles.Bold;
                    tmp.alignment = TextAlignmentOptions.Center;
                    EditorUtility.SetDirty(tmp);
                }
            }

            GameObject stepIndexGO = GameObject.Find("Canvas/LessonPanel/Header/StepIndexText");
            if (stepIndexGO != null)
            {
                var tmp = stepIndexGO.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.font = null;
                    if (alataFont != null) tmp.font = alataFont;
                    tmp.fontSize = 28f;
                    tmp.color = new Color(0.9f, 0.9f, 0.95f, 1f);
                    tmp.alignment = TextAlignmentOptions.Center;
                    EditorUtility.SetDirty(tmp);
                }
            }

            GameObject stepTypeGO = GameObject.Find("Canvas/LessonPanel/Header/StepTypeText");
            if (stepTypeGO != null)
            {
                var tmp = stepTypeGO.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.font = null;
                    if (alataFont != null) tmp.font = alataFont;
                    tmp.fontSize = 24f;
                    tmp.color = new Color(0.35f, 0.85f, 1.0f, 1f); // Cyan
                    tmp.alignment = TextAlignmentOptions.Center;
                    EditorUtility.SetDirty(tmp);
                }
            }

            GameObject textPanel = GameObject.Find("Canvas/LessonPanel/ContentArea/TextPanel");
            if (textPanel != null)
            {
                var tpImg = textPanel.GetComponent<Image>();
                if (tpImg != null)
                {
                    tpImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Mini UI/Panels/Plain Panel/Plain Panel LIGHT DARK.png");
                    tpImg.type = Image.Type.Sliced;
                    tpImg.color = new Color(0f, 0f, 0f, 0.5f); // Kính mờ lồng
                    EditorUtility.SetDirty(tpImg);
                }

                // Add ScrollRect and RectMask2D for scrollable text
                ScrollRect scrollRect = textPanel.GetComponent<ScrollRect>();
                if (scrollRect == null) scrollRect = textPanel.AddComponent<ScrollRect>();
                scrollRect.horizontal = false;
                scrollRect.vertical = true;
                scrollRect.movementType = ScrollRect.MovementType.Elastic;
                scrollRect.elasticity = 0.1f;
                scrollRect.scrollSensitivity = 25f;
                EditorUtility.SetDirty(scrollRect);

                RectMask2D mask = textPanel.GetComponent<RectMask2D>();
                if (mask == null) mask = textPanel.AddComponent<RectMask2D>();
                EditorUtility.SetDirty(mask);
            }

            GameObject stepContentGO = GameObject.Find("Canvas/LessonPanel/ContentArea/TextPanel/StepContentText");
            if (stepContentGO != null)
            {
                var tmp = stepContentGO.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.font = null;
                    if (alataFont != null) tmp.font = alataFont;
                    tmp.fontSize = 28f;
                    tmp.color = new Color(0.96f, 0.97f, 0.98f, 1f);
                    tmp.alignment = TextAlignmentOptions.Top; // Top aligned for better scrolling start
                    tmp.textWrappingMode = TextWrappingModes.Normal;
                    EditorUtility.SetDirty(tmp);
                }

                RectTransform contentRT = stepContentGO.GetComponent<RectTransform>();
                if (contentRT != null)
                {
                    contentRT.anchorMin = new Vector2(0f, 1f); // Stretch horizontal, anchor top
                    contentRT.anchorMax = new Vector2(1f, 1f);
                    contentRT.pivot = new Vector2(0.5f, 1f);   // Pivot at top-center
                    contentRT.anchoredPosition = new Vector2(0f, -40f); // Top margin 40
                    contentRT.sizeDelta = new Vector2(-80f, 0f); // 40px left & right padding
                    EditorUtility.SetDirty(contentRT);
                }

                ContentSizeFitter fitter = stepContentGO.GetComponent<ContentSizeFitter>();
                if (fitter == null) fitter = stepContentGO.AddComponent<ContentSizeFitter>();
                fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                EditorUtility.SetDirty(fitter);

                if (textPanel != null)
                {
                    ScrollRect scrollRect = textPanel.GetComponent<ScrollRect>();
                    if (scrollRect != null)
                    {
                        scrollRect.content = contentRT;
                        EditorUtility.SetDirty(scrollRect);
                    }
                }
            }


            GameObject oldPrev = GameObject.Find("Canvas/LessonPanel/PreviousButton");
            GameObject oldNext = GameObject.Find("Canvas/LessonPanel/NextButton");

            if (oldPrev != null) Undo.DestroyObjectImmediate(oldPrev);
            if (oldNext != null) Undo.DestroyObjectImmediate(oldNext);

            GameObject prevPref = AssetDatabase.LoadAssetAtPath<GameObject>(greyPrefab);
            GameObject nextPref = AssetDatabase.LoadAssetAtPath<GameObject>(bluePurplePrefab);

            if (prevPref != null && nextPref != null)
            {
                // Quay lại (GREY)
                GameObject newPrev = (GameObject)PrefabUtility.InstantiatePrefab(prevPref, lessonPanel.transform);
                newPrev.name = "PreviousButton";
                SetupPremiumButton(newPrev, "Quay lại", josefinFont, new Vector2(-230f, -590f));
                Button prevBtn = newPrev.GetComponent<Button>();
                UnityEditor.Events.UnityEventTools.AddPersistentListener(prevBtn.onClick, manager.PreviousStep);
                EditorUtility.SetDirty(prevBtn);

                // Tiếp theo (BLUE PURPLE)
                GameObject newNext = (GameObject)PrefabUtility.InstantiatePrefab(nextPref, lessonPanel.transform);
                newNext.name = "NextButton";
                SetupPremiumButton(newNext, "Tiếp theo", josefinFont, new Vector2(230f, -590f));
                Button nextBtn = newNext.GetComponent<Button>();
                UnityEditor.Events.UnityEventTools.AddPersistentListener(nextBtn.onClick, manager.NextStep);
                EditorUtility.SetDirty(nextBtn);
            }
        }

        // --- 3. NÂNG CẤP COMPLETIONPANEL (KHUNG BÁO HOÀN THÀNH HỌC) ---
        GameObject completionPanel = GameObject.Find("Canvas/CompletionPanel");
        if (completionPanel != null)
        {
            var panelImg3 = completionPanel.GetComponent<Image>();
            if (panelImg3 != null)
            {
                panelImg3.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Mini UI/Panels/Plain Panel/Plain Panel BLUE PURPLE.png");
                panelImg3.type = Image.Type.Sliced;
                panelImg3.color = new Color(0f, 0f, 0f, 0.85f); // Đậm màu
                EditorUtility.SetDirty(panelImg3);
            }

            RectTransform cRT = completionPanel.GetComponent<RectTransform>();
            if (cRT != null)
            {
                cRT.anchorMin = new Vector2(0.5f, 0.5f);
                cRT.anchorMax = new Vector2(0.5f, 0.5f);
                cRT.pivot = new Vector2(0.5f, 0.5f);
                cRT.anchoredPosition = Vector2.zero;
                cRT.sizeDelta = new Vector2(960f, 1100f);
                EditorUtility.SetDirty(cRT);
            }

            GameObject cardGO = GameObject.Find("Canvas/CompletionPanel/CompletionCard");
            if (cardGO != null)
            {
                var cardImg = cardGO.GetComponent<Image>();
                if (cardImg != null)
                {
                    cardImg.color = Color.clear;
                    EditorUtility.SetDirty(cardImg);
                }

                RectTransform cardRT = cardGO.GetComponent<RectTransform>();
                if (cardRT != null)
                {
                    cardRT.anchorMin = Vector2.zero;
                    cardRT.anchorMax = Vector2.one;
                    cardRT.offsetMin = Vector2.zero;
                    cardRT.offsetMax = Vector2.zero;
                    EditorUtility.SetDirty(cardRT);
                }

                GameObject titleC = GameObject.Find("Canvas/CompletionPanel/CompletionCard/TitleText");
                if (titleC != null)
                {
                    var tmp = titleC.GetComponent<TextMeshProUGUI>();
                    if (tmp != null)
                    {
                        tmp.font = null;
                        if (josefinFont != null) tmp.font = josefinFont;
                        tmp.fontSize = 52f;
                        tmp.color = new Color(1.0f, 0.82f, 0.35f, 1f); // Gold
                        tmp.fontStyle = FontStyles.Bold;
                        tmp.text = "Bài học hoàn thành!";
                        tmp.alignment = TextAlignmentOptions.Center;
                        EditorUtility.SetDirty(tmp);
                    }

                    var rt = titleC.GetComponent<RectTransform>();
                    if (rt != null)
                    {
                        rt.anchorMin = new Vector2(0.5f, 1f);
                        rt.anchorMax = new Vector2(0.5f, 1f);
                        rt.pivot = new Vector2(0.5f, 0.5f);
                        rt.anchoredPosition = new Vector2(0f, -180f);
                        rt.sizeDelta = new Vector2(860f, 100f);
                        EditorUtility.SetDirty(rt);
                    }
                }

                GameObject oldQuiz = GameObject.Find("Canvas/CompletionPanel/CompletionCard/QuizButton");
                GameObject oldGami = GameObject.Find("Canvas/CompletionPanel/CompletionCard/GamificationButton");
                GameObject oldHome = GameObject.Find("Canvas/CompletionPanel/CompletionCard/HomeButton");

                if (oldQuiz != null) Undo.DestroyObjectImmediate(oldQuiz);
                if (oldGami != null) Undo.DestroyObjectImmediate(oldGami);
                if (oldHome != null) Undo.DestroyObjectImmediate(oldHome);

                GameObject quizPref = AssetDatabase.LoadAssetAtPath<GameObject>(bluePurplePrefab);
                GameObject gamiPref = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Layer Lab/GUI Pro-FantasyRPG/Prefabs/Prefabs_Component_Buttons/Button_Rectangle01_Green.prefab");
                GameObject homePref = AssetDatabase.LoadAssetAtPath<GameObject>(skyPrefab);

                if (quizPref != null && gamiPref != null && homePref != null)
                {
                    CompletionPanelController compController = completionPanel.GetComponent<CompletionPanelController>();

                    // Quiz
                    GameObject newQuiz = (GameObject)PrefabUtility.InstantiatePrefab(quizPref, cardGO.transform);
                    newQuiz.name = "QuizButton";
                    SetupPremiumButton(newQuiz, "Làm bài tập Quiz", josefinFont, new Vector2(0f, -440f));
                    Button quizBtn = newQuiz.GetComponent<Button>();
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(quizBtn.onClick, manager.GoToQuiz);
                    if (compController != null) compController.quizButton = newQuiz;
                    EditorUtility.SetDirty(quizBtn);

                    // Gamification
                    GameObject newGami = (GameObject)PrefabUtility.InstantiatePrefab(gamiPref, cardGO.transform);
                    newGami.name = "GamificationButton";
                    SetupPremiumButton(newGami, "Trò chơi ôn tập", josefinFont, new Vector2(0f, -590f));
                    Button gamiBtn = newGami.GetComponent<Button>();
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(gamiBtn.onClick, manager.GoToGamification);
                    if (compController != null) compController.gamificationButton = newGami;
                    EditorUtility.SetDirty(gamiBtn);

                    // Home
                    GameObject newHome = (GameObject)PrefabUtility.InstantiatePrefab(homePref, cardGO.transform);
                    newHome.name = "HomeButton";
                    SetupPremiumButton(newHome, "Về trang chủ", josefinFont, new Vector2(0f, -740f));
                    Button homeBtn = newHome.GetComponent<Button>();
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(homeBtn.onClick, manager.GoHome);
                    EditorUtility.SetDirty(homeBtn);
                }
            }
        }

        // --- 4. NÂNG CẤP SCANGUIDEPANEL ---
        GameObject guideGO = GameObject.Find("Canvas/ScanGuidePanel/GuideText");
        if (guideGO != null)
        {
            var tmp = guideGO.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.font = null;
                if (alataFont != null) tmp.font = alataFont;
                tmp.fontSize = 32f;
                tmp.color = new Color(0.96f, 0.97f, 0.98f, 1f);
                tmp.alignment = TextAlignmentOptions.Center;
                EditorUtility.SetDirty(tmp);
            }
        }

        GameObject oldBackHome = GameObject.Find("Canvas/ScanGuidePanel/BackHomeButton");
        if (oldBackHome != null)
        {
            Undo.DestroyObjectImmediate(oldBackHome);
        }

        GameObject scanGuidePanelGO = GameObject.Find("Canvas/ScanGuidePanel");
        GameObject skyButtonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(skyPrefab);
        if (scanGuidePanelGO != null && skyButtonPrefab != null)
        {
            GameObject newBackHome = (GameObject)PrefabUtility.InstantiatePrefab(skyButtonPrefab, scanGuidePanelGO.transform);
            newBackHome.name = "BackHomeButton";
            SetupPremiumButton(newBackHome, "Về trang chủ", josefinFont, Vector2.zero);

            // Correctly anchor and position BackHomeButton at the bottom center of the screen
            RectTransform backHomeRT = newBackHome.GetComponent<RectTransform>();
            if (backHomeRT != null)
            {
                backHomeRT.anchorMin = new Vector2(0.5f, 0f);
                backHomeRT.anchorMax = new Vector2(0.5f, 0f);
                backHomeRT.pivot = new Vector2(0.5f, 0.5f);
                backHomeRT.anchoredPosition = new Vector2(0f, 150f);
                EditorUtility.SetDirty(backHomeRT);
            }

            Button backHomeBtn = newBackHome.GetComponent<Button>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(backHomeBtn.onClick, manager.GoHome);
            EditorUtility.SetDirty(backHomeBtn);
        }

        // --- 5. BIND OBJECT REFERENCES FOR UI CONTROLLERS ---
        GameObject lessonPanelGO = GameObject.Find("Canvas/LessonPanel");
        if (lessonPanelGO != null)
        {
            var lessonUI = lessonPanelGO.GetComponent<LessonUIController>();
            if (lessonUI != null)
            {
                var lessonTitleTextGO = GameObject.Find("Canvas/LessonPanel/Header/LessonTitleText");
                if (lessonTitleTextGO != null) lessonUI.lessonTitleText = lessonTitleTextGO.GetComponent<TextMeshProUGUI>();

                var stepTitleTextGO = GameObject.Find("Canvas/LessonPanel/Header/StepIndexText");
                if (stepTitleTextGO != null) lessonUI.stepTitleText = stepTitleTextGO.GetComponent<TextMeshProUGUI>();

                var stepContentTextGO = GameObject.Find("Canvas/LessonPanel/ContentArea/TextPanel/StepContentText");
                if (stepContentTextGO != null) lessonUI.stepContentText = stepContentTextGO.GetComponent<TextMeshProUGUI>();

                EditorUtility.SetDirty(lessonUI);
            }
        }

        GameObject lessonPanelManagerGO = GameObject.Find("Canvas/LessonPanel/LessonPanelManager");
        if (lessonPanelManagerGO != null)
        {
            var stepUI = lessonPanelManagerGO.GetComponent<LessonStepUIController>();
            if (stepUI != null)
            {
                var lessonTitleTextGO = GameObject.Find("Canvas/LessonPanel/Header/LessonTitleText");
                if (lessonTitleTextGO != null) stepUI.lessonTitleText = lessonTitleTextGO.GetComponent<TextMeshProUGUI>();

                var stepIndexTextGO = GameObject.Find("Canvas/LessonPanel/Header/StepIndexText");
                if (stepIndexTextGO != null) stepUI.stepIndexText = stepIndexTextGO.GetComponent<TextMeshProUGUI>();

                var stepTypeTextGO = GameObject.Find("Canvas/LessonPanel/Header/StepTypeText");
                if (stepTypeTextGO != null) stepUI.stepTypeText = stepTypeTextGO.GetComponent<TextMeshProUGUI>();

                var textPanelGO = GameObject.Find("Canvas/LessonPanel/ContentArea/TextPanel");
                if (textPanelGO != null) stepUI.textPanel = textPanelGO;

                var textContentTextGO = GameObject.Find("Canvas/LessonPanel/ContentArea/TextPanel/StepContentText");
                if (textContentTextGO != null) stepUI.textContentText = textContentTextGO.GetComponent<TextMeshProUGUI>();

                var videoPanelGO = GameObject.Find("Canvas/LessonPanel/ContentArea/VideoPanel");
                if (videoPanelGO != null)
                {
                    stepUI.videoPanel = videoPanelGO;
                    var videoPlayerTransform = videoPanelGO.transform.Find("RemoteVideoPlayer");
                    if (videoPlayerTransform != null) stepUI.remoteVideoPlayer = videoPlayerTransform.GetComponent<RemoteVideoPlayer>();

                    var videoDescTransform = videoPanelGO.transform.Find("VideoDescriptionText");
                    if (videoDescTransform != null) stepUI.videoDescriptionText = videoDescTransform.GetComponent<TextMeshProUGUI>();
                }

                var galleryPanelGO = GameObject.Find("Canvas/LessonPanel/ContentArea/GalleryPanel");
                if (galleryPanelGO != null)
                {
                    stepUI.galleryPanel = galleryPanelGO;
                    var mainImgTransform = galleryPanelGO.transform.Find("MainImage");
                    if (mainImgTransform != null)
                    {
                        stepUI.mainImage = mainImgTransform.GetComponent<Image>();
                    }

                    var imageLoaderTransform = galleryPanelGO.transform.Find("GalleryImageLoader");
                    if (imageLoaderTransform != null)
                    {
                        stepUI.mainImageLoader = imageLoaderTransform.GetComponent<RemoteImageLoader>();
                    }

                    var galleryDescTransform = galleryPanelGO.transform.Find("GalleryDescriptionText");
                    if (galleryDescTransform != null)
                    {
                        stepUI.galleryDescriptionText = galleryDescTransform.GetComponent<TextMeshProUGUI>();
                        var tmp = stepUI.galleryDescriptionText;
                        if (tmp != null)
                        {
                            tmp.font = null;
                            if (alataFont != null) tmp.font = alataFont;
                            tmp.fontSize = 28f;
                            tmp.color = new Color(0.96f, 0.97f, 0.98f, 1f);
                            tmp.alignment = TextAlignmentOptions.Top;
                            tmp.enableAutoSizing = false;
                            tmp.textWrappingMode = TextWrappingModes.Normal;
                            EditorUtility.SetDirty(tmp);
                        }

                        var rt = galleryDescTransform.GetComponent<RectTransform>();
                        if (rt != null)
                        {
                            rt.anchorMin = new Vector2(0.5f, 0f);
                            rt.anchorMax = new Vector2(0.5f, 0f);
                            rt.pivot = new Vector2(0.5f, 0.5f);
                            rt.anchoredPosition = new Vector2(0f, 180f);
                            rt.sizeDelta = new Vector2(820f, 320f);
                            EditorUtility.SetDirty(rt);
                        }
                    }

                    var buttonPreNextTransform = galleryPanelGO.transform.Find("ButtonPre-Next");
                    if (buttonPreNextTransform != null)
                    {
                        var pre = buttonPreNextTransform.Find("btnPre");
                        if (pre != null) stepUI.btnPre = pre.GetComponent<Button>();

                        var next = buttonPreNextTransform.Find("btnNext");
                        if (next != null) stepUI.btnNext = next.GetComponent<Button>();
                    }
                }

                EditorUtility.SetDirty(stepUI);
            }
        }

        EditorUtility.SetDirty(previewPanel);
        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(managerGO);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("--- ĐÃ NÂNG CẤP TOÀN BỘ UI & FONT CỦA AR SCAN SCENE LÊN PREMIUM PHÂN GIẢI CAO ---");
    }

    private static void SetupPremiumButton(GameObject btnGO, string label, TMP_FontAsset font, Vector2 anchoredPos)
    {
        RectTransform rt = btnGO.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(430f, 100f);
            rt.anchoredPosition = anchoredPos;
            EditorUtility.SetDirty(rt);
        }

        Image btnImg = btnGO.GetComponent<Image>();
        if (btnImg != null)
        {
            btnImg.type = Image.Type.Sliced;
            EditorUtility.SetDirty(btnImg);
        }

        Transform textChild = btnGO.transform.Find("Text");
        if (textChild != null)
        {
            var oldText = textChild.GetComponent<Text>();
            if (oldText != null) DestroyImmediate(oldText);

            var tmp = textChild.gameObject.GetComponent<TextMeshProUGUI>();
            if (tmp == null) tmp = textChild.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.font = null;
            if (font != null) tmp.font = font;
            tmp.fontSize = 28f;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            EditorUtility.SetDirty(tmp);

            RectTransform tRT = textChild.GetComponent<RectTransform>();
            tRT.anchorMin = Vector2.zero;
            tRT.anchorMax = Vector2.one;
            tRT.offsetMin = Vector2.zero;
            tRT.offsetMax = Vector2.zero;
            EditorUtility.SetDirty(tRT);

            var textPressed = btnGO.GetComponent<TextPressed>();
            if (textPressed != null)
            {
                textPressed.UnitsToMove = -5f;
                FieldInfo field = typeof(TextPressed).GetField("gameObject", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null) field.SetValue(textPressed, tRT);
                EditorUtility.SetDirty(textPressed);
            }
        }
    }

    [MenuItem("Tools/AR History/Stop Play and Open ARScan")]
    public static void StopPlayAndOpen()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            Debug.Log("Đã tắt Play Mode.");
        }
        
        EditorSceneManager.OpenScene("Assets/Scenes/ARScanScene.unity");
        Debug.Log("Đã mở ARScanScene ở chế độ Edit thành công.");
    }
}
