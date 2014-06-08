using System;
using System.Collections;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;

namespace AonWeb.FluentHttp.Handlers
{
    public class TypedHttpCallContext : IHttpCallContext
    {
        private readonly TypedHttpCallBuilderSettings _settings;
        private readonly IRecursiveTypedHttpCallBuilder _builder;

        public TypedHttpCallContext(TypedHttpCallContext context)
            : this(context.Builder, context._settings) { }

        public TypedHttpCallContext(IRecursiveTypedHttpCallBuilder builder, TypedHttpCallBuilderSettings settings)
        {
            _builder = builder;
            _settings = settings;
        }

        public Func<object> ContentFactory { get { return _settings.ContentFactory; } }

        public TypedHttpCallHandlerRegister Handler { get { return _settings.Handler; } }

        public IDictionary Items { get { return _settings.Items; } }

        public IRecursiveTypedHttpCallBuilder Builder { get { return _builder; } }
        public string MediaType { get { return _settings.MediaType; } }
        public Encoding ContentEncoding { get { return _settings.ContentEncoding; } }
        public MediaTypeFormatterCollection MediaTypeFormatters { get { return _settings.MediaTypeFormatters; } }

        public bool DeserializeResult
        {
            get
            {
                return _settings.DeserializeResult;
            }
        }

        public CancellationTokenSource TokenSource
        {
            get
            {
                return _settings.TokenSource;
            }
        }

        public Type ResultType
        {
            get
            {
                return _settings.ResultType;
            }
        }

        public Type ErrorType
        {
            get
            {
                return _settings.ErrorType;
            }
        }

        public Type ContentType
        {
            get
            {
                return _settings.ContentType;
            }
        }

        public bool SuppressHandlerTypeExceptions
        {
            get
            {
                return _settings.SuppressHandlerTypeExceptions;
            }
        }

        public bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return !_settings.SuccessfulResponseValidators.Any() || _settings.SuccessfulResponseValidators.All(v => v(response));
        }
    }
}