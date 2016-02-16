using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Settings
{
    public class TypedBuilderSettings : ITypedBuilderSettings
    {
        private readonly Modifiable<Type> _resultTypeValue;
        private readonly Modifiable<Type> _errorTypeValue;
        private readonly Modifiable<Type> _contentTypeValue;

        public TypedBuilderSettings(
            IFormatter formatter,
            ICacheSettings cacheSettings,
            IEnumerable<ITypedHandler> handlers,
            IEnumerable<ITypedResponseValidator> responseValidators)
            : this()
        {
            Formatter = formatter;
            ResponseValidator = new ResponseValidatorCollection(responseValidators);
            CacheSettings = cacheSettings;

            if (handlers != null)
            {
                foreach (var handler in handlers)
                    HandlerRegister.WithHandler(handler);
            }
        }

        protected TypedBuilderSettings()
        { 
            Items = new Dictionary<string, object>();

            _resultTypeValue = new Modifiable<Type>(typeof(string));
            _contentTypeValue = new Modifiable<Type>(typeof(string));
            _errorTypeValue = new Modifiable<Type>(typeof(EmptyError));

            HandlerRegister = new TypedHandlerRegister();
            DeserializeResult = true;
            
            SuppressCancellationErrors = false;
            SuppressTypeMismatchExceptions = false;
            MediaType = "application/json";
            MediaTypeFormatters = new MediaTypeFormatterCollection().FluentAdd(new StringMediaFormatter());
            ResultType = typeof(string);
            ErrorType = typeof(string);
            ContentType = typeof(EmptyRequest);
            DefaultResultFactory = TypeHelpers.GetDefaultValueForType;
            DefaultErrorFactory = (type, exception) => TypeHelpers.GetDefaultValueForType(type);
            ExceptionFactory = ObjectHelpers.CreateTypedException;
            
            HttpContentFactory = ObjectHelpers.CreateHttpContent;
            ResultFactory = ObjectHelpers.CreateResult;
            ErrorFactory = ObjectHelpers.CreateError;
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
        public ICacheMetadata CacheMetadata => CacheSettings;
        public ResponseValidatorCollection ResponseValidator { get; }

        public bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return ResponseValidator.IsValid(response);
        }
        
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

        public void SetBuilder(IChildTypedBuilder builder)
        {
            Builder = builder;
        }

        public ITypedBuilderSettings GetSettings()
        {
            return this;
        }
        
        public ITypedBuilder Builder { get; set; }
    }
}