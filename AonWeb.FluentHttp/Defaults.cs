using System;
using System.Net;
using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp
{
    public partial class Defaults
    {
        public static readonly Defaults Current = new Defaults();

        private readonly Lazy<HttpClientSettings> _client;
        private readonly Lazy<HttpBuilderDefaults> _httpBuilder;
        private readonly Lazy<TypedBuilderDefaults> _typedBuilder;
        private readonly Lazy<CachingDefaults> _caching;
        private readonly Lazy<HttpHandlerDefaults> _handlers;

        private Defaults()
        {
            _client = new Lazy<HttpClientSettings>(() => HttpClientSettings.Empty);
            _httpBuilder = new Lazy<HttpBuilderDefaults>(() => new HttpBuilderDefaults(this));
            _typedBuilder = new Lazy<TypedBuilderDefaults>(() => new TypedBuilderDefaults(this));
            _caching = new Lazy<CachingDefaults>(() => new CachingDefaults(this));
            _handlers = new Lazy<HttpHandlerDefaults>(() => new HttpHandlerDefaults(this));

            ResetRequested += (sender, args) => ResetClient();
        }

        public HttpClientSettings GetClientDefaults()
        {
            return _client.Value;
        }
        public HttpBuilderDefaults GetHttpBuilderDefaults()
        {
            return _httpBuilder.Value;
        }
        public TypedBuilderDefaults GetTypedBuilderDefaults()
        {
            return _typedBuilder.Value;
        }
        public CachingDefaults GetCachingDefaults()
        {
            return _caching.Value;
        }
        public HttpHandlerDefaults GetHandlerDefaults()
        {
            return _handlers.Value;
        }

        public EventHandler ResetRequested;

        public Defaults Reset()
        {
            var handler = ResetRequested;
            handler?.Invoke(this, EventArgs.Empty);

            return this;
        }

        private void ResetClient()
        {
            _client.Value.DecompressionMethods = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        } 
    }
}