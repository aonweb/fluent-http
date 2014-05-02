using System.Runtime.InteropServices;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public static class Helper
    {
        [DllImport("WinInet.dll", PreserveSig = true, SetLastError = true)]
        public static extern void DeleteUrlCacheEntry(string url);
    }
}