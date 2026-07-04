using UnityEngine;

namespace Core.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeArea : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Rect lastSafeArea = Rect.zero;
        private Vector2 lastScreenSize = Vector2.zero;
        private ScreenOrientation lastOrientation = ScreenOrientation.Unknown;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            Refresh();
        }

        void Update()
        {
            Refresh();
        }

        void Refresh()
        {
            Rect safeArea = Screen.safeArea;

            if (safeArea != lastSafeArea ||
                Screen.width != lastScreenSize.x ||
                Screen.height != lastScreenSize.y ||
                Screen.orientation != lastOrientation)
            {
                lastSafeArea = safeArea;
                lastScreenSize = new Vector2(Screen.width, Screen.height);
                lastOrientation = Screen.orientation;

                ApplySafeArea(safeArea);
            }
        }

        void ApplySafeArea(Rect r)
        {
            // Convert safe area rectangle from screen space to RectTransform anchor coordinates
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;

            if (Screen.width <= 0 || Screen.height <= 0) return;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            string logMessage = $"[SafeArea] Time: {Time.time}, Screen: {Screen.width}x{Screen.height}, SafeArea: {r}, anchorMin: {anchorMin}, anchorMax: {anchorMax}\n";
            try
            {
                System.IO.File.AppendAllText("safearea_debug.txt", logMessage);
            }
            catch {}
            Debug.Log(logMessage);
        }
    }
}
