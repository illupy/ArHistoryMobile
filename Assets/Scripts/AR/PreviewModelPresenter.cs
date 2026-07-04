using UnityEngine;

public class PreviewModelPresenter : MonoBehaviour
{
    [SerializeField] private PreviewModelRegistry previewModelRegistry;
    [SerializeField] private GameObject fallbackPrefab;

    private GameObject currentPreviewObject;
    private ARModelInteraction currentInteraction;

    public void ShowPreviewModel(string previewModelCode, Transform previewAnchor)
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
        currentPreviewObject.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);

        currentInteraction = currentPreviewObject.GetComponent<ARModelInteraction>();
        if (currentInteraction == null)
        {
            currentInteraction = currentPreviewObject.AddComponent<ARModelInteraction>();
        }

        currentInteraction.Setup(currentPreviewObject.transform);
    }

    public void ClearPreview()
    {
        if (currentPreviewObject != null)
        {
            Destroy(currentPreviewObject);
            currentPreviewObject = null;
        }

        currentInteraction = null;
    }

    public void ToggleDetachCurrentModel()
    {
        if (currentInteraction != null)
        {
            currentInteraction.ToggleDetach();
        }
    }

    public void ResetCurrentModel()
    {
        if (currentInteraction != null)
        {
            currentInteraction.ResetToMarker();
        }
    }
}