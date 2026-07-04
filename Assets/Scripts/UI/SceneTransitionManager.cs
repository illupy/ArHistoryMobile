using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneTransitionManager : MonoBehaviour
{
    private static SceneTransitionManager _instance;
    public static SceneTransitionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SceneTransitionManager");
                _instance = go.AddComponent<SceneTransitionManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private Slider _progressBar;
    private TMP_Text _progressText;
    private TMP_Text _tipsText;

    private bool _isLoading = false;

    private string[] _loadingTips = new string[]
    {
        "Khám phá thế giới lịch sử qua công nghệ AR chân thực.",
        "Hướng camera vào hình ảnh marker để xem mô hình 3D.",
        "Hoàn thành các thử thách trí tuệ để nhận điểm số cao.",
        "Quét hình ảnh rõ nét, đủ ánh sáng để nhận diện tốt hơn.",
        "Mỗi bài học đều chứa đựng những kiến thức lịch sử bổ ích."
    };

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            CreateLoadingUI();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void CreateLoadingUI()
    {
        // Create Canvas GameObject
        GameObject canvasGO = new GameObject("LoadingCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CanvasGroup));
        canvasGO.transform.SetParent(transform, false);

        _canvas = canvasGO.GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 9999; // Make sure it sits on top of everything

        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        _canvasGroup = canvasGO.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
        canvasGO.SetActive(false);

        // Background
        GameObject bgGO = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bgGO.transform.SetParent(canvasGO.transform, false);
        RectTransform bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        Image bgImg = bgGO.GetComponent<Image>();
        // Sleek premium dark blue/indigo color palette
        bgImg.color = new Color(0.07f, 0.09f, 0.15f, 1f); 

        // Panel Container (center-bottom positioned)
        GameObject containerGO = new GameObject("Container", typeof(RectTransform));
        containerGO.transform.SetParent(canvasGO.transform, false);
        RectTransform containerRT = containerGO.GetComponent<RectTransform>();
        containerRT.anchorMin = new Vector2(0.5f, 0.5f);
        containerRT.anchorMax = new Vector2(0.5f, 0.5f);
        containerRT.anchoredPosition = new Vector2(0f, -200f);
        containerRT.sizeDelta = new Vector2(800, 400);

        // Title text ("Đang tải...")
        GameObject titleGO = new GameObject("TitleText", typeof(RectTransform), typeof(TextMeshProUGUI));
        titleGO.transform.SetParent(containerGO.transform, false);
        RectTransform titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.5f, 1f);
        titleRT.anchorMax = new Vector2(0.5f, 1f);
        titleRT.anchoredPosition = new Vector2(0f, -50f);
        titleRT.sizeDelta = new Vector2(700, 80);
        TMP_Text titleTxt = titleGO.GetComponent<TextMeshProUGUI>();
        titleTxt.text = "ĐANG TẢI BÀI HỌC";
        titleTxt.fontSize = 44;
        titleTxt.fontStyle = FontStyles.Bold;
        titleTxt.alignment = TextAlignmentOptions.Center;
        titleTxt.color = new Color(0.9f, 0.92f, 0.98f, 1f);

        // Progress Slider
        GameObject sliderGO = new GameObject("ProgressBar", typeof(RectTransform), typeof(Slider));
        sliderGO.transform.SetParent(containerGO.transform, false);
        RectTransform sliderRT = sliderGO.GetComponent<RectTransform>();
        sliderRT.anchorMin = new Vector2(0.5f, 0.5f);
        sliderRT.anchorMax = new Vector2(0.5f, 0.5f);
        sliderRT.anchoredPosition = new Vector2(0f, -60f);
        sliderRT.sizeDelta = new Vector2(700, 24);

        _progressBar = sliderGO.GetComponent<Slider>();
        _progressBar.transition = Selectable.Transition.None;
        _progressBar.interactable = false;

        // Slider Background Area
        GameObject sliderBgGO = new GameObject("Background", typeof(RectTransform), typeof(Image));
        sliderBgGO.transform.SetParent(sliderGO.transform, false);
        RectTransform sBgRT = sliderBgGO.GetComponent<RectTransform>();
        sBgRT.anchorMin = Vector2.zero;
        sBgRT.anchorMax = Vector2.one;
        sBgRT.offsetMin = Vector2.zero;
        sBgRT.offsetMax = Vector2.zero;
        Image sBgImg = sliderBgGO.GetComponent<Image>();
        sBgImg.color = new Color(0.15f, 0.18f, 0.28f, 1f);

        // Slider Fill Area
        GameObject fillAreaGO = new GameObject("Fill Area", typeof(RectTransform));
        fillAreaGO.transform.SetParent(sliderGO.transform, false);
        RectTransform fillAreaRT = fillAreaGO.GetComponent<RectTransform>();
        fillAreaRT.anchorMin = Vector2.zero;
        fillAreaRT.anchorMax = Vector2.one;
        fillAreaRT.offsetMin = new Vector2(5, 2);
        fillAreaRT.offsetMax = new Vector2(-5, -2);

        GameObject fillGO = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fillGO.transform.SetParent(fillAreaGO.transform, false);
        RectTransform fillRT = fillGO.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = new Vector2(0f, 1f); // controlled by Slider
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;
        Image fillImg = fillGO.GetComponent<Image>();
        // Vivid Cyan/Teal color for premium UI look
        fillImg.color = new Color(0.18f, 0.77f, 0.71f, 1f); 
        _progressBar.fillRect = fillRT;

        // Progress Text (e.g. "45%")
        GameObject progressTextGO = new GameObject("ProgressText", typeof(RectTransform), typeof(TextMeshProUGUI));
        progressTextGO.transform.SetParent(containerGO.transform, false);
        RectTransform pTextRT = progressTextGO.GetComponent<RectTransform>();
        pTextRT.anchorMin = new Vector2(0.5f, 0.5f);
        pTextRT.anchorMax = new Vector2(0.5f, 0.5f);
        pTextRT.anchoredPosition = new Vector2(0f, -120f);
        pTextRT.sizeDelta = new Vector2(200, 60);
        _progressText = progressTextGO.GetComponent<TextMeshProUGUI>();
        _progressText.text = "0%";
        _progressText.fontSize = 32;
        _progressText.fontStyle = FontStyles.Bold;
        _progressText.alignment = TextAlignmentOptions.Center;
        _progressText.color = new Color(0.18f, 0.77f, 0.71f, 1f);

        // Loading Tips Text
        GameObject tipsTextGO = new GameObject("TipsText", typeof(RectTransform), typeof(TextMeshProUGUI));
        tipsTextGO.transform.SetParent(canvasGO.transform, false);
        RectTransform tipsRT = tipsTextGO.GetComponent<RectTransform>();
        tipsRT.anchorMin = new Vector2(0.5f, 0f);
        tipsRT.anchorMax = new Vector2(0.5f, 0f);
        tipsRT.anchoredPosition = new Vector2(0f, 180f);
        tipsRT.sizeDelta = new Vector2(900, 150);
        _tipsText = tipsTextGO.GetComponent<TextMeshProUGUI>();
        _tipsText.text = "Gợi ý: " + _loadingTips[0];
        _tipsText.fontSize = 32;
        _tipsText.fontStyle = FontStyles.Italic;
        _tipsText.alignment = TextAlignmentOptions.Center;
        _tipsText.enableWordWrapping = true;
        _tipsText.color = new Color(0.6f, 0.64f, 0.75f, 1f);
    }

    public void LoadScene(string sceneName)
    {
        if (_isLoading) return;
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        _isLoading = true;

        // Choose a random tip
        if (_tipsText != null && _loadingTips.Length > 0)
        {
            _tipsText.text = "Gợi ý: " + _loadingTips[Random.Range(0, _loadingTips.Length)];
        }

        // Show UI Canvas
        _canvas.gameObject.SetActive(true);
        _progressBar.value = 0f;
        _progressText.text = "0%";

        // Fade In
        float elapsed = 0f;
        float fadeDuration = 0.4f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        _canvasGroup.alpha = 1f;

        // Start loading scene asynchronously
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float visualProgress = 0f;
        while (!op.isDone)
        {
            // Unity's async operation progress goes from 0 to 0.9, then jumps to 1.0 when activated
            float targetProgress = Mathf.Clamp01(op.progress / 0.9f);
            
            // Smoothly interpolate the progress bar for premium look
            while (visualProgress < targetProgress)
            {
                visualProgress += Time.deltaTime * 1.5f; // control speed of progression
                visualProgress = Mathf.Min(visualProgress, targetProgress);
                _progressBar.value = visualProgress;
                _progressText.text = Mathf.RoundToInt(visualProgress * 100f) + "%";
                yield return null;
            }

            if (op.progress >= 0.9f && !op.allowSceneActivation)
            {
                // Smooth final crawl to 100%
                while (visualProgress < 1f)
                {
                    visualProgress += Time.deltaTime * 2.0f;
                    _progressBar.value = Mathf.Min(visualProgress, 1f);
                    _progressText.text = Mathf.RoundToInt(_progressBar.value * 100f) + "%";
                    yield return null;
                }
                
                // Extra short delay for visual satisfaction
                yield return new WaitForSeconds(0.1f);
                op.allowSceneActivation = true;
            }

            yield return null;
        }

        // Turn off loading screen immediately without fade out
        _canvasGroup.alpha = 0f;
        _canvas.gameObject.SetActive(false);

        _isLoading = false;
    }

    public void ShowLoadingDirect(string title, string tip)
    {
        if (_canvas == null) CreateLoadingUI();
        _canvas.gameObject.SetActive(true);
        _canvasGroup.alpha = 1f;
        _progressBar.value = 0f;
        _progressText.text = "0%";
        if (_tipsText != null) _tipsText.text = tip;
    }

    public void UpdateProgressDirect(float progress)
    {
        if (_progressBar != null)
        {
            _progressBar.value = progress;
        }
        if (_progressText != null)
        {
            _progressText.text = Mathf.RoundToInt(progress * 100f) + "%";
        }
    }

    public void HideLoadingDirect()
    {
        if (_canvas != null)
        {
            StartCoroutine(FadeOutDirectCoroutine());
        }
    }

    private IEnumerator FadeOutDirectCoroutine()
    {
        // Turn off loading screen immediately without fade out
        _canvasGroup.alpha = 0f;
        _canvas.gameObject.SetActive(false);
        yield break;
    }
}
