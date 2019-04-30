using System;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _cache;
        private CancellationTokenSource _resetCacheToken;

        // TODO: this should use IOptions
        public CacheProvider(MemoryCacheOptions options)
        {
            _cache = new MemoryCache(options);
            _resetCacheToken = new CancellationTokenSource();
        }

        public Task<T> Get<T>(string key)
        {
            _cache.TryGetValue(key, out var value);

            TypeHelpers.CheckType<T>(value);

            return Task.FromResult((T)value);
        }

        public Task<bool> Put<T>(string key, T value, TimeSpan? expiration)
        {
            var changeToken = new CancellationChangeToken(_resetCacheToken.Token);

            // TODO: should be handled higher up and not cached at all?
            // if not, should this have a configurable, brief duration?
            expiration = expiration > TimeSpan.Zero ? expiration : null;

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration,
                ExpirationTokens = {changeToken},
            };

            _cache.Set(key, value, options);

            return Task.FromResult(true);
        }

        public Task<bool> Delete(string key)
        {
            _cache.Remove(key);

            return Task.FromResult(true);
        }

        public Task DeleteAll()
        {
            if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
            {
                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }

            _resetCacheToken = new CancellationTokenSource();

            return Task.CompletedTask;
        }
    }
}