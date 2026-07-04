public static class AppConfig
{
    // Chỉ sửa IP ở đây
    public static string ServerHost = "http://103.178.235.163";

    public static string ApiBaseUrl => ServerHost + "/api";

    public static string BuildUrl(string path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        if (path.StartsWith("http")) return path;

        return ServerHost.TrimEnd('/') + "/" + path.TrimStart('/');
    }
}