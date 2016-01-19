using System.Threading.Tasks;
using SQLite.Net;
using XLabs.Caching;
using XLabs.Caching.SQLite;
using XLabs.Serialization;
using ICacheProvider = AonWeb.FluentHttp.Caching.ICacheProvider;

namespace AonWeb.FluentHttp.Xamarin
{
    public class SqlLiteCacheProvider : ICacheProvider
    {
        private static readonly object _lock = new object();

        private readonly ISimpleCache _cache;

        public SqlLiteCacheProvider(IPlatformSettings settings, IJsonSerializer serializer)
        {
            _cache = new SQLiteSimpleCache(settings.SqLitePlatform, new SQLiteConnectionString(settings.SqlLiteDbPath, true), serializer);
        }

        public Task<T> Get<T>(string key)
        {
            lock (_lock)
            {
                var result = _cache.Get<T>(key);

                return Task.FromResult(result);
            }
        }

        public Task<bool> Put<T>(string key, T value)
        {
            lock (_lock)
            {
                return Task.FromResult(_cache.Set(key, value));
            }
        }

        public Task<bool> Delete(string key)
        {
            lock (_lock)
            {
                return Task.FromResult(_cache.Remove(key));
            }
        }

        public Task DeleteAll()
        {
            lock (_lock)
            {
                _cache.FlushAll();
            }

            return Task.FromResult(true);
        }
    }
}