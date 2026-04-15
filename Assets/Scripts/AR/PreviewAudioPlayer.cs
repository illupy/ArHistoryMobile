using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PreviewAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ARPreviewSceneManager arPreviewSceneManager;

    private Coroutine currentRoutine;

    public void PlayPreviewAudio(string audioUrl)
    {
        StopPreviewAudio();

        if (string.IsNullOrEmpty(audioUrl))
        {
            arPreviewSceneManager.OnPreviewVoiceCompleted();
            return;
        }

        currentRoutine = StartCoroutine(LoadAndPlay(audioUrl));
    }

    public void SkipPreviewAudio()
    {
        StopPreviewAudio();
        arPreviewSceneManager.OnPreviewVoiceCompleted();
    }

    public void StopPreviewAudio()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private IEnumerator LoadAndPlay(string url)
    {
        using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Load preview audio failed: " + request.error);
            arPreviewSceneManager.OnPreviewVoiceCompleted();
            yield break;
        }

        AudioClip clip = DownloadHandlerAudioClip.GetContent(request);

        if (audioSource == null || clip == null)
        {
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