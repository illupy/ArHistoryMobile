using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class DynamicSpriteManager : MonoBehaviour
{
    private static bool m_shuttingDown = false;
    private static DynamicSpriteManager _instance;
    public static DynamicSpriteManager Instance
    {
        get
        {
            if (m_shuttingDown)
            {
                return null;
            }
            if (_instance == null)
            {
                GameObject go = new GameObject("DynamicSpriteManager");
                _instance = go.AddComponent<DynamicSpriteManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // Dictionary mapping item type to loaded dynamic Sprites (each list contains exactly 3 sprites)
    private Dictionary<NormalItem.eNormalType, List<Sprite>> m_cachedSprites = new Dictionary<NormalItem.eNormalType, List<Sprite>>();

    // Mapping item type to their dynamic server URLs (each list contains exactly 3 URLs)
    private Dictionary<NormalItem.eNormalType, List<string>> m_imageUrlSettings = new Dictionary<NormalItem.eNormalType, List<string>>();

    // Mapping item type to its historical note/information
    private Dictionary<NormalItem.eNormalType, string> m_noteSettings = new Dictionary<NormalItem.eNormalType, string>();

    // Mapping item type to its 3D model code
    private Dictionary<NormalItem.eNormalType, string> m_noteModelCodeSettings = new Dictionary<NormalItem.eNormalType, string>();

    public event Action OnSpritesLoaded = delegate { };

    public bool IsLoaded { get; private set; } = false;

    public float Progress { get; private set; } = 0f;

    private string CacheDirectoryPath => Path.Combine(Application.persistentDataPath, "DynamicSpriteCache");

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure cache directory exists
        if (!Directory.Exists(CacheDirectoryPath))
        {
            Directory.CreateDirectory(CacheDirectoryPath);
        }

        // Initialize lists
        var allTypes = (NormalItem.eNormalType[])System.Enum.GetValues(typeof(NormalItem.eNormalType));
        foreach (var type in allTypes)
        {
            m_cachedSprites[type] = new List<Sprite> { null, null, null };
            m_imageUrlSettings[type] = new List<string> { null, null, null };
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void OnApplicationQuit()
    {
        m_shuttingDown = true;
    }

    /// <summary>
    /// Configure URLs for item types. You can call this from your Server Config / Supabase Manager.
    /// </summary>
    public void SetImageUrl(NormalItem.eNormalType type, string url)
    {
        if (m_imageUrlSettings.ContainsKey(type))
        {
            m_imageUrlSettings[type][0] = url;
        }
    }

    /// <summary>
    /// Configure all 3 URLs for an item type.
    /// </summary>
    public void SetImageUrls(NormalItem.eNormalType type, List<string> urls)
    {
        if (urls != null)
        {
            m_imageUrlSettings[type] = urls;
            if (!m_cachedSprites.ContainsKey(type) || m_cachedSprites[type].Count < urls.Count)
            {
                m_cachedSprites[type] = new List<Sprite>();
                for (int i = 0; i < urls.Count; i++)
                {
                    m_cachedSprites[type].Add(null);
                }
            }
        }
    }

    /// <summary>
    /// Gets the cached dynamic sprite for a given type. Returns null if not loaded yet.
    /// </summary>
    public Sprite GetSprite(NormalItem.eNormalType type)
    {
        return GetSprite(type, 0);
    }

    /// <summary>
    /// Gets the cached dynamic sprite for a given type and image index. Returns null if not loaded yet.
    /// </summary>
    public Sprite GetSprite(NormalItem.eNormalType type, int imageIndex)
    {
        if (m_cachedSprites.TryGetValue(type, out List<Sprite> sprites))
        {
            if (imageIndex >= 0 && imageIndex < sprites.Count)
            {
                return sprites[imageIndex];
            }
        }
        return null;
    }

    /// <summary>
    /// Loads all registered dynamic sprites from local cache or server.
    /// </summary>
    /// <param name="onComplete">Callback triggered when all downloads/loads complete (returns success count)</param>
    public void LoadAllDynamicSprites(Action<int> onComplete)
    {
        StartCoroutine(FetchSetsAndLoadSpritesCoroutine(onComplete));
    }

    private IEnumerator FetchSetsAndLoadSpritesCoroutine(Action<int> onComplete)
    {
        IsLoaded = false;
        bool fetchSuccess = false;
        yield return StartCoroutine(LoadGameSetsCoroutine((success) => {
            fetchSuccess = success;
        }));

        if (!fetchSuccess)
        {
            Debug.LogWarning("[DynamicSpriteManager] Failed to fetch sets from API, trying to load with existing or fallback URLs.");
        }

        yield return StartCoroutine(LoadAllSpritesCoroutine((successCount) => {
            IsLoaded = true;
            onComplete?.Invoke(successCount);
            OnSpritesLoaded?.Invoke();
        }));
    }

    [Serializable]
    public class Match3GameResponse
    {
        public bool success;
        public Match3GameData data;
        public string message;
    }

    [Serializable]
    public class Match3GameData
    {
        public List<Match3Set> sets;
    }

    [Serializable]
    public class Match3Set
    {
        public int id;
        public string imageUrl1;
        public string imageUrl2;
        public string imageUrl3;
        public string note;
        public string noteModelCode;
    }

    private IEnumerator LoadGameSetsCoroutine(Action<bool> onComplete)
    {
        string url = $"{AppConfig.ApiBaseUrl}/match3/game";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Thiết lập các Header giả lập trình duyệt để tránh lỗi 403 Forbidden
            // request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            // request.SetRequestHeader("Accept", "application/json, text/plain, */*");
            // request.SetRequestHeader("Accept-Language", "vi-VN,vi;q=0.9,en-US;q=0.8,en;q=0.7");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string json = request.downloadHandler.text;
                    Debug.Log($"[DynamicSpriteManager] Raw API response: {json}");
                    var response = JsonConvert.DeserializeObject<Match3GameResponse>(json);
                    if (response != null && response.success && response.data != null && response.data.sets != null)
                    {
                        Debug.Log($"[DynamicSpriteManager] Received {response.data.sets.Count} sets from backend");
                        // Log each set to detect duplicates from the backend
                        HashSet<int> seenIds = new HashSet<int>();
                        for (int s = 0; s < response.data.sets.Count; s++)
                        {
                            var set = response.data.sets[s];
                            bool isDuplicate = !seenIds.Add(set.id);
                            Debug.Log($"[DynamicSpriteManager] Set[{s}]: id={set.id}, url1={set.imageUrl1}, url2={set.imageUrl2}, url3={set.imageUrl3}{(isDuplicate ? " ** DUPLICATE ID **" : "")}");
                        }
                        SetupSets(response.data.sets);
                        onComplete?.Invoke(true);
                        yield break;
                    }
                    else
                    {
                        Debug.LogError($"[DynamicSpriteManager] API response parsed but invalid: success={response?.success}, data null={response?.data == null}, sets null={response?.data?.sets == null}");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("[DynamicSpriteManager] Error parsing API response: " + ex.Message);
                }
            }
            else
            {
                Debug.LogError($"[DynamicSpriteManager] API request to url {url} failed: " + request.error + " (Response Code: " + request.responseCode + ")");
            }
        }
        onComplete?.Invoke(false);
    }

    private void SetupSets(List<Match3Set> sets)
    {
        if (sets == null || sets.Count == 0)
        {
            Debug.LogWarning("[DynamicSpriteManager] SetupSets: sets is null or empty!");
            return;
        }

        var allTypes = (NormalItem.eNormalType[])System.Enum.GetValues(typeof(NormalItem.eNormalType));

        if (sets.Count < allTypes.Length)
        {
            Debug.LogWarning($"[DynamicSpriteManager] Backend returned {sets.Count} sets but there are {allTypes.Length} item types. Some types will have no images!");
        }

        for (int i = 0; i < allTypes.Length; i++)
        {
            NormalItem.eNormalType type = allTypes[i];

            if (i < sets.Count)
            {
                Match3Set set = sets[i];
                List<string> urls = new List<string> { set.imageUrl1, set.imageUrl2, set.imageUrl3 };
                m_imageUrlSettings[type] = urls;
                m_cachedSprites[type] = new List<Sprite> { null, null, null };
                m_noteSettings[type] = set.note;
                m_noteModelCodeSettings[type] = set.noteModelCode;
                Debug.Log($"[DynamicSpriteManager] Mapped {type} -> set id={set.id} (urls: {set.imageUrl1}, {set.imageUrl2}, {set.imageUrl3})");
            }
            else
            {
                Debug.LogWarning($"[DynamicSpriteManager] No set available for {type} (index {i}), this type will have no dynamic sprites!");
            }
        }
    }

    public string GetNote(NormalItem.eNormalType type)
    {
        if (m_noteSettings.TryGetValue(type, out string note))
        {
            return note;
        }
        return string.Empty;
    }

    public string GetNoteModelCode(NormalItem.eNormalType type)
    {
        if (m_noteModelCodeSettings.TryGetValue(type, out string modelCode))
        {
            return modelCode;
        }
        return string.Empty;
    }

    private IEnumerator LoadAllSpritesCoroutine(Action<int> onComplete)
    {
        int successCount = 0;
        int totalToLoad = 0;
        foreach (var kvp in m_imageUrlSettings)
        {
            if (kvp.Value != null)
            {
                totalToLoad += kvp.Value.Count;
            }
        }

        Progress = 0f;

        if (totalToLoad == 0)
        {
            Progress = 1f;
            onComplete?.Invoke(0);
            yield break;
        }

        int processedCount = 0;
        foreach (var kvp in m_imageUrlSettings)
        {
            NormalItem.eNormalType type = kvp.Key;
            List<string> urls = kvp.Value;

            if (urls == null) continue;

            for (int i = 0; i < urls.Count; i++)
            {
                string url = urls[i];
                if (string.IsNullOrEmpty(url))
                {
                    processedCount++;
                    Progress = (float)processedCount / totalToLoad;
                    continue;
                }

                // Step 1: Check local persistent disk cache first
                string localFileName = $"{type}_{i}_{HashUrl(url)}.png";
                string localFilePath = Path.Combine(CacheDirectoryPath, localFileName);

                if (File.Exists(localFilePath))
                {
                    // Load from disk offline
                    byte[] fileData = File.ReadAllBytes(localFilePath);
                    Texture2D texture = new Texture2D(2, 2);
                    if (texture.LoadImage(fileData))
                    {
                        Sprite sprite = CreateSpriteFromTexture(texture);
                        
                        // Destroy old sprite and texture before replacing to avoid native memory leak
                        if (m_cachedSprites.TryGetValue(type, out var list) && i < list.Count && list[i] != null)
                        {
                            if (list[i].texture != null) Destroy(list[i].texture);
                            Destroy(list[i]);
                        }

                        m_cachedSprites[type][i] = sprite;
                        successCount++;
                        
                        processedCount++;
                        Progress = (float)processedCount / totalToLoad;
                        continue; // Skip web request since we loaded it successfully
                    }
                }

                // Step 2: Download from Server URL
                using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
                {
                    webRequest.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                    yield return webRequest.SendWebRequest();

                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(webRequest);
                        if (downloadedTexture != null)
                        {
                            // Save in memory
                            Sprite sprite = CreateSpriteFromTexture(downloadedTexture);
                            
                            // Destroy old sprite and texture before replacing to avoid native memory leak
                            if (m_cachedSprites.TryGetValue(type, out var list) && i < list.Count && list[i] != null)
                            {
                                if (list[i].texture != null) Destroy(list[i].texture);
                                Destroy(list[i]);
                            }

                            m_cachedSprites[type][i] = sprite;
                            successCount++;

                            // Save to disk cache for future offline launches
                            try
                            {
                                byte[] bytes = downloadedTexture.EncodeToPNG();
                                File.WriteAllBytes(localFilePath, bytes);
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError($"[DynamicSpriteManager] Failed to cache sprite to disk: {ex.Message}");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[DynamicSpriteManager] Failed to download sprite for {type} at index {i} from URL: {url}. Error: {webRequest.error}");
                    }
                }

                processedCount++;
                Progress = (float)processedCount / totalToLoad;
            }
        }

        Progress = 1f;
        onComplete?.Invoke(successCount);
    }

    private Sprite CreateSpriteFromTexture(Texture2D texture)
    {
        // Calculate PPU dynamically so the largest side of the image fits exactly 135 pixels (1.35 world units)
        // This makes the icon larger, filling the grid cell beautifully.
        float maxDimension = Mathf.Max(texture.width, texture.height);
        float ppu = (maxDimension / 135f) * 100f;

        // Use a pivot of (0.5, 0.5) to keep it centered
        return Sprite.Create(
            texture, 
            new Rect(0.0f, 0.0f, texture.width, texture.height), 
            new Vector2(0.5f, 0.5f), 
            ppu
        );
    }

    private string HashUrl(string url)
    {
        // Simple hash helper to generate unique safe filename for URL
        return url.GetHashCode().ToString("X");
    }

    /// <summary>
    /// Helper method to clear both memory cache and local persistent cache files
    /// </summary>
    public void ClearCache()
    {
        foreach (var kvp in m_cachedSprites)
        {
            if (kvp.Value != null)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    if (kvp.Value[i] != null)
                    {
                        if (kvp.Value[i].texture != null) Destroy(kvp.Value[i].texture);
                        Destroy(kvp.Value[i]);
                        kvp.Value[i] = null;
                    }
                }
            }
        }
        try
        {
            if (Directory.Exists(CacheDirectoryPath))
            {
                Directory.Delete(CacheDirectoryPath, true);
                Directory.CreateDirectory(CacheDirectoryPath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[DynamicSpriteManager] Failed to clear disk cache: {ex.Message}");
        }
    }
}
