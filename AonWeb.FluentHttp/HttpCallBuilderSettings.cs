using System.Net.Http.Headers;
using System.Text;
using AonWeb.FluentHttp.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading;

namespace AonWeb.FluentHttp
{
    public class HttpCallBuilderSettings
    {
        public HttpCallBuilderSettings()
        {
            Method = HttpMethod.Get;
            CompletionOption = HttpCompletionOption.ResponseContentRead;
            TokenSource = new CancellationTokenSource();
            Handler = new HttpCallHandlerRegister();

            SuccessfulResponseValidators = new List<Func<HttpResponseMessage, bool>>();
        }

        public Uri Uri { get; set; }
        public NameValueCollection QueryString { get; set; }
        public HttpMethod Method { get; set; }
        public string MediaType { get; set; }
        public Encoding ContentEncoding { get; set; }
        public HttpCompletionOption CompletionOption { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
        public Func<HttpContent> ContentFactory { get; set; }
        public HttpCallHandlerRegister Handler { get; private set; }
        public IList<Func<HttpResponseMessage, bool>> SuccessfulResponseValidators { get; private set; }
        public Func<HttpResponseMessage, Exception> ExceptionFactory { get; set; }

        public void ValidateSettings()
        {
            if (Uri == null)
                throw new InvalidOperationException("Uri not set");
        }

        public void AddSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator)
        {
            SuccessfulResponseValidators.Add(validator);
        }
    }

    public class HttpCallBuilderSettings<TResult, TContent, TError>
    {
        private static readonly Func<TResult> DefaultDefaultResultFactory = () => default(TResult);

        private Func<TResult> _defaultResultFactory;

        public HttpCallBuilderSettings()
        {
            
            Handler = new HttpCallHandlerRegister<TResult, TContent, TError>();

            SuccessfulResponseValidators = new List<Func<HttpResponseMessage, bool>>
            {
                DefaultSuccessfulResponseValidator.IsSuccessfulResponse
            };

            ExceptionFactory = DefaultExceptionFactory<TError>.CreateException;
        }

        public Func<TContent> ContentFactory { get; set; }
        public string MediaType { get; set; }
        public Encoding ContentEncoding { get; set; }
        public HttpCallHandlerRegister<TResult, TContent, TError> Handler { get; private set; }
        public IList<Func<HttpResponseMessage, bool>> SuccessfulResponseValidators { get; private set; }
        public Func<HttpErrorContext<TResult, TContent, TError>, Exception> ExceptionFactory { get; set; }

        public Func<TResult> DefaultResultFactory
        {
            get
            {
                if (_defaultResultFactory == null)
                    return DefaultDefaultResultFactory;

                return _defaultResultFactory;
            }
            set
            {
                _defaultResultFactory = value;
            }
        }

        public void AddSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator)
        {
            SuccessfulResponseValidators.Add(validator);
        }
    }
}