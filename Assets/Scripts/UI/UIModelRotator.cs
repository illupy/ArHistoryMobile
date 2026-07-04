using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIModelRotator : MonoBehaviour, IDragHandler, IScrollHandler
{
    [HideInInspector] public Transform targetModel;
    public float rotationSpeed = 0.4f;
    public float scaleSpeed = 0.05f;
    
    public float minPitch = -60f;
    public float maxPitch = 60f;

    private float m_minScale;
    private float m_maxScale;
    private float m_yaw;
    private float m_pitch;
    private RectTransform m_rectTransform;

    public void Init(Transform target, float initialScale)
    {
        targetModel = target;
        m_minScale = initialScale * 0.2f;
        m_maxScale = initialScale * 5f;

        Vector3 euler = targetModel.rotation.eulerAngles;
        m_yaw = euler.y;
        m_pitch = NormalizeAngle(euler.x);
    }

    private void Start()
    {
        m_rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (targetModel == null) return;

        int activeTouchCount = 0;
        Vector2 pos0 = Vector2.zero;
        Vector2 pos1 = Vector2.zero;
        Vector2 delta0 = Vector2.zero;
        Vector2 delta1 = Vector2.zero;
        bool hasTouch = false;

        // 1. Try Touchscreen.current (New Input System)
        if (Touchscreen.current != null)
        {
            var touches = Touchscreen.current.touches;
            int firstIndex = -1;
            int secondIndex = -1;

            for (int i = 0; i < touches.Count; i++)
            {
                if (touches[i].press.isPressed)
                {
                    if (firstIndex == -1) firstIndex = i;
                    else if (secondIndex == -1) secondIndex = i;
                    activeTouchCount++;
                }
            }

            if (activeTouchCount >= 2 && firstIndex >= 0 && secondIndex >= 0)
            {
                pos0 = touches[firstIndex].position.ReadValue();
                pos1 = touches[secondIndex].position.ReadValue();
                delta0 = touches[firstIndex].delta.ReadValue();
                delta1 = touches[secondIndex].delta.ReadValue();
                hasTouch = true;
            }
        }

        // 2. Fallback to classic Input (WebGL / older setups where Touchscreen.current is null)
        if (!hasTouch && Input.touchCount >= 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            pos0 = touch0.position;
            pos1 = touch1.position;
            delta0 = touch0.deltaPosition;
            delta1 = touch1.deltaPosition;
            activeTouchCount = Input.touchCount;
            hasTouch = true;
        }

        if (hasTouch)
        {
            // Ensure at least one touch is inside the model interactive viewport area
            if (m_rectTransform != null)
            {
                bool touch0Inside = RectTransformUtility.RectangleContainsScreenPoint(m_rectTransform, pos0, null);
                bool touch1Inside = RectTransformUtility.RectangleContainsScreenPoint(m_rectTransform, pos1, null);
                if (!touch0Inside && !touch1Inside) return;
            }

            Vector2 prevPos0 = pos0 - delta0;
            Vector2 prevPos1 = pos1 - delta1;

            float prevMagnitude = (prevPos0 - prevPos1).magnitude;
            float currentMagnitude = (pos0 - pos1).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            // scaleSpeed is 0.05f. Multiply by 0.1f to adjust sensitivity.
            ScaleModel(difference * scaleSpeed * 0.1f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Ignore single touch drag rotation if pinching
        bool isPinching = false;
        if (Touchscreen.current != null)
        {
            int activeTouchCount = 0;
            for (int i = 0; i < Touchscreen.current.touches.Count; i++)
            {
                if (Touchscreen.current.touches[i].press.isPressed)
                {
                    activeTouchCount++;
                }
            }
            if (activeTouchCount >= 2) isPinching = true;
        }
        if (!isPinching && Input.touchCount >= 2)
        {
            isPinching = true;
        }

        if (isPinching) return;

        if (targetModel != null)
        {
            m_yaw -= eventData.delta.x * rotationSpeed;
            m_pitch += eventData.delta.y * rotationSpeed;
            m_pitch = Mathf.Clamp(m_pitch, minPitch, maxPitch);

            targetModel.rotation = Quaternion.Euler(m_pitch, m_yaw, 0f);
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (targetModel != null)
        {
            ScaleModel(eventData.scrollDelta.y * scaleSpeed * 0.1f);
        }
    }

    private void ScaleModel(float increment)
    {
        Vector3 newScale = targetModel.localScale + Vector3.one * increment;
        newScale.x = Mathf.Clamp(newScale.x, m_minScale, m_maxScale);
        newScale.y = Mathf.Clamp(newScale.y, m_minScale, m_maxScale);
        newScale.z = Mathf.Clamp(newScale.z, m_minScale, m_maxScale);
        targetModel.localScale = newScale;
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        return angle;
    }
}
