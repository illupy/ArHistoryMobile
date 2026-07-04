using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class RemoteVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage videoRawImage;
    public GameObject loadingText;
    public TextMeshProUGUI debugText;

    private void Awake()
    {
        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.waitForFirstFrame = true;
            videoPlayer.skipOnDrop = true;
            videoPlayer.source = VideoSource.Url;
            videoPlayer.renderMode = VideoRenderMode.APIOnly;

            videoPlayer.prepareCompleted += OnPrepared;
            videoPlayer.errorReceived += OnError;
            videoPlayer.started += OnStarted;
        }
    }

    public void PlayVideo(string url)
    {
        if (videoPlayer == null || videoRawImage == null) return;

        SetDebug("PlayVideo: " + url);

        if (loadingText != null) loadingText.SetActive(true);

        videoRawImage.texture = null;
        videoPlayer.Stop();
        videoPlayer.url = url;
        videoPlayer.Prepare();
    }

    private void OnPrepared(VideoPlayer source)
    {
        string msg = "Prepared. isPrepared=" + source.isPrepared;

        if (source.texture == null)
        {
            SetDebug(msg + " | texture=NULL");
            if (loadingText != null) loadingText.SetActive(false);
            return;
        }

        SetDebug(msg + $" | texture={source.texture.width}x{source.texture.height}");
        videoRawImage.texture = source.texture;

        if (loadingText != null) loadingText.SetActive(false);

        source.Play();
    }

    private void OnStarted(VideoPlayer source)
    {
        SetDebug("Video started");
    }

    private void OnError(VideoPlayer source, string message)
    {
        SetDebug("Video error: " + message);
        if (loadingText != null) loadingText.SetActive(false);
    }

    public void StopVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }

        if (videoRawImage != null)
        {
            videoRawImage.texture = null;
        }

        if (loadingText != null)
        {
            loadingText.SetActive(false);
        }

        SetDebug("Video stopped");
    }

    private void SetDebug(string msg)
    {
        if (debugText != null)
        {
            debugText.text = msg;
        }
    }
}