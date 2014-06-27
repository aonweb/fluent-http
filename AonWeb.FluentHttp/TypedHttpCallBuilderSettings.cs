using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public class TypedHttpCallBuilderSettings
    {
        private readonly IDictionary _items;

        private readonly ModifyTracker<Type> _resultTypeValue;
        private readonly ModifyTracker<Type> _errorTypeValue;
        private readonly ModifyTracker<Type> _contentTypeValue;

        public TypedHttpCallBuilderSettings()
            : this(
            HttpCallBuilderDefaults.DefaultResultType,
            HttpCallBuilderDefaults.DefaultContentType,
            HttpCallBuilderDefaults.DefaultErrorType) { }

        public TypedHttpCallBuilderSettings(Type resultType, Type contentType, Type errorType)
        {
            _items = new HybridDictionary();

            _resultTypeValue = new ModifyTracker<Type>(resultType);
            _contentTypeValue = new ModifyTracker<Type>(contentType);
            _errorTypeValue = new ModifyTracker<Type>(errorType);
            

            DeserializeResult = true;
            MediaType = HttpCallBuilderDefaults.DefaultMediaType;
            ContentEncoding = HttpCallBuilderDefaults.DefaultContentEncoding;
            MediaTypeFormatters = new MediaTypeFormatterCollection(HttpCallBuilderDefaults.DefaultMediaTypeFormatters);

            Handler = new TypedHttpCallHandlerRegister();

            SuccessfulResponseValidators = new List<Func<HttpResponseMessage, bool>>
            {
                HttpCallBuilderDefaults.DefaultSuccessfulResponseValidator
            };

            ExceptionFactory = HttpCallBuilderDefaults.DefaultExceptionFactory;
            DefaultResultFactory = HttpCallBuilderDefaults.DefaultResultFactory;
        }

        public Type ResultType
        {
            get { return _resultTypeValue.Value; }
            private set { _resultTypeValue.Value = value; }
        }

        public Type ErrorType
        {
            get { return _errorTypeValue.Value; }
            private set { _errorTypeValue.Value = value; }
        }

        public Type ContentType
        {
            get { return _contentTypeValue.Value; }
            private set { _contentTypeValue.Value = value; }
        }

        public CancellationTokenSource TokenSource { get; set; }
        public bool DeserializeResult { get; set; }
        public IDictionary Items { get { return _items; } }
        public Func<object> ContentFactory { get; set; }
        public string MediaType { get; set; }
        public Encoding ContentEncoding { get; set; }
        public MediaTypeFormatterCollection MediaTypeFormatters { get; set; }
        public TypedHttpCallHandlerRegister Handler { get; private set; }
        public IList<Func<HttpResponseMessage, bool>> SuccessfulResponseValidators { get; private set; }
        public Func<HttpCallErrorContext, Exception> ExceptionFactory { get; set; }
        public Func<Type, object> DefaultResultFactory { get; set; }

        public bool SuppressHandlerTypeExceptions { get; set; }

        public TypedHttpCallBuilderSettings SetResultType(Type type, bool authoritative = false)
        {
            return SetType(t => ResultType = t, type, _resultTypeValue.Modified, authoritative);

            return this;
        }

        public TypedHttpCallBuilderSettings SetContentType(Type type, bool authoritative = false)
        {
            return SetType(t => ContentType = t, type, _contentTypeValue.Modified, authoritative);
        }

        public TypedHttpCallBuilderSettings SetErrorType(Type type, bool authoritative = false)
        {
            return SetType(t => ErrorType = t, type, _errorTypeValue.Modified, authoritative);
        }

        private TypedHttpCallBuilderSettings SetType(Action<Type> configure, Type type, bool modified, bool authoritative = false)
        {
            if (authoritative || (!modified && type != typeof(object)))
                configure(type);

            return this;
        }

        public void Reset()
        {
            _items.Clear();
            DeserializeResult = true;
        }
    }
}