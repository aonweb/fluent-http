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

        public TypedHttpCallBuilderSettings()
        {
            _items = new HybridDictionary();

            ResultType = HttpCallBuilderDefaults.DefaultResultType;
            ErrorType = HttpCallBuilderDefaults.DefaultErrorType;
            ContentType = HttpCallBuilderDefaults.DefaultContentType;

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

        public Type ResultType { get; set; }
        public Type ErrorType { get; set; }
        public Type ContentType { get; set; }
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

        public void Reset()
        {
            _items.Clear();
            DeserializeResult = true;
        }
    }
}