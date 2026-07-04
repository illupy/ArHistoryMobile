using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LessonStepUIController : MonoBehaviour
{
    [Header("Header")]
    public TextMeshProUGUI lessonTitleText;
    public TextMeshProUGUI stepIndexText;
    public TextMeshProUGUI stepTypeText;

    [Header("TEXT")]
    public GameObject textPanel;
    public TextMeshProUGUI textContentText;

    [Header("VIDEO")]
    public GameObject videoPanel;
    public RemoteVideoPlayer remoteVideoPlayer;
    public TextMeshProUGUI videoDescriptionText;

    [Header("GALLERY")]
    public GameObject galleryPanel;
    public RemoteImageLoader mainImageLoader;
    public Image mainImage;
    public TextMeshProUGUI galleryDescriptionText;
    public TextMeshProUGUI galleryIndexText;
    public Button btnPre;
    public Button btnNext;

    private List<string> currentGalleryUrls = new List<string>();
    private int currentGalleryIndex = 0;
    private int lastSlideDirection = 0;
    private bool isTransitioning = false;
    private LessonDetailResponse m_currentLesson;

    private void Awake()
    {
        SetupGalleryDescScroll();
    }

    private void SetupGalleryDescScroll()
    {
        if (galleryDescriptionText == null) return;

        // Check if already wrapped in a ScrollRect
        ScrollRect existingScroll = galleryDescriptionText.GetComponentInParent<ScrollRect>();
        if (existingScroll != null && existingScroll.gameObject != galleryPanel)
        {
            return;
        }

        // Save original references and configuration
        Transform originalParent = galleryDescriptionText.transform.parent;
        RectTransform textRT = galleryDescriptionText.rectTransform;
        
        Vector2 anchoredPosition = textRT.anchoredPosition;
        Vector2 sizeDelta = textRT.sizeDelta;
        Vector2 anchorMin = textRT.anchorMin;
        Vector2 anchorMax = textRT.anchorMax;
        Vector2 pivot = textRT.pivot;

        // Create ScrollRect wrapper viewport
        GameObject viewportGO = new GameObject("GalleryDescViewport", typeof(RectTransform), typeof(RectMask2D), typeof(ScrollRect));
        viewportGO.transform.SetParent(originalParent, false);
        
        RectTransform viewportRT = viewportGO.GetComponent<RectTransform>();
        viewportRT.anchorMin = anchorMin;
        viewportRT.anchorMax = anchorMax;
        viewportRT.pivot = pivot;
        viewportRT.anchoredPosition = anchoredPosition;
        viewportRT.sizeDelta = sizeDelta;

        ScrollRect scrollRect = viewportGO.GetComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 25f;

        // Move text under viewport
        galleryDescriptionText.transform.SetParent(viewportRT, false);

        // Adjust anchors to stretch horizontal and anchor to top
        textRT.anchorMin = new Vector2(0f, 1f);
        textRT.anchorMax = new Vector2(1f, 1f);
        textRT.pivot = new Vector2(0.5f, 1f);
        textRT.anchoredPosition = Vector2.zero;
        textRT.sizeDelta = new Vector2(0f, 0f);

        galleryDescriptionText.enableWordWrapping = true;
        
        ContentSizeFitter fitter = galleryDescriptionText.gameObject.GetComponent<ContentSizeFitter>();
        if (fitter == null) fitter = galleryDescriptionText.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.content = textRT;
    }

    public void ShowStep(LessonDetailResponse lesson, LessonAssetItem step, int currentIndex, int totalSteps)
    {
        m_currentLesson = lesson;

        if (lessonTitleText != null) lessonTitleText.text = lesson != null ? lesson.title : "";
        if (stepIndexText != null) stepIndexText.text = $"Bước {currentIndex + 1}/{totalSteps}";
        if (stepTypeText != null) stepTypeText.text = GetTypeLabel(step.type);

        if (textPanel != null) textPanel.SetActive(step.type == "TEXT");
        if (videoPanel != null) videoPanel.SetActive(step.type == "VIDEO");
        if (galleryPanel != null) galleryPanel.SetActive(step.type == "IMAGE_GALLERY");

        if (remoteVideoPlayer != null)
        {
            remoteVideoPlayer.StopVideo();
        }

        if (step.type == "TEXT")
        {
            if (textContentText != null)
            {
                textContentText.text = ProcessAnnotations(step.content ?? "");
                
                // Reset scroll position to the top
                ScrollRect scrollRect = textContentText.GetComponentInParent<ScrollRect>();
                if (scrollRect != null)
                {
                    Canvas.ForceUpdateCanvases();
                    scrollRect.velocity = Vector2.zero;
                    scrollRect.verticalNormalizedPosition = 1f;
                }
            }
        }
        else if (step.type == "VIDEO")
        {
            if (videoDescriptionText != null)
                videoDescriptionText.text = ProcessAnnotations(step.content ?? "");

            string fullUrl = MediaUrlHelper.ToFullUrl(step.fileUrl);
            if (remoteVideoPlayer != null && !string.IsNullOrEmpty(fullUrl))
            {
                remoteVideoPlayer.PlayVideo(fullUrl);
            }
        }
        else if (step.type == "IMAGE_GALLERY")
        {
            if (galleryDescriptionText != null)
            {
                galleryDescriptionText.text = ProcessAnnotations(step.content ?? "");
                
                // Reset scroll position to the top
                ScrollRect scrollRect = galleryDescriptionText.GetComponentInParent<ScrollRect>();
                if (scrollRect != null)
                {
                    Canvas.ForceUpdateCanvases();
                    scrollRect.velocity = Vector2.zero;
                    scrollRect.verticalNormalizedPosition = 1f;
                }
            }

            currentGalleryUrls = step.mediaUrls ?? new List<string>();
            currentGalleryIndex = 0;
            lastSlideDirection = 0;
            isTransitioning = false;
            BindGalleryButtons();

            if (currentGalleryUrls.Count > 0 && mainImageLoader != null)
            {
                mainImageLoader.LoadImage(MediaUrlHelper.ToFullUrl(currentGalleryUrls[0]));
            }
            else if (mainImage != null)
            {
                mainImage.sprite = null;
            }
        }
    }

    private void BindGalleryButtons()
    {
        if (btnPre != null)
        {
            btnPre.onClick.RemoveAllListeners();
            btnPre.onClick.AddListener(ShowPreviousImage);
        }

        if (btnNext != null)
        {
            btnNext.onClick.RemoveAllListeners();
            btnNext.onClick.AddListener(ShowNextImage);
        }

        UpdateGalleryNavigationButtons();
    }

    private void Start()
    {
        if (mainImageLoader != null)
        {
            mainImageLoader.OnImageLoaded += OnRemoteImageLoaded;
        }

        AttachLinkHandler(textContentText);
        AttachLinkHandler(videoDescriptionText);
        AttachLinkHandler(galleryDescriptionText);
    }

    private void AttachLinkHandler(TextMeshProUGUI tmpText)
    {
        if (tmpText == null) return;
        tmpText.raycastTarget = true;
        var handler = tmpText.gameObject.GetComponent<TextMeshProLinkHandler>();
        if (handler == null)
        {
            handler = tmpText.gameObject.AddComponent<TextMeshProLinkHandler>();
        }
        handler.onLinkClick = HandleLinkClick;
    }

    private void HandleLinkClick(string linkId)
    {
        Debug.Log($"[LessonStepUIController] Link clicked: {linkId}");
        if (linkId.StartsWith("ann_"))
        {
            string idStr = linkId.Substring(4);
            if (long.TryParse(idStr, out long annotationId))
            {
                if (m_currentLesson != null && m_currentLesson.annotations != null)
                {
                    var annotation = m_currentLesson.annotations.Find(a => a.id == annotationId);
                    if (annotation != null)
                    {
                        Debug.Log($"[LessonStepUIController] Found annotation: {annotation.keyword}, type={annotation.annotationType}, modelCode={annotation.modelCode}");
                        
                        // Show popup containing model or text description
                        Match3NotePopup.Show(annotation.description, annotation.modelCode, annotation.mediaUrl, null);
                    }
                }
            }
        }
    }

    private string ProcessAnnotations(string content)
    {
        if (string.IsNullOrEmpty(content)) return "";

        string cleaned = Utils.CleanHtmlTags(content);

        if (m_currentLesson == null || m_currentLesson.annotations == null || m_currentLesson.annotations.Count == 0)
        {
            return cleaned;
        }

        foreach (var ann in m_currentLesson.annotations)
        {
            if (ann == null) continue;

            string targetTag = $"[ann:{ann.id}]";
            if (cleaned.Contains(targetTag))
            {
                string replacement = $"<link=\"ann_{ann.id}\"><color=#FFCC00><u>{ann.keyword}</u></color></link>";
                cleaned = cleaned.Replace(targetTag, replacement);
            }
        }

        return cleaned;
    }

    private void OnDestroy()
    {
        if (mainImageLoader != null)
        {
            mainImageLoader.OnImageLoaded -= OnRemoteImageLoaded;
        }
    }

    private void OnRemoteImageLoaded()
    {
        if (mainImage == null) return;

        mainImage.rectTransform.DOKill();
        mainImage.DOKill();

        if (lastSlideDirection == 0)
        {
            mainImage.rectTransform.anchoredPosition = Vector2.zero;
            mainImage.color = Color.white;
            isTransitioning = false;
        }
        else
        {
            float startX = lastSlideDirection == 1 ? 800f : -800f;
            mainImage.rectTransform.anchoredPosition = new Vector2(startX, 0f);
            Color c = mainImage.color;
            c.a = 0f;
            mainImage.color = c;

            mainImage.rectTransform.DOAnchorPosX(0f, 0.35f).SetEase(Ease.OutQuad);
            mainImage.DOFade(1f, 0.35f).OnComplete(() =>
            {
                isTransitioning = false;
            });
        }
    }

    private void ShowPreviousImage()
    {
        if (isTransitioning) return;
        if (currentGalleryUrls == null || currentGalleryUrls.Count == 0) return;
        
        isTransitioning = true;
        currentGalleryIndex--;
        if (currentGalleryIndex < 0)
        {
            currentGalleryIndex = currentGalleryUrls.Count - 1;
        }
        
        if (mainImage != null)
        {
            mainImage.rectTransform.DOKill();
            mainImage.DOKill();
            
            mainImage.rectTransform.DOAnchorPosX(800f, 0.25f).SetEase(Ease.InQuad);
            mainImage.DOFade(0f, 0.25f).OnComplete(() =>
            {
                lastSlideDirection = -1;
                LoadCurrentImage();
            });
        }
        else
        {
            lastSlideDirection = 0;
            LoadCurrentImage();
        }
        UpdateGalleryNavigationButtons();
    }

    private void ShowNextImage()
    {
        if (isTransitioning) return;
        if (currentGalleryUrls == null || currentGalleryUrls.Count == 0) return;

        isTransitioning = true;
        currentGalleryIndex++;
        if (currentGalleryIndex >= currentGalleryUrls.Count)
        {
            currentGalleryIndex = 0;
        }
        
        if (mainImage != null)
        {
            mainImage.rectTransform.DOKill();
            mainImage.DOKill();
            
            mainImage.rectTransform.DOAnchorPosX(-800f, 0.25f).SetEase(Ease.InQuad);
            mainImage.DOFade(0f, 0.25f).OnComplete(() =>
            {
                lastSlideDirection = 1;
                LoadCurrentImage();
            });
        }
        else
        {
            lastSlideDirection = 0;
            LoadCurrentImage();
        }
        UpdateGalleryNavigationButtons();
    }

    private void LoadCurrentImage()
    {
        if (currentGalleryUrls == null || currentGalleryIndex < 0 || currentGalleryIndex >= currentGalleryUrls.Count) return;

        if (mainImageLoader != null)
        {
            mainImageLoader.LoadImage(MediaUrlHelper.ToFullUrl(currentGalleryUrls[currentGalleryIndex]));
        }
    }

    private void UpdateGalleryNavigationButtons()
    {
        bool hasMultipleImages = currentGalleryUrls != null && currentGalleryUrls.Count > 1;
        
        if (btnPre != null) btnPre.gameObject.SetActive(hasMultipleImages);
        if (btnNext != null) btnNext.gameObject.SetActive(hasMultipleImages);

        if (galleryIndexText != null)
        {
            if (currentGalleryUrls == null || currentGalleryUrls.Count == 0)
            {
                galleryIndexText.text = "";
            }
            else
            {
                galleryIndexText.text = $"{currentGalleryIndex + 1}/{currentGalleryUrls.Count}";
            }
        }
    }

    private string GetTypeLabel(string type)
    {
        return type switch
        {
            "TEXT" => "Văn bản",
            "VIDEO" => "Video",
            "IMAGE_GALLERY" => "Bộ ảnh",
            _ => type
        };
    }
}