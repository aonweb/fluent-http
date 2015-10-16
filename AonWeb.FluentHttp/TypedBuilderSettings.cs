using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;

namespace AonWeb.FluentHttp
{
    public class TypedBuilderSettings : ITypedBuilderSettings
    {
        private readonly Modifiable<Type> _resultTypeValue;
        private readonly Modifiable<Type> _errorTypeValue;
        private readonly Modifiable<Type> _contentTypeValue;

        public TypedBuilderSettings(IFormatter formatter)
            : this(formatter,
            Defaults.Current.GetTypedBuilderDefaults().ResultType,
            Defaults.Current.GetTypedBuilderDefaults().ContentType,
            Defaults.Current.GetTypedBuilderDefaults().ErrorType) { }

        public TypedBuilderSettings(IFormatter formatter, Type resultType, Type contentType, Type errorType)
        {
            Items = new Dictionary<string, object>();

            Formatter = formatter;
            _resultTypeValue = new Modifiable<Type>(resultType);
            _contentTypeValue = new Modifiable<Type>(contentType);
            _errorTypeValue = new Modifiable<Type>(errorType);

            HandlerRegister = new TypedHandlerRegister();
            DeserializeResult = true;

            var defaults = Defaults.Current.GetTypedBuilderDefaults();

            MediaType = defaults.MediaType;
            MediaTypeFormatters = new MediaTypeFormatterCollection(defaults.MediaTypeFormatters());
            SuccessfulResponseValidators = new List<Func<HttpResponseMessage, bool>>
            {
                defaults.SuccessfulResponseValidator
            };
            ExceptionFactory = defaults.ExceptionFactory;
            DefaultResultFactory = defaults.DefaultResultFactory;
            DefaultErrorFactory = defaults.DefaultErrorFactory;
            SuppressCancellationErrors = defaults.SuppressCancellationErrors;
            MediaType = defaults.MediaType;
            SuppressTypeMismatchExceptions = defaults.SuppressTypeMismatchExceptions;
            CacheSettings = new CacheSettings();
            HttpContentFactory = defaults.HttpContentFactory;
            ResultFactory = defaults.ResultFactory;
            ErrorFactory = defaults.ErrorFactory;
        }

        public IDictionary Items { get; }

        public Type ResultType
        {
            get { return _resultTypeValue.Value; }
            private set { _resultTypeValue.Value = value; }
        }
     
        public Type ContentType
        {
            get { return _contentTypeValue.Value; }
            private set { _contentTypeValue.Value = value; }
        }

        public Type ErrorType
        {
            get { return _errorTypeValue.Value; }
            private set { _errorTypeValue.Value = value; }
        }

        public Func<object> ContentFactory { get; set; }

        public string MediaType { get; set; }
        public Func<Type, object> DefaultResultFactory { get; set; }
        public Func<Type, Exception, object> DefaultErrorFactory { get; set; }
        public bool DeserializeResult { get; set; }
        public bool SuppressTypeMismatchExceptions { get; set; }
        public bool SuppressCancellationErrors { get; set; }
        public CancellationToken Token { get; set; }

        public bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return SuccessfulResponseValidators.Any() && SuccessfulResponseValidators.All(v => v(response));
        }

        public IList<Func<HttpResponseMessage, bool>> SuccessfulResponseValidators { get; }
        public Func<ExceptionCreationContext, Exception> ExceptionFactory { get; set; }
        public TypedHandlerRegister HandlerRegister { get; }
        public MediaTypeFormatterCollection MediaTypeFormatters { get; }
        public IFormatter Formatter { get; }

        public Func<ITypedBuilderContext, object, Task<HttpContent>> HttpContentFactory { get; set; }
        public Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, Task<object>> ResultFactory { get; set; }

        public ICacheSettings CacheSettings { get; }

        public virtual ITypedBuilderSettings WithContentType(Type type)
        {
            return SetType(t => ContentType = t, type, _contentTypeValue.IsDirty);
        }

        public virtual ITypedBuilderSettings WithDefiniteContentType(Type type)
        {
            return SetType(t => ContentType = t, type, _contentTypeValue.IsDirty, true);
        }

        public virtual ITypedBuilderSettings WithResultType(Type type)
        {
            return SetType(t => ResultType = t, type, _resultTypeValue.IsDirty);
        }

        public virtual ITypedBuilderSettings WithDefiniteResultType(Type type)
        {
            return SetType(t => ResultType = t, type, _resultTypeValue.IsDirty, true);
        }

        public virtual ITypedBuilderSettings WithErrorType(Type type)
        {
            return SetType(t => ErrorType = t, type, _errorTypeValue.IsDirty);
        }

        public virtual ITypedBuilderSettings WithDefiniteErrorType(Type type)
        {
            return SetType(t => ErrorType = t, type, _errorTypeValue.IsDirty, true);
        }

        private ITypedBuilderSettings SetType(Action<Type> configure, Type type, bool modified, bool authoritative = false)
        {
            if (authoritative || (!modified && type != typeof(object)))
                configure(type);

            return this;
        }

        public Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, ExceptionDispatchInfo, Task<object>> ErrorFactory { get; set; }

        public void Reset()
        {
            Items.Clear();
            DeserializeResult = true;
        }

        public ITypedBuilderSettings GetSettings()
        {
            return this;
        }
        
        public virtual IChildTypedBuilder Builder { get; protected internal set; }
    }
}