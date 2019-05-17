using System;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp
{
    public class TypedBuilderContext : ITypedBuilderContext
    {
        public TypedBuilderContext(ITypedBuilderSettings settings)
            :this((ITypedBuilderContext)settings)
        {
            CacheMetadata = new CacheMetadata(settings.CacheSettings);
        }

        public TypedBuilderContext(ITypedBuilderContext context)
        {
            Items = context.Items;
            ResultType = context.ResultType;
            Builder = context.Builder;
            SuppressCancellationErrors = context.SuppressCancellationErrors;
            Formatter = context.Formatter;
            ContentFactory = context.ContentFactory;
            HttpContentFactory = context.HttpContentFactory;
            ResultFactory = context.ResultFactory;
            ErrorFactory = context.ErrorFactory;
            ContentType = context.ContentType;
            ErrorType = context.ErrorType;
            MediaType = context.MediaType;
            DefaultResultFactory = context.DefaultResultFactory;
            DefaultErrorFactory = context.DefaultErrorFactory;
            HandlerRegister = context.HandlerRegister;
            SuppressTypeMismatchExceptions = context.SuppressTypeMismatchExceptions;
            ExceptionFactory = context.ExceptionFactory;
            DeserializeResult = context.DeserializeResult;
            Token = context.Token;
            CacheMetadata = context.CacheMetadata;
            ResponseValidator = context.ResponseValidator;
        }

        public IDictionary Items { get; }
        public Type ResultType { get; }
        public ITypedBuilder Builder { get; }
        public bool SuppressCancellationErrors { get; }
        public IFormatter Formatter { get; }
        public Func<object> ContentFactory { get; }
        public Func<ITypedBuilderContext, object, Task<HttpContent>> HttpContentFactory { get; }
        public Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, Task<object>> ResultFactory { get; }
        public Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, Exception, Task<object>> ErrorFactory { get; }
        public Type ContentType { get; }
        public Type ErrorType { get; }
        public string MediaType { get; }
        public Func<Type, object> DefaultResultFactory { get; }
        public Func<Type, Exception, object> DefaultErrorFactory { get; }
        public MediaTypeFormatterCollection MediaTypeFormatters { get; }
        public TypedHandlerRegister HandlerRegister { get; }
        public bool SuppressTypeMismatchExceptions { get; }
        public Func<ExceptionCreationContext, Exception> ExceptionFactory { get; }
        public bool DeserializeResult { get; }
        public CancellationToken Token { get; }
        public ICacheMetadata CacheMetadata { get; }
        public ResponseValidatorCollection ResponseValidator { get; }

        public bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return ResponseValidator.IsValid(response);
        }
    }
}