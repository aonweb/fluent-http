using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading;
using AonWeb.Fluent.Http.Handlers;

namespace AonWeb.Fluent.Http
{
    public class HttpCallBuilderSettings
    {
        private IList<IHttpCallHandler> _handlers;
        public HttpCallBuilderSettings()
        {
            Method = HttpMethod.Get;
            CompletionOption = HttpCompletionOption.ResponseContentRead;
            TokenSource = new CancellationTokenSource();
            _handlers = new List<IHttpCallHandler>();
        }

        public Uri Uri { get; set; }
        public NameValueCollection QueryString { get; set; }
        public HttpMethod Method { get; set; }
        public HttpCompletionOption CompletionOption { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
        public Func<HttpContent> Content { get; set; }

        public IEnumerable<IHttpCallHandler> Handlers { get { return _handlers; }}

        public void Validate()
        {
            if (Uri == null)
                throw new InvalidOperationException("Uri not set");
        }

        public HttpCallBuilderSettings AddHandler(IHttpCallHandler handler)
        {
            _handlers.Add(handler);

            return this;
        }

        public HttpCallBuilderSettings AddHandlers(IEnumerable<IHttpCallHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                 _handlers.Add(handler);
            }

            return this;
        }
        
        public HttpCallBuilderSettings AddHandlers(params IHttpCallHandler[] handlers)
        {
            return AddHandlers((IEnumerable<IHttpCallHandler>)handlers);
        }
    }

    public class HttpCallBuilderSettings<TResult>
    {
        private static readonly Func<TResult> _defaultDefaultResult = () => default(TResult);

        private Func<TResult> _defaultResult;

        public Func<TResult> DefaultResult
        {
            get
            {
                if (_defaultResult == null)
                    return _defaultDefaultResult;

                return _defaultResult;
            }
            set
            {
                _defaultResult = value;
            }
        }
    }
}