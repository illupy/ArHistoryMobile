using UnityEngine;
using Vuforia;

public class MarkerDetectedTrigger : MonoBehaviour
{
    [SerializeField] private ARLessonLoader lessonLoader;
    [SerializeField] private Transform previewAnchor;

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

            string markerCode = GetMarkerCodeFromTarget(behaviour);

            if (lessonLoader != null && !string.IsNullOrEmpty(markerCode))
            {
                lessonLoader.LoadLesson(markerCode, previewAnchor);
            }
        }
    }

    private string GetMarkerCodeFromTarget(ObserverBehaviour behaviour)
    {
        if (behaviour == null) return null;

        if (!string.IsNullOrEmpty(behaviour.TargetName))
        {
            return behaviour.TargetName;
        }

        return behaviour.gameObject.name;
    }

    public void ResetTrigger()
    {
        triggered = false;
    }
}