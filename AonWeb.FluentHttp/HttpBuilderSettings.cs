using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public class HttpBuilderSettings : IHttpBuilderSettings
    {
        public HttpBuilderSettings()
        {
            UriBuilder = new HttpUriBuilder();
            Items = new Dictionary<string, object>();
            Handler = new HandlerRegister();
            UriQuery = new UriQueryCollection();
            SuccessfulResponseValidators = new List<Func<HttpResponseMessage, bool>>();
            Method = Defaults.Builder.HttpMethod;
            CompletionOption = Defaults.Builder.CompletionOption;
            SuppressCancellationErrors = Defaults.Builder.SuppressCancellationErrors;
            MediaType = Defaults.Builder.MediaType;
            ContentEncoding = Defaults.Builder.ContentEncoding;
            AutoDecompression = Defaults.Builder.AutoDecompressionEnabled;
        }

        public IDictionary Items { get; }
        public Type ResultType => typeof(HttpResponseMessage);

        public HttpUriBuilder UriBuilder { get; }
        public Uri Uri => UriBuilder.Uri;
        public UriQueryCollection UriQuery { get; set; }
        public HttpMethod Method { get; set; }
        public string MediaType { get; set; }
        public Encoding ContentEncoding { get; set; }
        public HttpCompletionOption CompletionOption { get; set; }
        public IRecursiveHttpBuilder Builder { get; protected internal set; }
        public Func<IHttpBuilderContext, HttpContent> ContentFactory { get; set; }
        public HandlerRegister Handler { get; }
        public IList<Func<HttpResponseMessage, bool>> SuccessfulResponseValidators { get; }
        public Func<HttpResponseMessage, Exception> ExceptionFactory { get; set; }
        public bool SuppressCancellationErrors { get; set; }
        public bool AutoDecompression { get; set; }
        public CancellationToken Token { get; set; }
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