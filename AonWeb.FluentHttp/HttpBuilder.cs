using System;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp
{
    public class HttpBuilder : IChildHttpBuilder
    {
        private readonly IHttpClientBuilder _clientBuilder;
        private CancellationTokenSource _tokenSource;

        public HttpBuilder(IHttpBuilderSettings settings, IHttpClientBuilder clientBuilder)
        {
            _clientBuilder = clientBuilder;

            Settings = settings;

            Settings.Builder = this;
        }

        public IAdvancedHttpBuilder Advanced => this;

        private IHttpBuilderSettings Settings { get; set; }

        public virtual void WithSettings(IHttpBuilderSettings settings)
        {
            Settings = settings;
        }

        public IHttpBuilder WithConfiguration(Action<IHttpBuilderSettings> configuration)
        {
            configuration?.Invoke(Settings);

            return this;
        }

        public IAdvancedHttpBuilder WithConfiguration(Action<ICacheSettings> configuration)
        {
            this.WithOptionalHandlerConfiguration<HttpCacheConfigurationHandler>(
                h => h.WithConfiguration(configuration));

            return this;
        }

        public IAdvancedHttpBuilder WithClientConfiguration(Action<IHttpClientBuilder> configuration)
        {
            configuration?.Invoke(_clientBuilder);

            return this;
        }

        void IConfigurable<IHttpBuilderSettings>.WithConfiguration(Action<IHttpBuilderSettings> configuration)
        {
            WithConfiguration(configuration);
        }
        void IConfigurable<ICacheSettings>.WithConfiguration(Action<ICacheSettings> configuration)
        {
            WithConfiguration(configuration);
        }

        public virtual async Task<HttpResponseMessage> ResultAsync(CancellationToken token)
        {
            HttpResponseMessage response = null;

            using (_tokenSource = ObjectHelpers.GetCancellationTokenSource(token))
            {
                Settings.Token = _tokenSource.Token;

                try
                {
                    response = await RecursiveResultAsync(Settings.Token);
                }
                catch (OperationCanceledException)
                {
                    if (!Settings.SuppressCancellationErrors)
                        throw;
                }
            }

            Reset();

            return response;
        }

        private void Reset()
        {
            Settings.Reset();
        }

        public async Task<HttpResponseMessage> RecursiveResultAsync(CancellationToken token)
        {
            var context = new HttpBuilderContext(Settings);

            using (var request = await CreateRequest(context))
            {
                token.ThrowIfCancellationRequested();

                return await ResultFromRequestAsync(context, request, token);
            }
        }

        public Task<HttpRequestMessage> CreateRequest()
        {
            var context = new HttpBuilderContext(Settings);

            return CreateRequest(context);
        }

        private async Task<HttpRequestMessage> CreateRequest(IHttpBuilderContext context)
        {
            var request = new HttpRequestMessage(context.Method, context.Uri);

            if (context.ContentFactory != null)
                request.Content = await context.ContentFactory?.Invoke(context);

            _clientBuilder.ApplyRequestHeaders(request);

            // if we haven't added an accept header, add a default
            if (!string.IsNullOrWhiteSpace(context.MediaType))
                request.Headers.Accept.AddDistinct(h => h.MediaType, context.MediaType);
            
            if (context.AutoDecompression)
            {
                request.Headers.AcceptEncoding.AddDistinct(h => h.Value, "gzip");
                request.Headers.AcceptEncoding.AddDistinct(h => h.Value, "deflate");
                request.Headers.AcceptEncoding.AddDistinct(h => h.Value, "identity");
            }

            return request;
        }

        public async Task<HttpResponseMessage> ResultFromRequestAsync(HttpRequestMessage request, CancellationToken token)
        {
            var context = new HttpBuilderContext(Settings);

            return await ResultFromRequestAsync(context, request, token).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> ResultFromRequestAsync(IHttpBuilderContext context, HttpRequestMessage request, CancellationToken token)
        {
            ExceptionDispatchInfo capturedException = null;
            HttpResponseMessage response = null;

            try
            {
                
                var sendingContext = new HttpSendingContext(context, request);

                token.ThrowIfCancellationRequested();

                await context.HandlerRegister.OnSending(sendingContext);

                if (sendingContext.Result != null)
                {
                    response = sendingContext.Result;
                }
                else
                {
                    token.ThrowIfCancellationRequested();

                    using (var client = _clientBuilder.Build())
                        response = await client.SendAsync(request, context.CompletionOption, token);
                }

                if (!context.IsSuccessfulResponse(response) && context.ExceptionFactory != null)
                {
                    var ex = context.ExceptionFactory(response, request);

                    if (ex != null)
                        throw ex;
                }

                var sentContext = new HttpSentContext(context, request, response);

                token.ThrowIfCancellationRequested();

                await context.HandlerRegister.OnSent(sentContext);

                response = sentContext.Result;
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            if (capturedException != null)
            {
                var exContext = new HttpExceptionContext(context, response, capturedException.SourceException);

                await context.HandlerRegister.OnException(exContext);

                if (!exContext.ExceptionHandled)
                    capturedException.Throw();
            }

            return response;
        }

        public void CancelRequest()
        {
            if (_tokenSource != null && !_tokenSource.IsCancellationRequested)
            {
                try
                {
                    _tokenSource?.Cancel();
                }
                catch (ObjectDisposedException)
                {
                    // if were done, this is disposed and I don't care.
                }
            }

        }
    }
}