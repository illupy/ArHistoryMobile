using UnityEngine;

public class PreviewModelPresenter : MonoBehaviour
{
    [SerializeField] private Transform previewAnchor;
    [SerializeField] private PreviewModelRegistry previewModelRegistry;
    [SerializeField] private GameObject fallbackPrefab;

    private GameObject currentPreviewObject;

    public void ShowPreviewModel(string previewModelCode)
    {
        ClearPreview();

        GameObject prefab = null;

        if (previewModelRegistry != null)
        {
            prefab = previewModelRegistry.GetPrefabByCode(previewModelCode);
        }

        if (prefab == null)
        {
            prefab = fallbackPrefab;
        }

        if (prefab == null || previewAnchor == null) return;

        currentPreviewObject = Instantiate(prefab, previewAnchor);
        currentPreviewObject.transform.localPosition = Vector3.zero;
        currentPreviewObject.transform.localRotation = Quaternion.identity;

        // chỉnh nhỏ lại
        currentPreviewObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    public void ClearPreview()
    {
        if (currentPreviewObject != null)
        {
            Destroy(currentPreviewObject);
            currentPreviewObject = null;
        }
    }
}