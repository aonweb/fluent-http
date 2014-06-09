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
        private ModifyTracker<Type> _errorTypeValue;
        private ModifyTracker<Type> _contentTypeValue;

        public TypedHttpCallBuilderSettings()
        {
            _items = new HybridDictionary();

            _resultTypeValue = new ModifyTracker<Type>(HttpCallBuilderDefaults.DefaultResultType);
            _errorTypeValue = new ModifyTracker<Type>(HttpCallBuilderDefaults.DefaultErrorType);
            _contentTypeValue = new ModifyTracker<Type>(HttpCallBuilderDefaults.DefaultContentType);

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

        public void SetResultType(Type type, bool authoritative = false)
        {
            SetType(t => ResultType = t, type, _resultTypeValue.Modified, authoritative);
        }

        public void SetContentType(Type type, bool authoritative = false)
        {
            SetType(t => ContentType = t, type, _contentTypeValue.Modified, authoritative);
        }

        public void SetErrorType(Type type, bool authoritative = false)
        {
            SetType(t => ErrorType = t, type, _errorTypeValue.Modified, authoritative);
        }

        private void SetType(Action<Type> configure, Type type, bool modified, bool authoritative = false)
        {
            if (authoritative || (!modified && type != typeof(object)))
                configure(type);
        }

        public void Reset()
        {
            _items.Clear();
            DeserializeResult = true;
        }
    }
}