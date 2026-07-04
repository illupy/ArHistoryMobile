// using UnityEditor;
// using UnityEditor.SceneManagement;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.Video;
// using TMPro;

// public class LessonPanelRebuilder
// {
//     [MenuItem("Tools/AR History/Rebuild LessonPanel Only")]
//     public static void RebuildLessonPanelOnly()
//     {
//         var scene = EditorSceneManager.GetActiveScene();

//         Canvas canvas = Object.FindFirstObjectByType<Canvas>();
//         if (canvas == null)
//         {
//             Debug.LogError("Không tìm thấy Canvas trong scene.");
//             return;
//         }

//         Transform lessonPanel = canvas.transform.Find("LessonPanel");
//         if (lessonPanel == null)
//         {
//             Debug.LogError("Không tìm thấy LessonPanel trong Canvas. Hãy tạo sẵn object LessonPanel trước.");
//             return;
//         }

//         // Xóa toàn bộ con cũ trong LessonPanel
//         for (int i = lessonPanel.childCount - 1; i >= 0; i--)
//         {
//             Object.DestroyImmediate(lessonPanel.GetChild(i).gameObject);
//         }

//         RectTransform lessonRT = lessonPanel.GetComponent<RectTransform>();
//         if (lessonRT == null)
//         {
//             lessonRT = lessonPanel.gameObject.AddComponent<RectTransform>();
//         }

//         Image lessonBg = lessonPanel.GetComponent<Image>();
//         if (lessonBg == null)
//         {
//             lessonBg = lessonPanel.gameObject.AddComponent<Image>();
//         }
//         lessonBg.color = new Color(0.08f, 0.09f, 0.18f, 0.92f);

//         lessonRT.anchorMin = new Vector2(0.5f, 0.5f);
//         lessonRT.anchorMax = new Vector2(0.5f, 0.5f);
//         lessonRT.pivot = new Vector2(0.5f, 0.5f);
//         lessonRT.sizeDelta = new Vector2(980, 1450);
//         lessonRT.anchoredPosition = Vector2.zero;

//         // ===== Header =====
//         GameObject header = CreateUIObject("Header", lessonPanel);
//         RectTransform headerRT = header.GetComponent<RectTransform>();
//         headerRT.anchorMin = new Vector2(0.5f, 1f);
//         headerRT.anchorMax = new Vector2(0.5f, 1f);
//         headerRT.pivot = new Vector2(0.5f, 1f);
//         headerRT.sizeDelta = new Vector2(900, 180);
//         headerRT.anchoredPosition = new Vector2(0, -30);

//         GameObject lessonTitleText = CreateTMPText("LessonTitleText", header.transform, "Tiêu đề bài học", 42, FontStyles.Bold);
//         SetRect(lessonTitleText.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -20), new Vector2(820, 60));

//         GameObject stepIndexText = CreateTMPText("StepIndexText", header.transform, "Bước 1/3", 28, FontStyles.Bold);
//         SetRect(stepIndexText.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(120, -90), new Vector2(180, 50));

//         GameObject stepTypeText = CreateTMPText("StepTypeText", header.transform, "Văn bản", 26, FontStyles.Bold);
//         SetRect(stepTypeText.GetComponent<RectTransform>(), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-140, -90), new Vector2(220, 50));

//         // ===== Content Area =====
//         GameObject contentArea = CreateUIObject("ContentArea", lessonPanel);
//         RectTransform contentRT = contentArea.GetComponent<RectTransform>();
//         contentRT.anchorMin = new Vector2(0.5f, 1f);
//         contentRT.anchorMax = new Vector2(0.5f, 1f);
//         contentRT.pivot = new Vector2(0.5f, 1f);
//         contentRT.sizeDelta = new Vector2(900, 980);
//         contentRT.anchoredPosition = new Vector2(0, -220);

//         // ===== Text Panel =====
//         GameObject textPanel = CreatePanel("TextPanel", contentArea.transform, new Color(0.12f, 0.13f, 0.22f, 0.95f));
//         StretchFull(textPanel.GetComponent<RectTransform>());

//         GameObject stepContentText = CreateTMPText("StepContentText", textPanel.transform, "Nội dung bước học", 30, FontStyles.Normal);
//         TMP_Text contentTmp = stepContentText.GetComponent<TextMeshProUGUI>();
//         contentTmp.alignment = TextAlignmentOptions.TopLeft;
//         contentTmp.enableWordWrapping = true;
//         SetRect(stepContentText.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(820, 900));

//         // ===== Video Panel =====
//         GameObject videoPanel = CreatePanel("VideoPanel", contentArea.transform, new Color(0.12f, 0.13f, 0.22f, 0.95f));
//         StretchFull(videoPanel.GetComponent<RectTransform>());
//         videoPanel.SetActive(false);

//         GameObject videoRaw = CreateUIObject("VideoRawImage", videoPanel.transform);
//         RawImage rawImage = videoRaw.AddComponent<RawImage>();
//         rawImage.color = Color.white;
//         RectTransform rawRT = videoRaw.GetComponent<RectTransform>();
//         rawRT.anchorMin = new Vector2(0.5f, 1f);
//         rawRT.anchorMax = new Vector2(0.5f, 1f);
//         rawRT.pivot = new Vector2(0.5f, 1f);
//         rawRT.sizeDelta = new Vector2(780, 440);
//         rawRT.anchoredPosition = new Vector2(0, -30);

//         GameObject loadingText = CreateTMPText("LoadingText", videoPanel.transform, "Đang tải video...", 26, FontStyles.Italic);
//         SetRect(loadingText.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -500), new Vector2(400, 50));

//         GameObject videoDescriptionText = CreateTMPText("VideoDescriptionText", videoPanel.transform, "Mô tả video", 28, FontStyles.Normal);
//         TMP_Text videoDescTmp = videoDescriptionText.GetComponent<TextMeshProUGUI>();
//         videoDescTmp.alignment = TextAlignmentOptions.TopLeft;
//         SetRect(videoDescriptionText.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 180), new Vector2(820, 360));

//         GameObject videoPlayerGO = new GameObject("RemoteVideoPlayer");
//         videoPlayerGO.transform.SetParent(videoPanel.transform, false);
//         VideoPlayer videoPlayer = videoPlayerGO.AddComponent<VideoPlayer>();
//         RemoteVideoPlayer remoteVideoPlayer = videoPlayerGO.AddComponent<RemoteVideoPlayer>();

//         // ===== Gallery Panel =====
//         GameObject galleryPanel = CreatePanel("GalleryPanel", contentArea.transform, new Color(0.12f, 0.13f, 0.22f, 0.95f));
//         StretchFull(galleryPanel.GetComponent<RectTransform>());
//         galleryPanel.SetActive(false);

//         GameObject mainImage = CreateUIObject("MainImage", galleryPanel.transform);
//         Image mainImageComp = mainImage.AddComponent<Image>();
//         mainImageComp.color = Color.white;
//         RectTransform mainImageRT = mainImage.GetComponent<RectTransform>();
//         mainImageRT.anchorMin = new Vector2(0.5f, 1f);
//         mainImageRT.anchorMax = new Vector2(0.5f, 1f);
//         mainImageRT.pivot = new Vector2(0.5f, 1f);
//         mainImageRT.sizeDelta = new Vector2(760, 420);
//         mainImageRT.anchoredPosition = new Vector2(0, -30);

//         GameObject thumbsRow = CreateUIObject("ThumbsRow", galleryPanel.transform);
//         RectTransform thumbsRT = thumbsRow.GetComponent<RectTransform>();
//         thumbsRT.anchorMin = new Vector2(0.5f, 1f);
//         thumbsRT.anchorMax = new Vector2(0.5f, 1f);
//         thumbsRT.pivot = new Vector2(0.5f, 1f);
//         thumbsRT.sizeDelta = new Vector2(780, 120);
//         thumbsRT.anchoredPosition = new Vector2(0, -490);

//         GameObject thumb1 = CreateButton(thumbsRow.transform, "ThumbButton1", "1", new Vector2(150, 90), new Vector2(-240, 0));
//         GameObject thumb2 = CreateButton(thumbsRow.transform, "ThumbButton2", "2", new Vector2(150, 90), new Vector2(-80, 0));
//         GameObject thumb3 = CreateButton(thumbsRow.transform, "ThumbButton3", "3", new Vector2(150, 90), new Vector2(80, 0));
//         GameObject thumb4 = CreateButton(thumbsRow.transform, "ThumbButton4", "4", new Vector2(150, 90), new Vector2(240, 0));

//         GameObject galleryDescriptionText = CreateTMPText("GalleryDescriptionText", galleryPanel.transform, "Mô tả bộ ảnh", 28, FontStyles.Normal);
//         TMP_Text galleryDescTmp = galleryDescriptionText.GetComponent<TextMeshProUGUI>();
//         galleryDescTmp.alignment = TextAlignmentOptions.TopLeft;
//         SetRect(galleryDescriptionText.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 180), new Vector2(820, 320));

//         GameObject galleryLoaderGO = new GameObject("GalleryImageLoader");
//         galleryLoaderGO.transform.SetParent(galleryPanel.transform, false);
//         RemoteImageLoader imageLoader = galleryLoaderGO.AddComponent<RemoteImageLoader>();

//         // ===== Buttons =====
//         GameObject prevButton = CreateButton(lessonPanel, "PreviousButton", "Trước", new Vector2(220, 90), new Vector2(-180, 1340));
//         GameObject nextButton = CreateButton(lessonPanel, "NextButton", "Tiếp", new Vector2(220, 90), new Vector2(180, 1340));

//         // ===== Manager =====
//         GameObject managerGO = lessonPanel.Find("LessonPanelManager")?.gameObject;
//         if (managerGO == null)
//         {
//             managerGO = new GameObject("LessonPanelManager");
//             managerGO.transform.SetParent(lessonPanel, false);
//         }

//         LessonStepUIController controller = managerGO.GetComponent<LessonStepUIController>();
//         if (controller == null)
//         {
//             controller = managerGO.AddComponent<LessonStepUIController>();
//         }

//         controller.lessonTitleText = lessonTitleText.GetComponent<TextMeshProUGUI>();
//         controller.stepIndexText = stepIndexText.GetComponent<TextMeshProUGUI>();
//         controller.stepTypeText = stepTypeText.GetComponent<TextMeshProUGUI>();
//         controller.stepContentText = stepContentText.GetComponent<TextMeshProUGUI>();

//         controller.textPanel = textPanel;
//         controller.videoPanel = videoPanel;
//         controller.galleryPanel = galleryPanel;

//         remoteVideoPlayer.videoPlayer = videoPlayer;
//         remoteVideoPlayer.videoRawImage = rawImage;
//         remoteVideoPlayer.loadingText = loadingText;
//         controller.remoteVideoPlayer = remoteVideoPlayer;

//         imageLoader.targetImage = mainImageComp;
//         controller.mainImageLoader = imageLoader;
//         controller.mainImage = mainImageComp;
//         controller.thumbButton1 = thumb1.GetComponent<Button>();
//         controller.thumbButton2 = thumb2.GetComponent<Button>();
//         controller.thumbButton3 = thumb3.GetComponent<Button>();
//         controller.thumbButton4 = thumb4.GetComponent<Button>();

//         // tìm ARPreviewSceneManager để gắn lại nút nếu có
//         ARPreviewSceneManager previewManager = Object.FindFirstObjectByType<ARPreviewSceneManager>();
//         if (previewManager != null)
//         {
//             Button prevBtnComp = prevButton.GetComponent<Button>();
//             prevBtnComp.onClick.RemoveAllListeners();
//             prevBtnComp.onClick.AddListener(previewManager.PreviousStep);

//             Button nextBtnComp = nextButton.GetComponent<Button>();
//             nextBtnComp.onClick.RemoveAllListeners();
//             nextBtnComp.onClick.AddListener(previewManager.NextStep);
//         }

//         EditorSceneManager.MarkSceneDirty(scene);
//         Debug.Log("Đã rebuild xong LessonPanel.");
//     }

//     private static GameObject CreateUIObject(string name, Transform parent)
//     {
//         GameObject go = new GameObject(name, typeof(RectTransform));
//         go.transform.SetParent(parent, false);
//         return go;
//     }

//     private static GameObject CreatePanel(string name, Transform parent, Color color)
//     {
//         GameObject go = CreateUIObject(name, parent);
//         Image img = go.AddComponent<Image>();
//         img.color = color;
//         return go;
//     }

//     private static void StretchFull(RectTransform rt)
//     {
//         rt.anchorMin = Vector2.zero;
//         rt.anchorMax = Vector2.one;
//         rt.offsetMin = Vector2.zero;
//         rt.offsetMax = Vector2.zero;
//     }

//     private static void SetRect(RectTransform rt, Vector2 min, Vector2 max, Vector2 pos, Vector2 size)
//     {
//         rt.anchorMin = min;
//         rt.anchorMax = max;
//         rt.pivot = new Vector2(0.5f, 0.5f);
//         rt.anchoredPosition = pos;
//         rt.sizeDelta = size;
//     }

//     private static GameObject CreateTMPText(string name, Transform parent, string text, float fontSize, FontStyles style)
//     {
//         GameObject go = CreateUIObject(name, parent);
//         TMP_Text tmp = go.AddComponent<TextMeshProUGUI>();
//         tmp.text = text;
//         tmp.fontSize = fontSize;
//         tmp.fontStyle = style;
//         tmp.alignment = TextAlignmentOptions.Center;
//         tmp.color = Color.white;
//         return go;
//     }

//     private static GameObject CreateButton(Transform parent, string name, string label, Vector2 size, Vector2 pos)
//     {
//         GameObject btn = CreateUIObject(name, parent);
//         Image img = btn.AddComponent<Image>();
//         img.color = new Color(0.17f, 0.36f, 0.62f, 1f);
//         btn.AddComponent<Button>();

//         RectTransform rt = btn.GetComponent<RectTransform>();
//         rt.anchorMin = new Vector2(0.5f, 1f);
//         rt.anchorMax = new Vector2(0.5f, 1f);
//         rt.pivot = new Vector2(0.5f, 1f);
//         rt.sizeDelta = size;
//         rt.anchoredPosition = new Vector2(pos.x, -pos.y);

//         GameObject txt = CreateTMPText("Text", btn.transform, label, 28, FontStyles.Bold);
//         TMP_Text tmp = txt.GetComponent<TextMeshProUGUI>();
//         tmp.color = Color.white;
//         StretchFull(txt.GetComponent<RectTransform>());

//         return btn;
//     }
// }