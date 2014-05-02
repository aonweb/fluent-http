using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public class HttpCallBuilderSettings
    {
        private static readonly HashSet<HttpCallHandlerType> ValidHandlerTypes = new HashSet<HttpCallHandlerType> { HttpCallHandlerType.Sending, HttpCallHandlerType.Sent, HttpCallHandlerType.Exception };

        private readonly IDictionary<HttpCallHandlerType, ISet<IHttpCallHandler<HttpCallContext>>> _handlers;
        private readonly IList<Func<HttpResponseMessage, bool>> _responseValidators;

        public HttpCallBuilderSettings()
        {
            Method = HttpMethod.Get;
            CompletionOption = HttpCompletionOption.ResponseContentRead;
            TokenSource = new CancellationTokenSource();

            _handlers = new Dictionary<HttpCallHandlerType, ISet<IHttpCallHandler<HttpCallContext>>>();

            foreach (var handlerType in ValidHandlerTypes)
                _handlers.Add(handlerType, new HashSet<IHttpCallHandler<HttpCallContext>>());

            _responseValidators = new List<Func<HttpResponseMessage, bool>>();
        }

        public Uri Uri { get; set; }
        public NameValueCollection QueryString { get; set; }
        public HttpMethod Method { get; set; }
        public HttpCompletionOption CompletionOption { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
        public Func<HttpContent> Content { get; set; }

        public IDictionary<HttpCallHandlerType, ISet<IHttpCallHandler<HttpCallContext>>> Handlers { get { return _handlers; } }
        public IEnumerable<Func<HttpResponseMessage, bool>> SuccessfulResponseValidators { get { return _responseValidators; } }

        public Func<HttpResponseMessage, Exception> ExceptionFactory { get; set; }

        public void ValidateSettings()
        {
            if (Uri == null)
                throw new InvalidOperationException("Uri not set");
        }

        public HttpCallBuilderSettings AddHandler(HttpCallHandlerType handlerType, HttpCallHandlerPriority priority, Func<HttpCallContext, Task> handler)
        {
            AddHandler(new HttpCallHandlerFunc(handlerType, priority, handler));

            return this;
        }

        public HttpCallBuilderSettings AddHandler(IHttpCallHandler<HttpCallContext> handler)
        {

            if (!ValidHandlerTypes.Contains(handler.HandlerType))
                throw new NotSupportedException(string.Format(SR.HandlerTypeNotSupportedErrorFormat, handler.HandlerType));

            if (_handlers[handler.HandlerType].Contains(handler))
                throw new InvalidOperationException(string.Format(SR.HanderAlreadyExistsErrorFormat, handler.HandlerType));

            _handlers[handler.HandlerType].Add(handler);

            return this;
        }

        public HttpCallBuilderSettings AddHandlers(IEnumerable<IHttpCallHandler<HttpCallContext>> handlers)
        {
            foreach (var handler in handlers)
                AddHandler(handler);

            return this;
        }

        public HttpCallBuilderSettings AddHandlers(params IHttpCallHandler<HttpCallContext>[] handlers)
        {
            return AddHandlers((IEnumerable<IHttpCallHandler<HttpCallContext>>)handlers);
        }

        public void AddSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator)
        {
            _responseValidators.Add(validator);
        }
    }

    public class HttpCallBuilderSettings<TResult, TContent, TError>
    {
        private static readonly Func<TResult> DefaultDefaultResult = () => default(TResult);

        private Func<TResult> _defaultResult;

        private readonly IDictionary<HttpCallHandlerType, ISet<IHttpCallHandler<HttpCallContext<TResult, TContent, TError>>>> _handlers;

        private readonly IList<Func<HttpResponseMessage, bool>> _responseValidators;

        public HttpCallBuilderSettings()
        {
            _handlers = new ConcurrentDictionary<HttpCallHandlerType, ISet<IHttpCallHandler<HttpCallContext<TResult, TContent, TError>>>>();

            foreach (var handlerType in Enum.GetValues(typeof(HttpCallHandlerType)).Cast<HttpCallHandlerType>())
                _handlers.Add(handlerType, new HashSet<IHttpCallHandler<HttpCallContext<TResult, TContent, TError>>>());

            _responseValidators = new List<Func<HttpResponseMessage, bool>>
            {
                DefaultSuccessfulResponseValidator.IsSuccessfulResponse
            };

            ExceptionFactory = DefaultExceptionFactory<TError>.CreateException;
        }

        public IDictionary<HttpCallHandlerType, ISet<IHttpCallHandler<HttpCallContext<TResult, TContent, TError>>>> Handlers { get { return _handlers; } }
        public IEnumerable<Func<HttpResponseMessage, bool>> SuccessfulResponseValidators { get { return _responseValidators; } }
        public Func<HttpErrorContext<TResult, TContent, TError>, Exception> ExceptionFactory { get; set; }

        public Func<TResult> DefaultResult
        {
            get
            {
                if (_defaultResult == null)
                    return DefaultDefaultResult;

                return _defaultResult;
            }
            set
            {
                _defaultResult = value;
            }
        }

        public HttpCallBuilderSettings<TResult, TContent, TError> AddHandler(HttpCallHandlerType handlerType, HttpCallHandlerPriority priority, Func<HttpCallContext<TResult, TContent, TError>, Task> handler)
        {
            AddHandler(new HttpCallHandlerFunc<TResult, TContent, TError>(handlerType, priority, handler));

            return this;
        }

        public HttpCallBuilderSettings<TResult, TContent, TError> AddHandler(IHttpCallHandler<HttpCallContext<TResult, TContent, TError>> handler)
        {
            if (_handlers[handler.HandlerType].Contains(handler))
                throw new InvalidOperationException(string.Format(SR.HanderAlreadyExistsErrorFormat, handler.HandlerType));

            _handlers[handler.HandlerType].Add(handler);

            return this;
        }

        public HttpCallBuilderSettings<TResult, TContent, TError> AddHandlers(IEnumerable<IHttpCallHandler<HttpCallContext<TResult, TContent, TError>>> handlers)
        {
            foreach (var handler in handlers)
                AddHandler(handler);

            return this;
        }

        public HttpCallBuilderSettings<TResult, TContent, TError> AddHandlers(params IHttpCallHandler<HttpCallContext<TResult, TContent, TError>>[] handlers)
        {
            return AddHandlers((IEnumerable<IHttpCallHandler<HttpCallContext<TResult, TContent, TError>>>)handlers);
        }

        public void AddSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator)
        {
            _responseValidators.Add(validator);
        }
    }
}