using System.Runtime.InteropServices;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public static class TestHelper
    {
        [DllImport("WinInet.dll", PreserveSig = true, SetLastError = true)]
        public static extern void DeleteUrlCacheEntry(string url);
    }
}