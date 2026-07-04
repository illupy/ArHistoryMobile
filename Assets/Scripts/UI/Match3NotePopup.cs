using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Match3NotePopup : MonoBehaviour
{
    private Action m_onCloseCallback;
    private Text m_buttonText;

    private RenderTexture m_renderTexture;
    private Camera m_modelCamera;
    private GameObject m_modelInstance;

    public static void Show(string noteText, Action onClose)
    {
        Show(noteText, null, null, onClose);
    }

    public static void Show(string noteText, string modelCode, Action onClose)
    {
        Show(noteText, modelCode, null, onClose);
    }

    public static void Show(string noteText, string modelCode, string imageUrl, Action onClose)
    {
        // Find existing main UI Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("PopupCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10000; // Place on top of everything
            CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
        }

        // Create popup root object
        GameObject popupGO = new GameObject("Match3NotePopup", typeof(RectTransform), typeof(CanvasGroup));
        popupGO.transform.SetParent(canvas.transform, false);

        RectTransform rootRT = popupGO.GetComponent<RectTransform>();
        rootRT.anchorMin = Vector2.zero;
        rootRT.anchorMax = Vector2.one;
        rootRT.offsetMin = Vector2.zero;
        rootRT.offsetMax = Vector2.zero;

        Match3NotePopup popup = popupGO.AddComponent<Match3NotePopup>();
        popup.m_onCloseCallback = onClose;

        popup.InitUI(noteText, modelCode, imageUrl);
    }

    private void InitUI(string noteText, string modelCode, string imageUrl)
    {
        // 1. Dark overlay background to block interactions, close on tap
        GameObject overlayGO = new GameObject("Overlay", typeof(RectTransform), typeof(Image), typeof(Button));
        overlayGO.transform.SetParent(transform, false);
        
        RectTransform overlayRT = overlayGO.GetComponent<RectTransform>();
        overlayRT.anchorMin = Vector2.zero;
        overlayRT.anchorMax = Vector2.one;
        overlayRT.offsetMin = Vector2.zero;
        overlayRT.offsetMax = Vector2.zero;

        Image overlayImg = overlayGO.GetComponent<Image>();
        overlayImg.color = new Color(0.04f, 0.05f, 0.08f, 0.85f); // Rich dark overlay
        overlayImg.raycastTarget = true; // Block UI clicks behind

        Button overlayBtn = overlayGO.GetComponent<Button>();
        overlayBtn.onClick.AddListener(OnClickOK);

        // Try to load model prefab
        GameObject modelPrefab = null;
        if (!string.IsNullOrEmpty(modelCode))
        {
            GameObject registryPrefab = Resources.Load<GameObject>("prefabs/PreviewModelRegistry");
            if (registryPrefab != null)
            {
                PreviewModelRegistry registry = registryPrefab.GetComponent<PreviewModelRegistry>();
                if (registry != null)
                {
                    modelPrefab = registry.GetPrefabByCode(modelCode);
                }
            }
        }

        bool hasModel = modelPrefab != null;
        bool hasImage = !string.IsNullOrEmpty(imageUrl);
        bool hasVisual = hasModel || hasImage;

        // 2. Dialog Container Panel
        GameObject panelGO = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        panelGO.transform.SetParent(transform, false);

        RectTransform panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.anchoredPosition = Vector2.zero;
        
        if (hasVisual)
        {
            panelRT.sizeDelta = new Vector2(900f, 850f);
        }
        else
        {
            panelRT.sizeDelta = new Vector2(900f, 650f);
        }

        Image panelImg = panelGO.GetComponent<Image>();
        // Premium Sleek dark blue indigo card background
        panelImg.color = new Color(0.12f, 0.15f, 0.24f, 1f);

        // 3. Header Title text
        GameObject headerGO = new GameObject("HeaderTitle", typeof(RectTransform), typeof(Text));
        headerGO.transform.SetParent(panelGO.transform, false);

        RectTransform headerRT = headerGO.GetComponent<RectTransform>();
        headerRT.anchorMin = new Vector2(0.5f, 1f);
        headerRT.anchorMax = new Vector2(0.5f, 1f);
        headerRT.anchoredPosition = new Vector2(0f, -60f);
        headerRT.sizeDelta = new Vector2(800f, 80f);

        Text headerTxt = headerGO.GetComponent<Text>();
        headerTxt.text = "KIẾN THỨC LỊCH SỬ";
        headerTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        headerTxt.fontSize = 42;
        headerTxt.fontStyle = FontStyle.Bold;
        headerTxt.alignment = TextAnchor.MiddleCenter;
        headerTxt.color = new Color(0.95f, 0.75f, 0.25f, 1f); // Gold accent

        // Instantiate 3D model if available
        if (hasModel)
        {
            // Create RenderTexture
            m_renderTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
            m_renderTexture.antiAliasing = 4;

            // Safe world space position far away
            Vector3 renderPosition = new Vector3(1000f, 1000f, 1000f);

            // Model
            m_modelInstance = Instantiate(modelPrefab);
            m_modelInstance.transform.position = renderPosition;
            m_modelInstance.transform.rotation = Quaternion.identity;
            m_modelInstance.transform.localScale = Vector3.one;

            // Create Camera
            GameObject cameraGO = new GameObject("PopupModelCamera", typeof(Camera));
            m_modelCamera = cameraGO.GetComponent<Camera>();
            m_modelCamera.targetTexture = m_renderTexture;
            m_modelCamera.clearFlags = CameraClearFlags.SolidColor;
            m_modelCamera.backgroundColor = new Color(0.12f, 0.15f, 0.24f, 0f); // transparent
            m_modelCamera.fieldOfView = 25f;

            // Focus camera on the model bounds dynamically
            FocusCameraOnModel(m_modelCamera, m_modelInstance);

            // Directional Light
            GameObject lightGO = new GameObject("PopupModelLight", typeof(Light));
            lightGO.transform.SetParent(cameraGO.transform);
            lightGO.transform.localPosition = Vector3.zero;
            lightGO.transform.localRotation = Quaternion.Euler(30f, 35f, 0f);
            Light light = lightGO.GetComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.3f;
            light.color = Color.white;

            float initialScale = 1.0f;

            // RawImage in UI
            GameObject rawImageGO = new GameObject("ModelRawImage", typeof(RectTransform), typeof(RawImage));
            rawImageGO.transform.SetParent(panelGO.transform, false);

            RectTransform rawImageRT = rawImageGO.GetComponent<RectTransform>();
            rawImageRT.anchorMin = new Vector2(0.5f, 1f);
            rawImageRT.anchorMax = new Vector2(0.5f, 1f);
            rawImageRT.sizeDelta = new Vector2(500f, 400f);
            rawImageRT.anchoredPosition = new Vector2(0f, -320f);

            RawImage rawImage = rawImageGO.GetComponent<RawImage>();
            rawImage.texture = m_renderTexture;

            // Attach drag rotator
            UIModelRotator rotator = rawImageGO.AddComponent<UIModelRotator>();
            rotator.Init(m_modelInstance.transform, initialScale);
        }
        else if (hasImage)
        {
            // Create Image in UI
            GameObject imageGO = new GameObject("ModelImage", typeof(RectTransform), typeof(Image));
            imageGO.transform.SetParent(panelGO.transform, false);

            RectTransform imageRT = imageGO.GetComponent<RectTransform>();
            imageRT.anchorMin = new Vector2(0.5f, 1f);
            imageRT.anchorMax = new Vector2(0.5f, 1f);
            imageRT.sizeDelta = new Vector2(500f, 400f);
            imageRT.anchoredPosition = new Vector2(0f, -320f);

            Image uiImage = imageGO.GetComponent<Image>();
            uiImage.color = new Color(1f, 1f, 1f, 0.1f); // transparent before load
            uiImage.preserveAspect = true;

            RemoteImageLoader loader = imageGO.AddComponent<RemoteImageLoader>();
            loader.targetImage = uiImage;
            loader.LoadImage(MediaUrlHelper.ToFullUrl(imageUrl));
        }

        // 4. Note Content text
        GameObject noteGO = new GameObject("NoteContent", typeof(RectTransform), typeof(Text));
        noteGO.transform.SetParent(panelGO.transform, false);

        RectTransform noteRT = noteGO.GetComponent<RectTransform>();
        noteRT.anchorMin = new Vector2(0.5f, 1f);
        noteRT.anchorMax = new Vector2(0.5f, 1f);
        
        if (hasVisual)
        {
            noteRT.anchoredPosition = new Vector2(0f, -570f);
            noteRT.sizeDelta = new Vector2(800f, 150f);
        }
        else
        {
            noteRT.anchoredPosition = new Vector2(0f, -310f);
            noteRT.sizeDelta = new Vector2(800f, 320f);
        }

        Text noteTxt = noteGO.GetComponent<Text>();
        string cleanNote = noteText.Trim();
        noteTxt.text = cleanNote;
        noteTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        noteTxt.fontSize = 32;
        noteTxt.fontStyle = FontStyle.Normal;
        noteTxt.alignment = TextAnchor.MiddleCenter;
        noteTxt.color = new Color(0.9f, 0.92f, 0.96f, 1f);
        noteTxt.horizontalOverflow = HorizontalWrapMode.Wrap;
        noteTxt.verticalOverflow = VerticalWrapMode.Truncate;

        // 5. Button OK
        GameObject btnGO = new GameObject("ButtonOK", typeof(RectTransform), typeof(Image), typeof(Button));
        btnGO.transform.SetParent(panelGO.transform, false);

        RectTransform btnRT = btnGO.GetComponent<RectTransform>();
        btnRT.anchorMin = new Vector2(0.5f, 0f);
        btnRT.anchorMax = new Vector2(0.5f, 0f);
        
        if (hasVisual)
        {
            btnRT.anchoredPosition = new Vector2(0f, 60f);
        }
        else
        {
            btnRT.anchoredPosition = new Vector2(0f, 70f);
        }
        btnRT.sizeDelta = new Vector2(350f, 90f);

        Image btnImg = btnGO.GetComponent<Image>();
        btnImg.color = new Color(0.18f, 0.77f, 0.71f, 1f); // Teal

        Button btn = btnGO.GetComponent<Button>();
        btn.targetGraphic = btnImg;
        btn.onClick.AddListener(OnClickOK);

        // 6. Button Text
        GameObject btnTxtGO = new GameObject("ButtonText", typeof(RectTransform), typeof(Text));
        btnTxtGO.transform.SetParent(btnGO.transform, false);

        RectTransform btnTxtRT = btnTxtGO.GetComponent<RectTransform>();
        btnTxtRT.anchorMin = Vector2.zero;
        btnTxtRT.anchorMax = Vector2.one;
        btnTxtRT.offsetMin = Vector2.zero;
        btnTxtRT.offsetMax = Vector2.zero;

        m_buttonText = btnTxtGO.GetComponent<Text>();
        m_buttonText.text = "ĐÃ HIỂU";
        m_buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        m_buttonText.fontSize = 30;
        m_buttonText.fontStyle = FontStyle.Bold;
        m_buttonText.alignment = TextAnchor.MiddleCenter;
        m_buttonText.color = Color.white;
    }

    private void OnClickOK()
    {
        m_onCloseCallback?.Invoke();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (m_modelInstance != null)
        {
            Destroy(m_modelInstance);
        }
        if (m_modelCamera != null)
        {
            Destroy(m_modelCamera.gameObject);
        }
        if (m_renderTexture != null)
        {
            m_renderTexture.Release();
            Destroy(m_renderTexture);
        }
    }

    private void FocusCameraOnModel(Camera cam, GameObject model, float paddingFactor = 1.3f)
    {
        // Calculate bounds of all renderers in the model (including inactive to be safe)
        Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);
        Debug.Log($"[Match3NotePopup] FocusCameraOnModel called. Found {renderers.Length} renderers in model. Model layer: {model.layer} ({LayerMask.LayerToName(model.layer)})");
        
        if (renderers.Length == 0)
        {
            Debug.LogWarning("[Match3NotePopup] No renderers found in model! Using default camera fallback.");
            // Fallback if no renderers are found immediately
            cam.transform.position = model.transform.position + new Vector3(0f, 0.8f, -7f);
            cam.transform.LookAt(model.transform.position + new Vector3(0f, 0.3f, 0f));
            cam.nearClipPlane = 0.3f;
            cam.farClipPlane = 20f;
            return;
        }

        Bounds bounds = renderers[0].bounds;
        bool hasValidBounds = false;

        if (bounds.size.magnitude > 0.001f)
        {
            hasValidBounds = true;
        }
        
        Debug.Log($"[Match3NotePopup] Renderer 0: {renderers[0].name}, Active: {renderers[0].gameObject.activeInHierarchy}, Enabled: {renderers[0].enabled}, LocalBounds: {bounds}, Layer: {renderers[0].gameObject.layer}");

        for (int i = 1; i < renderers.Length; i++)
        {
            Debug.Log($"[Match3NotePopup] Renderer {i}: {renderers[i].name}, Active: {renderers[i].gameObject.activeInHierarchy}, Enabled: {renderers[i].enabled}, LocalBounds: {renderers[i].bounds}, Layer: {renderers[i].gameObject.layer}");
            if (renderers[i].bounds.size.magnitude > 0.001f)
            {
                if (!hasValidBounds)
                {
                    bounds = renderers[i].bounds;
                    hasValidBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }
            }
        }

        if (!hasValidBounds)
        {
            Debug.LogWarning("[Match3NotePopup] No valid bounds (>0.001f) found among renderers! Using default camera fallback.");
            // Fallback if all bounds are zero
            cam.transform.position = model.transform.position + new Vector3(0f, 0.8f, -7f);
            cam.transform.LookAt(model.transform.position + new Vector3(0f, 0.3f, 0f));
            cam.nearClipPlane = 0.3f;
            cam.farClipPlane = 20f;
            return;
        }

        // Center of bounds
        Vector3 center = bounds.center;
        
        // Size of bounds
        float objectSize = bounds.size.magnitude;

        // Calculate distance based on FOV
        float fov = cam.fieldOfView;
        float distance = (objectSize / 2.0f) / Mathf.Sin(fov / 2.0f * Mathf.Deg2Rad);

        // Position camera back from center
        cam.transform.position = center + new Vector3(0f, objectSize * 0.15f, -distance * paddingFactor);
        cam.transform.LookAt(center);

        // Clip planes adjusted to distance
        cam.nearClipPlane = Mathf.Max(0.01f, distance * 0.05f);
        cam.farClipPlane = distance * 4.0f;
        
        Debug.Log($"[Match3NotePopup] Model Bounds Center: {center}, Size: {objectSize}, Cam Position: {cam.transform.position}, Cam NearClip: {cam.nearClipPlane}, Cam FarClip: {cam.farClipPlane}");
    }
}
