using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;

namespace AonWeb.FluentHttp
{
    public class HttpBuilderSettings : IHttpBuilderSettings
    {
        public HttpBuilderSettings()
        {
            UriBuilder = new HttpUriBuilder();
            Items = new Dictionary<string, object>();
            HandlerRegister = new HttpHandlerRegister();
            NormalizedUriQuery = new NormalizedUriQueryCollection();
            SuccessfulResponseValidators = new List<Func<HttpResponseMessage, bool>>();
            Method = Defaults.Current.GetHttpBuilderDefaults().HttpMethod;
            CompletionOption = Defaults.Current.GetHttpBuilderDefaults().CompletionOption;
            SuppressCancellationErrors = Defaults.Current.GetHttpBuilderDefaults().SuppressCancellationErrors;
            MediaType = Defaults.Current.GetHttpBuilderDefaults().MediaType;
            ContentEncoding = Defaults.Current.GetHttpBuilderDefaults().ContentEncoding;
            AutoDecompression = Defaults.Current.GetHttpBuilderDefaults().AutoDecompressionEnabled;
            CacheSettings = new CacheSettings();
        }

        public IDictionary Items { get; }
        public Type ResultType => typeof(HttpResponseMessage);

        public HttpUriBuilder UriBuilder { get; }
        public Uri Uri => UriBuilder.Uri;
        public NormalizedUriQueryCollection NormalizedUriQuery { get; set; }
        public HttpMethod Method { get; set; }
        public string MediaType { get; set; }
        public Encoding ContentEncoding { get; set; }
        public HttpCompletionOption CompletionOption { get; set; }
        public IRecursiveHttpBuilder Builder { get; protected internal set; }
        public Func<IHttpBuilderContext, HttpContent> ContentFactory { get; set; }
        public HttpHandlerRegister HandlerRegister { get; }
        public IList<Func<HttpResponseMessage, bool>> SuccessfulResponseValidators { get; }
        public Func<HttpResponseMessage, Exception> ExceptionFactory { get; set; }
        public bool SuppressCancellationErrors { get; set; }
        public bool AutoDecompression { get; set; }
        public CancellationToken Token { get; set; }
        public ICacheSettings CacheSettings { get; }

        public bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return !SuccessfulResponseValidators.Any() || SuccessfulResponseValidators.All(v => v(response));
        }

        public void Reset()
        {
            Items.Clear();
        }

        public void ValidateSettings()
        {
            if (!UriBuilder.IsSet)
                throw new InvalidOperationException("Uri not set");
        }

        //TODO: should this be a clone?
        public IHttpBuilderSettings GetSettings()
        {
            return this;
        }
    }
}