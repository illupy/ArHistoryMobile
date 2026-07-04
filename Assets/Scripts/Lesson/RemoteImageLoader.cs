using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RemoteImageLoader : MonoBehaviour
{
    public Image targetImage;
    public Action OnImageLoaded;

    public void LoadImage(string url)
    {
        StartCoroutine(LoadImageCoroutine(url));
    }

    private IEnumerator LoadImageCoroutine(string url)
    {
        if (targetImage == null)
        {
            Debug.LogError("RemoteImageLoader: targetImage is null");
            yield break;
        }

        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("RemoteImageLoader: image url is empty");
            yield break;
        }

        Debug.Log("Loading image: " + url);

        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Load image failed: " + request.error + " | URL = " + url);
            OnImageLoaded?.Invoke();
            yield break;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(request);

        if (texture == null)
        {
            Debug.LogError("Downloaded texture is null");
            OnImageLoaded?.Invoke();
            yield break;
        }

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );

        targetImage.sprite = sprite;
        targetImage.preserveAspect = true;
        targetImage.color = Color.white;

        Debug.Log("Image loaded OK: " + url);
        OnImageLoaded?.Invoke();
    }
}