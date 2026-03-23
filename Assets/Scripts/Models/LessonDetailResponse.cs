using System;
using System.Collections.Generic;

[Serializable]
public class ApiResponse<T>
{
    public bool success;
    public T data;
    public string message;
}

[Serializable]
public class LessonDetailResponse
{
    public long id;
    public string title;
    public string description;
    public string content;
    public string thumbnailUrl;
    public string status;
    public List<LessonAssetItem> assets;
}

[Serializable]
public class LessonAssetItem
{
    public long id;
    public string type;
    public string fileUrl;
    public string content;
    public int orderIndex;
}