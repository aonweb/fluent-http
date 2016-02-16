using SQLite.Net.Interop;

namespace AonWeb.FluentHttp.Xamarin
{
    public interface IPlatformSettings
    {
        ISQLitePlatform SqLitePlatform { get; }
        string SqlLiteDbPath { get; }
    }

    public class PlatformSettings : IPlatformSettings
    {
        public PlatformSettings()
        {
            SqLitePlatform = null;
            SqlLiteDbPath = "";
        }

        public ISQLitePlatform SqLitePlatform { get; }
        public string SqlLiteDbPath { get; }
    }
}