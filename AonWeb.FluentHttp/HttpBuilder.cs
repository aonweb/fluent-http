using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp
{

    public class HttpBuilder : IChildHttpBuilder
    {
        private readonly IHttpClientBuilder _clientBuilder;
        private CancellationTokenSource _tokenSource;

        public HttpBuilder(IHttpBuilderSettings settings, IHttpClientBuilder clientBuilder, IReadOnlyCollection<IHandler> defaultHandlers)
        {
            Settings = settings;
            _clientBuilder = clientBuilder;

            foreach (var handler in defaultHandlers)
                Settings.Handler.WithHandler(handler);
        }

        public IAdvancedHttpBuilder Advanced => this;

        private IHttpBuilderSettings Settings { get; }

        public IHttpBuilder WithConfiguration(Action<IHttpBuilderSettings> configuration)
        {
            configuration?.Invoke(Settings);

            return this;
        }

        void IConfigurable<IHttpBuilderSettings>.WithConfiguration(Action<IHttpBuilderSettings> configuration)
        {
            WithConfiguration(configuration);
        }

        public IAdvancedHttpBuilder WithClientConfiguration(Action<IHttpClientBuilder> configuration)
        {
            configuration?.Invoke(_clientBuilder);

            return this;
        }

        public virtual Task<HttpResponseMessage> ResultAsync()
        {
            return ResultAsync(CancellationToken.None);
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

            using (var request = CreateRequest(context))
            {
                token.ThrowIfCancellationRequested();

                return await ResultFromRequestAsync(context, request, token);
            }
        }

        public HttpRequestMessage CreateRequest()
        {
            var context = new HttpBuilderContext(Settings);

            return CreateRequest(context);
        }

        private HttpRequestMessage CreateRequest(IHttpBuilderContext context)
        {
            context.ValidateSettings();

            var request = new HttpRequestMessage(context.Method, context.Uri);

            if (context.ContentFactory != null)
                request.Content = context.ContentFactory();


            _clientBuilder.ApplyRequestHeaders(request);

            // if we haven't added an accept header, add a default
            if (!string.IsNullOrWhiteSpace(context.MediaType))
                request.Headers.Accept.AddDistinct(h => h.MediaType, context.MediaType);


            //if we haven't added a char-set, add a default
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
                using (var client = _clientBuilder.Build())
                {
                    var sendingContext = new SendingContext(context, request);

                    token.ThrowIfCancellationRequested();

                    await context.Handler.OnSending(sendingContext);

                    if (sendingContext.Result != null)
                    {
                        response = sendingContext.Result;
                    }
                    else
                    {
                        token.ThrowIfCancellationRequested();

                        response = await client.SendAsync(request, context.CompletionOption, token);
                    }

                    if (!context.IsSuccessfulResponse(response) && Settings.ExceptionFactory != null)
                    {
                        var ex = Settings.ExceptionFactory(response);

                        if (ex != null)
                            throw ex;
                    }

                    var sentContext = new SentContext(context, response);

                    token.ThrowIfCancellationRequested();

                    await context.Handler.OnSent(sentContext);

                    response = sentContext.Result;
                }
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            if (capturedException != null)
            {
                var exContext = new ExceptionContext(context, response, capturedException.SourceException);

                await context.Handler.OnException(exContext);

                if (!exContext.ExceptionHandled)
                    capturedException.Throw();
            }

            return response;
        }

        public void CancelRequest()
        {
            _tokenSource?.Cancel();
        }
    }
}