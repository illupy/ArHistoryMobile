using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PreviewModelEntry
{
    public string modelCode;
    public GameObject prefab;
}

public class PreviewModelRegistry : MonoBehaviour
{
    public List<PreviewModelEntry> entries = new List<PreviewModelEntry>();

    public GameObject GetPrefabByCode(string modelCode)
    {
        if (string.IsNullOrEmpty(modelCode)) return null;

        foreach (var entry in entries)
        {
            if (entry != null && entry.modelCode == modelCode)
            {
                return entry.prefab;
            }
        }

        return null;
    }
}