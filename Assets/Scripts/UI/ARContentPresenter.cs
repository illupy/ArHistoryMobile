using UnityEngine;

public class ARContentPresenter : MonoBehaviour
{
    public Transform arContentRoot;
    public GameObject defaultPrefab;

    private GameObject currentObject;

    public void ShowStep(LessonDetailResponse lesson, LessonAssetItem step)
    {
        if (currentObject != null)
        {
            Destroy(currentObject);
        }

        // Tạm thời luôn hiện default prefab
        if (defaultPrefab != null && arContentRoot != null)
        {
            currentObject = Instantiate(defaultPrefab, arContentRoot);
            currentObject.transform.localPosition = Vector3.zero;
            currentObject.transform.localRotation = Quaternion.identity;
            currentObject.transform.localScale = Vector3.one * 0.1f;
        }
    }
}