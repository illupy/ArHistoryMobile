using UnityEngine;
using Vuforia;

public class MarkerDetectedTrigger : MonoBehaviour
{
    private ObserverBehaviour observerBehaviour;
    private ARLessonLoader lessonLoader;

    private void Start()
    {
        observerBehaviour = GetComponent<ObserverBehaviour>();
        lessonLoader = GetComponent<ARLessonLoader>();

        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnDestroy()
    {
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            lessonLoader.LoadLesson();
        }
    }
}