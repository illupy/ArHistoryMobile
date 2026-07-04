public static class MediaUrlHelper
{
    public static string ToFullUrl(string path)
    {
        return AppConfig.BuildUrl(path);
    }
}