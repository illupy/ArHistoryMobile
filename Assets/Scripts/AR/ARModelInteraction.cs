using UnityEngine;
using UnityEngine.InputSystem;

public class ARModelInteraction : MonoBehaviour
{
    [Header("Transform Settings")]
    public float rotationSpeed = 0.4f;
    public float scaleSpeed = 0.01f;
    public float minScale = 0.05f;
    public float maxScale = 10f;

    [Header("Vertical Rotation Limit")]
    public float minPitch = -60f;
    public float maxPitch = 60f;

    private Transform targetObject;

    private bool isDetached = false;
    private Transform originalParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;

    private float yaw;
    private float pitch;

    public void Setup(Transform target)
    {
        targetObject = target;

        if (targetObject != null)
        {
            originalParent = targetObject.parent;
            originalLocalPosition = targetObject.localPosition;
            originalLocalRotation = targetObject.localRotation;
            originalLocalScale = targetObject.localScale;

            Vector3 euler = targetObject.rotation.eulerAngles;
            yaw = euler.y;
            pitch = NormalizeAngle(euler.x);
        }
    }

    private void Update()
    {
        if (targetObject == null) return;
        if (!isDetached) return;

        // Run touch input first on mobile/WebGL touch browsers
        bool hadTouch = false;

        bool hasNewTouch = Touchscreen.current != null && Touchscreen.current.touches.Count > 0;
        bool hasLegacyTouch = Input.touchCount > 0;

        if (hasNewTouch || hasLegacyTouch)
        {
            HandleTouchInput();
            hadTouch = true;
        }

        // Fallback or run mouse input if no touch was handled (useful for editor and PC WebGL)
        if (!hadTouch)
        {
            HandleMouseInput();
        }
    }

    private void HandleMouseInput()
    {
        if (Mouse.current == null) return;

        Vector2 scroll = Mouse.current.scroll.ReadValue();
        if (Mathf.Abs(scroll.y) > 0.01f)
        {
            ScaleModel(scroll.y * scaleSpeed * 0.01f);
        }

        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            if (delta.sqrMagnitude > 0.001f)
            {
                RotateModel(delta.x, delta.y);
            }
        }
    }

    private void HandleTouchInput()
    {
        int activeTouchCount = 0;
        Vector2 pos0 = Vector2.zero;
        Vector2 pos1 = Vector2.zero;
        Vector2 delta0 = Vector2.zero;
        Vector2 delta1 = Vector2.zero;
        bool hasTouch = false;
        bool isMultiTouch = false;

        // 1. Try Touchscreen.current
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
                isMultiTouch = true;
            }
            else if (activeTouchCount == 1 && firstIndex >= 0)
            {
                pos0 = touches[firstIndex].position.ReadValue();
                delta0 = touches[firstIndex].delta.ReadValue();
                hasTouch = true;
                isMultiTouch = false;
            }
        }

        // 2. Try classic Input fallback
        if (!hasTouch)
        {
            if (Input.touchCount >= 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);
                pos0 = touch0.position;
                pos1 = touch1.position;
                delta0 = touch0.deltaPosition;
                delta1 = touch1.deltaPosition;
                activeTouchCount = Input.touchCount;
                hasTouch = true;
                isMultiTouch = true;
            }
            else if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                pos0 = touch.position;
                delta0 = touch.deltaPosition;
                activeTouchCount = 1;
                hasTouch = true;
                isMultiTouch = false;
            }
        }

        if (hasTouch)
        {
            if (isMultiTouch)
            {
                Vector2 prevPos0 = pos0 - delta0;
                Vector2 prevPos1 = pos1 - delta1;

                float prevMagnitude = (prevPos0 - prevPos1).magnitude;
                float currentMagnitude = (pos0 - pos1).magnitude;

                float difference = currentMagnitude - prevMagnitude;
                ScaleModel(difference * scaleSpeed);
            }
            else if (activeTouchCount == 1)
            {
                if (delta0.sqrMagnitude > 0.001f)
                {
                    RotateModel(delta0.x, delta0.y);
                }
            }
        }
    }

    private void ScaleModel(float increment)
    {
        Vector3 newScale = targetObject.localScale + Vector3.one * increment;
        newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
        newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
        newScale.z = Mathf.Clamp(newScale.z, minScale, maxScale);
        targetObject.localScale = newScale;
    }

    private void RotateModel(float deltaX, float deltaY)
    {
        yaw -= deltaX * rotationSpeed;
        pitch += deltaY * rotationSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        targetObject.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    public void ToggleDetach()
    {
        if (targetObject == null) return;

        isDetached = !isDetached;

        if (isDetached)
        {
            targetObject.SetParent(null, true);

            Vector3 euler = targetObject.rotation.eulerAngles;
            yaw = euler.y;
            pitch = NormalizeAngle(euler.x);
        }
        else
        {
            ResetToMarker();
        }
    }

    public void ResetToMarker()
    {
        if (targetObject == null || originalParent == null) return;

        isDetached = false;
        targetObject.SetParent(originalParent, false);
        targetObject.localPosition = originalLocalPosition;
        targetObject.localRotation = originalLocalRotation;
        targetObject.localScale = originalLocalScale;

        Vector3 euler = targetObject.rotation.eulerAngles;
        yaw = euler.y;
        pitch = NormalizeAngle(euler.x);
    }

    public bool IsDetached()
    {
        return isDetached;
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        return angle;
    }
}