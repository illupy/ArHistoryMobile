using UnityEngine;
using Vuforia;

public class MarkerDetectedTrigger : MonoBehaviour
{
    [SerializeField] private ARLessonLoader lessonLoader;

    private ObserverBehaviour observerBehaviour;
    private bool triggered = false;

    private void Start()
    {
        observerBehaviour = GetComponent<ObserverBehaviour>();

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
        if (triggered) return;

        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            triggered = true;

            if (lessonLoader != null)
            {
                lessonLoader.LoadLesson();
            }
        }
    }
}