using System.Collections.Generic;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheProvider : ICacheProvider
    {
        private static object _lock = new object();

        private readonly IDictionary<string, object> _cache = new Dictionary<string, object>();

        public Task<T> Get<T>(string key)
        {
            lock (_lock)
            {
                object value;

                _cache.TryGetValue(key, out value);

                TypeHelpers.CheckType<T>(value);

                return Task.FromResult((T)value);
            }
        }

        public Task<bool> Put<T>(string key, T value)
        {
            lock (_lock)
            {
                _cache[key] = value;
            }

            return Task.FromResult(true);
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
                _cache.Clear();
            }

            return Task.FromResult(true);
        }
    }
}