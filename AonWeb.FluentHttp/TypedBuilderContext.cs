using System;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp
{
    public class TypedBuilderContext : ITypedBuilderContext
    {
        private readonly ITypedBuilderSettings _settings;

        public TypedBuilderContext(ITypedBuilderContext context)
            : this(context.GetSettings()) { }

        public TypedBuilderContext(ITypedBuilderSettings settings)
        {
            _settings = settings;
        }
        
        public IDictionary Items => _settings.Items;
        public Type ResultType => _settings.ResultType;
        public IChildTypedBuilder Builder => _settings.Builder;
        public CancellationToken Token => _settings.Token;
        public bool SuppressCancellationErrors => _settings.SuppressCancellationErrors;
        public IFormatter Formatter => _settings.Formatter;
        public Func<object> ContentFactory => _settings.ContentFactory;
        public Func<ITypedBuilderContext, object, Task<HttpContent>> HttpContentFactory 
            => _settings.HttpContentFactory ?? ((ctx,ctn) => Task.FromResult<HttpContent>(null));
        public Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, Task<object>> ResultFactory 
            => _settings.ResultFactory ?? ((ctx, req, res) => Task.FromResult(TypeHelpers.GetDefaultValueForType(ctx.ResultType)));
        public Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, ExceptionDispatchInfo, Task<object>> ErrorFactory 
            => _settings.ErrorFactory ?? ((ctx, req, res, ex) => Task.FromResult(TypeHelpers.GetDefaultValueForType(ctx.ErrorType)));

        public Type ContentType => _settings.ContentType;
        public Type ErrorType => _settings.ErrorType;
        public string MediaType => _settings.MediaType;
        public Func<Type, object> DefaultResultFactory => _settings.DefaultResultFactory;
        public Func<Type, Exception, object> DefaultErrorFactory => _settings.DefaultErrorFactory;
        public MediaTypeFormatterCollection MediaTypeFormatters => _settings.MediaTypeFormatters;
        public TypedHandlerRegister HandlerRegister => _settings.HandlerRegister;
        public bool SuppressTypeMismatchExceptions => _settings.SuppressTypeMismatchExceptions;
        public Func<ExceptionCreationContext, Exception> ExceptionFactory => _settings.ExceptionFactory;
        public bool DeserializeResult => _settings.DeserializeResult;

        public bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return _settings.IsSuccessfulResponse(response);
        }

        public ITypedBuilderSettings GetSettings()
        {
            return _settings.GetSettings();
        }
    }
}