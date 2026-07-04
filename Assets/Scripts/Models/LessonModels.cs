using System;
using System.Collections.Generic;

[Serializable]
public class ApiResponse<T>
{
    public bool success;
    public T data;
    public string message;
}

[System.Serializable]
public class LessonDetailResponse
{
    public long id;
    public string title;
    public string description;
    public string content;
    public string thumbnailUrl;
    public string status;
    public string previewModelCode;
    public string previewAudioUrl;
    public bool hasQuiz;
    public bool hasGamification;

    public List<LessonAssetItem> assets;
    public List<LessonAnnotation> annotations;
}

[System.Serializable]
public class LessonAnnotation
{
    public long id;
    public string keyword;
    public string title;
    public string description;
    public string annotationType; // IMAGE, MODEL, VIDEO, etc.
    public string mediaUrl;
    public string modelCode;
    public int orderIndex;
}

[System.Serializable]
public class LessonAssetItem
{
    public long id;
    public string type;
    public string fileUrl;
    public string content;
    public int orderIndex;
    public List<string> mediaUrls;
}