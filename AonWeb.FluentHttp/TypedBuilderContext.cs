using System;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using AonWeb.FluentHttp.Handlers;

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
        public Func<object> ContentFactory => _settings.ContentFactory;
        public Type ContentType => _settings.ContentType;
        public Type ErrorType => _settings.ErrorType;
        public string MediaType => _settings.MediaType;
        public Func<Type, object> DefaultResultFactory => _settings.DefaultResultFactory;
        public MediaTypeFormatterCollection MediaTypeFormatters => _settings.MediaTypeFormatters;
        public TypedHandlerRegister HandlerRegister => _settings.HandlerRegister;
        public bool SuppressTypeMismatchExceptions => _settings.SuppressTypeMismatchExceptions;
        public Func<ErrorContext, Exception> ExceptionFactory => _settings.ExceptionFactory;
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