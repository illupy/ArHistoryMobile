using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PreviewAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ARPreviewSceneManager arPreviewSceneManager;

    private Coroutine playRoutine;

    public void PlayPreviewAudio(string audioUrl)
    {
        if (string.IsNullOrEmpty(audioUrl))
        {
            Debug.Log("Không có audio preview, bật luôn nút bắt đầu bài học.");
            arPreviewSceneManager.OnPreviewVoiceCompleted();
            return;
        }

        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
        }

        playRoutine = StartCoroutine(LoadAndPlayAudio(audioUrl));
    }

    public void SkipPreviewAudio()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        arPreviewSceneManager.OnPreviewVoiceCompleted();
    }

    private IEnumerator LoadAndPlayAudio(string url)
    {
        using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Lỗi tải audio preview: " + request.error);
            arPreviewSceneManager.OnPreviewVoiceCompleted();
            yield break;
        }

        AudioClip clip = DownloadHandlerAudioClip.GetContent(request);

        if (audioSource == null)
        {
            Debug.LogError("AudioSource chưa được gán.");
            arPreviewSceneManager.OnPreviewVoiceCompleted();
            yield break;
        }

        audioSource.clip = clip;
        audioSource.Play();

        while (audioSource.isPlaying)
        {
            yield return null;
        }

        arPreviewSceneManager.OnPreviewVoiceCompleted();
    }
}