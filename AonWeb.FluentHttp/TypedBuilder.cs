using System;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Exceptions.Helpers;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp
{
    public class TypedBuilder : IChildTypedBuilder
    {
        private readonly IChildHttpBuilder _innerBuilder;
        private CancellationTokenSource _tokenSource;

        public TypedBuilder(ITypedBuilderSettings settings, IChildHttpBuilder builder)
        {
            _innerBuilder = builder;

            Settings = settings;
            Settings.Builder = this;

            _innerBuilder.WithCaching(false);
            _innerBuilder.WithExceptionFactory(null);
        }

        public ITypedBuilderSettings Settings { get; private set; }

        public IAdvancedTypedBuilder Advanced => this;

        public virtual void WithSettings(ITypedBuilderSettings settings)
        {
            Settings = settings;
        }

        public ITypedBuilder WithConfiguration(Action<IHttpBuilderSettings> configuration)
        {
            _innerBuilder.WithConfiguration(configuration);

            return this;
        }
        public IAdvancedTypedBuilder WithConfiguration(Action<IAdvancedHttpBuilder> configuration)
        {
            configuration?.Invoke(_innerBuilder);

            return this;
        }

        public ITypedBuilder WithConfiguration(Action<ITypedBuilderSettings> configuration)
        {
            configuration?.Invoke(Settings);

            return this;
        }

        public IAdvancedTypedBuilder WithConfiguration(Action<ICacheSettings> configuration)
        {
            this.WithOptionalHandlerConfiguration<TypedCacheConfigurationHandler>(
                h => h.WithConfiguration(configuration));

            return this;
        }
        public IAdvancedTypedBuilder WithClientConfiguration(Action<IHttpClientBuilder> configuration)
        {
            _innerBuilder.WithClientConfiguration(configuration);

            return this;
        }
        void IConfigurable<IHttpBuilderSettings>.WithConfiguration(Action<IHttpBuilderSettings> configuration)
        {
            WithConfiguration(configuration);
        }

        void IConfigurable<IAdvancedHttpBuilder>.WithConfiguration(Action<IAdvancedHttpBuilder> configuration)
        {
            configuration?.Invoke(_innerBuilder);
        }

        void IConfigurable<ITypedBuilderSettings>.WithConfiguration(Action<ITypedBuilderSettings> configuration)
        {
            WithConfiguration(configuration);
        }

        void IConfigurable<ICacheSettings>.WithConfiguration(Action<ICacheSettings> configuration)
        {
            WithConfiguration(configuration);
        }

        public virtual Task<TResult> ResultAsync<TResult>()
        {
            return ResultAsync<TResult>(CancellationToken.None);
        }

        public virtual async Task<TResult> ResultAsync<TResult>(CancellationToken token)
        {
            if (typeof(IEmptyResult).IsAssignableFrom(typeof(TResult)))
                ConfigureResponseDeserialization(false);

            TResult result;
            using (_tokenSource = ObjectHelpers.GetCancellationTokenSource(token))
            {
                Settings.Token = _tokenSource.Token;

                result = await RecursiveResultAsync<TResult>(Settings.Token);
            }

            Reset();

            return result;
        }

        public Task SendAsync()
        {
            return SendAsync(CancellationToken.None);
        }

        public async Task SendAsync(CancellationToken token)
        {
            //if (!typeof(IEmptyResult).IsAssignableFrom(_settings.ResultType))
            //    _settings.SetResultType(typeof(EmptyResult), true);

            ConfigureResponseDeserialization(false);

            using (_tokenSource = ObjectHelpers.GetCancellationTokenSource(token))
            {
                Settings.Token = _tokenSource.Token;

                await ResultAsync(new TypedBuilderContext(Settings), Settings.Token).ConfigureAwait(false);
            }
        }

        public void CancelRequest()
        {
            _tokenSource?.Cancel();
        }

        public async Task<TResult> RecursiveResultAsync<TResult>(CancellationToken token)
        {
            Settings.WithDefiniteResultType(typeof(TResult));
            var context = new TypedBuilderContext(Settings);

            var result = await ResultAsync(context, token).ConfigureAwait(false);

            return TypeHelpers.CheckType<TResult>(result, context.SuppressTypeMismatchExceptions);
        }

        private async Task<object> ResultAsync(ITypedBuilderContext context, CancellationToken token)
        {
            HttpRequestMessage request = null;
            HttpResponseMessage response = null;
            ExceptionDispatchInfo capturedException = null;
            object result = null;
            try
            {
                try
                {
                    //build content before creating request
                    //var hasContent = false;
                    object content = null;

                    if (context.ContentFactory != null)
                    {
                        content = context.ContentFactory();

                        if (content != null)
                        {
                            _innerBuilder.WithContent(async ctx =>
                            {
                                var httpContent = await context.HttpContentFactory(context, content);

                                return httpContent;
                            });
                        }
                    }

                    token.ThrowIfCancellationRequested();

                    request = await _innerBuilder.CreateRequest();

                    token.ThrowIfCancellationRequested();

                    var sendingResult = await context.HandlerRegister.OnSending(context, request, content);

                    if (sendingResult.IsDirty)
                        return sendingResult.Value;

                    token.ThrowIfCancellationRequested();

                    response = await GetResponse(context, request, token);

                    if (!context.IsSuccessfulResponse(response))
                        throw ObjectHelpers.CreateHttpException(response, request);

                    var sentResult = await context.HandlerRegister.OnSent(context, request, response);

                    if (sentResult.IsDirty)
                    {
                        result = sentResult.Value;
                    }
                    else
                    {
                        if (context.DeserializeResult)
                        {
                            token.ThrowIfCancellationRequested();

                            result = await context.ResultFactory(context, request, response);
                        }
                    }

                    token.ThrowIfCancellationRequested();

                    var resultResult = await context.HandlerRegister.OnResult(context, request, response, result);

                    result = resultResult.Value;

                    if (result != null)
                    {
                        TypeHelpers.ValidateType(result, context.ResultType, context.SuppressTypeMismatchExceptions, () => response.GetExceptionMessage(request));

                        return result;
                    }
                }
                catch (Exception ex)
                {
                    capturedException = ExceptionDispatchInfo.Capture(ex);
                }

                if (capturedException != null)
                {
                    var error = await context.ErrorFactory(context, request, response, capturedException) ?? TypeHelpers.GetDefaultValueForType(context.ErrorType);

                    TypeHelpers.ValidateType(error, context.ErrorType, context.SuppressTypeMismatchExceptions, () => response.GetExceptionMessage(request));

                    var errorResult = await context.HandlerRegister.OnError(context, request, response, error);

                    var errorHandled = (bool)errorResult.Value;

                    if (!errorHandled)
                    {
                        var ex = context.ExceptionFactory?.Invoke(new ExceptionCreationContext(context, request, response, error, capturedException.SourceException));

                        if (ex != null)
                            throw ex;
                    }
                    else
                    {
                        capturedException = null;
                    }
                }
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }
            finally
            {
                if (result != response) // if the result is of type http response message and equal to the response don't dispose.
                {
                    ObjectHelpers.Dispose(request);
                    ObjectHelpers.Dispose(response);
                }
            }

            if (capturedException != null)
            {
                var exceptionResult = await context.HandlerRegister.OnException(context, request, response, capturedException.SourceException);

                if (!(bool)exceptionResult.Value)
                {
                    if (!context.SuppressCancellationErrors || !(capturedException.SourceException is OperationCanceledException))
                        capturedException.Throw();
                }

            }

            var defaultResult = context.DefaultResultFactory?.Invoke(context.ResultType);

            TypeHelpers.ValidateType(defaultResult, context.ResultType, context.SuppressTypeMismatchExceptions,
                        () => response.GetExceptionMessage(request));

            return defaultResult;
        }

        protected virtual Task<HttpResponseMessage> GetResponse(ITypedBuilderContext context, HttpRequestMessage request, CancellationToken token)
        {
            return _innerBuilder.ResultFromRequestAsync(request, token);
        }

        private void ConfigureResponseDeserialization(bool willDeserialize)
        {
            Settings.DeserializeResult = willDeserialize;
            _innerBuilder.WithOptionalHandlerConfiguration<FollowLocationHandler>(h => h.Enabled = willDeserialize);
        }

        private void Reset()
        {
            ConfigureResponseDeserialization(true);

            Settings.Reset();
        }
    }
}
