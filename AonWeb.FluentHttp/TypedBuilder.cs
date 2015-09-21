using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp
{
    public class TypedBuilder : IChildTypedBuilder
    {
        private readonly IChildHttpBuilder _innerBuilder;
        private readonly IFormatter _formatter;
        private CancellationTokenSource _tokenSource;

        public TypedBuilder(ITypedBuilderSettings settings, IChildHttpBuilder builder, IFormatter formatter, IReadOnlyCollection<ITypedHandler> defaultHandlers)
        {
            _formatter = formatter;
            _innerBuilder = builder;

            Settings = settings;

            if (defaultHandlers != null && defaultHandlers.Any())
                foreach (var handler in defaultHandlers)
                    this.WithHandler(handler);
        }

        public ITypedBuilderSettings Settings { get; }

        public IAdvancedTypedBuilder Advanced => this;

        public ITypedBuilder WithConfiguration(Action<IHttpBuilderSettings> configuration)
        {
            _innerBuilder.WithConfiguration(configuration);

            return this;
        }
        void IConfigurable<IHttpBuilderSettings>.WithConfiguration(Action<IHttpBuilderSettings> configuration)
        {
            WithConfiguration(configuration);
        }

        public ITypedBuilder WithConfiguration(Action<ITypedBuilderSettings> configuration)
        {
            configuration?.Invoke(Settings);

            return this;
        }

        void IConfigurable<ITypedBuilderSettings>.WithConfiguration(Action<ITypedBuilderSettings> configuration)
        {
            WithConfiguration(configuration);
        }

        public IAdvancedTypedBuilder WithClientConfiguration(Action<IHttpClientBuilder> configuration)
        {
            _innerBuilder.WithClientConfiguration(configuration);

            return this;
        }

        public Task<TResult> ResultAsync<TResult>()
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

            return (TResult)result;
        }

        private async Task<object> ResultAsync(ITypedBuilderContext context, CancellationToken token)
        {
            HttpResponseMessage response = null;
            ExceptionDispatchInfo capturedException = null;
            try
            {
                //build content before creating request
                var hasContent = false;
                object content = null;

                if (context.ContentFactory != null)
                {
                    content = context.ContentFactory();

                    token.ThrowIfCancellationRequested();

                    var httpContent = await _formatter.CreateContent(content, context);

                    _innerBuilder.WithContent(() => httpContent);

                    hasContent = true;
                }

                using (var request = _innerBuilder.CreateRequest())
                {
                    token.ThrowIfCancellationRequested();

                    var sendingResult = await context.Handler.OnSending(context, request, content, hasContent);

                    if (sendingResult.IsDirty)
                        return sendingResult.Value;

                    token.ThrowIfCancellationRequested();

                    response = await _innerBuilder.ResultFromRequestAsync(request, token);
                }

                if (!context.IsSuccessfulResponse(response))
                {
                    object error;

                    if (typeof(IEmptyError).IsAssignableFrom(context.ErrorType))
                    {
                        error = TypeHelpers.GetDefaultValueForType(context.ErrorType);
                    }
                    else
                    {
                        token.ThrowIfCancellationRequested();

                        error = await _formatter.DeserializeError(response, context);
                    }

                    if (error != null && !context.ErrorType.IsInstanceOfType(error))
                    {
                        if (!context.SuppressHandlerTypeExceptions)
                            throw new TypeMismatchException(context.ErrorType, error.GetType(), response.DetailsForException());

                        return TypeHelpers.GetDefaultValueForType(context.ResultType);
                    }

                    var errorResult = await context.Handler.OnError(context, response, error);

                    var errorHandled = (bool)errorResult.Value;

                    if (!errorHandled && context.ExceptionFactory != null)
                    {
                        var ex = context.ExceptionFactory(new ErrorContext(context, response, error));

                        if (ex != null)
                            throw ex;
                    }

                }
                else
                {
                    var sentResult = await context.Handler.OnSent(context, response);

                    object result = null;

                    if (sentResult.IsDirty)
                    {
                        result = sentResult.Value;
                    }
                    else
                    {
                        if (context.DeserializeResult)
                        {
                            token.ThrowIfCancellationRequested();

                            result = await _formatter.DeserializeResult(response, context);
                        }
                    }

                    token.ThrowIfCancellationRequested();

                    var resultResult = await context.Handler.OnResult(context, response, result);

                    result = resultResult.Value;

                    if (result != null && !context.ResultType.IsInstanceOfType(result))
                    {
                        if (!context.SuppressHandlerTypeExceptions)
                            throw new TypeMismatchException(context.ResultType, result.GetType(), response.DetailsForException());

                        return TypeHelpers.GetDefaultValueForType(context.ResultType);
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }
            finally
            {
                ObjectHelpers.DisposeResponse(response);
            }

            if (capturedException != null)
            {
                var exceptionResult = await context.Handler.OnException(context, response, capturedException.SourceException);

                if (!(bool)exceptionResult.Value)
                {
                    if (!context.SuppressCancellationErrors || !(capturedException.SourceException is OperationCanceledException))
                        capturedException.Throw();
                }

            }

            var defaultResult = context.DefaultResultFactory(context.ResultType);

            if (defaultResult != null && !context.ResultType.IsInstanceOfType(defaultResult))
            {
                if (!context.SuppressHandlerTypeExceptions)
                    throw new TypeMismatchException(context.ResultType, defaultResult.GetType(), response.DetailsForException());

                return TypeHelpers.GetDefaultValueForType(context.ResultType);
            }

            return defaultResult;
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
