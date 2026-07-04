using UnityEngine;

public class LessonSessionStore : MonoBehaviour
{
    public static LessonSessionStore Instance;

    public LessonDetailResponse CurrentLesson;
    public string CurrentMarkerCode;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetLesson(LessonDetailResponse lesson, string markerCode)
    {
        CurrentLesson = lesson;
        CurrentMarkerCode = markerCode;
    }

    public void ClearLesson()
    {
        CurrentLesson = null;
        CurrentMarkerCode = null;
    }
}