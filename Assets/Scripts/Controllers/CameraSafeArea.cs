using UnityEngine;

namespace Core.CameraTools
{
    [RequireComponent(typeof(Camera))]
    public class CameraSafeArea : MonoBehaviour
    {
        private Camera cam;
        
        // The original world-space height and width the camera was designed to show.
        // By default, a camera with orthographicSize = 5 shows 10 units of height.
        public float requiredSafeHeight = 10f;
        
        // A minimal width to ensure the board fits horizontally on very narrow screens (like Galaxy Fold).
        public float requiredSafeWidth = 5.5f;

        private Rect lastSafeArea = Rect.zero;
        private Vector2 lastScreenSize = Vector2.zero;

        void Awake()
        {
            cam = GetComponent<Camera>();
        }

        void Start()
        {
            AdjustCamera();
        }

        void Update()
        {
            Rect safeArea = Screen.safeArea;
            if (safeArea != lastSafeArea || Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
            {
                AdjustCamera();
            }
        }

        void AdjustCamera()
        {
            if (cam == null || !cam.orthographic) return;

            Rect safeArea = Screen.safeArea;
            lastSafeArea = safeArea;
            lastScreenSize = new Vector2(Screen.width, Screen.height);

            // Calculate the ratio of the safe area compared to the full screen
            float safeHeightRatio = safeArea.height / Screen.height;
            float safeWidthRatio = safeArea.width / Screen.width;

            // Calculate target camera heights based on safe area ratios
            float targetHeightByH = requiredSafeHeight / safeHeightRatio;
            float targetHeightByW = (requiredSafeWidth / safeWidthRatio) / cam.aspect;

            // Increase camera size if the safe area doesn't provide enough space
            float targetOrthoSize = Mathf.Max(targetHeightByH, targetHeightByW) * 0.5f;
            
            // We ensure the camera never shrinks below the originally designed size of 5
            cam.orthographicSize = Mathf.Max(5f, targetOrthoSize);

            // Adjust camera Y position to center the required safe area perfectly
            float safeCenterY = safeArea.y + (safeArea.height * 0.5f);
            float offsetScreenY = safeCenterY - (Screen.height * 0.5f);
            float offsetWorldY = offsetScreenY * (cam.orthographicSize * 2f) / Screen.height;

            Vector3 pos = transform.position;
            pos.y = -offsetWorldY;
            transform.position = pos;
        }
    }
}
