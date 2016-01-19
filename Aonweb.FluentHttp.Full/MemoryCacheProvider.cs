using System.Runtime.Caching;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Helpers;

namespace Aonweb.FluentHttp.Full
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private static readonly object _lock = new object();

        private readonly ObjectCache _cache = new MemoryCache("Aonweb.FluentHttp.MemoryCacheProvider");

        public Task<T> Get<T>(string key)
        {
            lock (_lock)
            {
                var value = _cache.Get(key);

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
                var result = _cache.Remove(key);

                return Task.FromResult(result != null);
            }
        }

        public Task DeleteAll()
        {
            lock (_lock)
            {
                foreach (var kp in _cache)
                    _cache.Remove(kp.Key);
            }

            return Task.FromResult(true);
        }
    }
}
