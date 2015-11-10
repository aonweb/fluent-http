namespace AonWeb.FluentHttp.Serialization
{
    public static class UriStringHelpers
    {
        public static bool IsAbsolutePath(string pathAndQuery)
        {
            if (string.IsNullOrWhiteSpace(pathAndQuery))
                return true;

            return pathAndQuery.StartsWith("/");
        }

        public static string CombineVirtualPaths(string basePath, string relativePath)
        {
            return string.Concat(basePath.TrimEnd('/'), "/", relativePath.TrimStart('/'));
        }
    }
}