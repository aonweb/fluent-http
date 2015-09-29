using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public class TypedBuilderSettings : ITypedBuilderSettings
    {
        private readonly Modifiable<Type> _resultTypeValue;
        private readonly Modifiable<Type> _errorTypeValue;
        private readonly Modifiable<Type> _contentTypeValue;

        public TypedBuilderSettings()
            : this(
            Defaults.TypedBuilder.ResultType,
            Defaults.TypedBuilder.ContentType,
            Defaults.TypedBuilder.ErrorType) { }

        public TypedBuilderSettings(Type resultType, Type contentType, Type errorType)
        {
            Items = new Dictionary<string, object>();

            _resultTypeValue = new Modifiable<Type>(resultType);
            _contentTypeValue = new Modifiable<Type>(contentType);
            _errorTypeValue = new Modifiable<Type>(errorType);

            Handler = new TypedHandlerRegister();
            DeserializeResult = true;

            MediaType = Defaults.TypedBuilder.MediaType;
            MediaTypeFormatters = new MediaTypeFormatterCollection(Defaults.TypedBuilder.MediaTypeFormatters());
            SuccessfulResponseValidators = new List<Func<HttpResponseMessage, bool>>
            {
                Defaults.TypedBuilder.SuccessfulResponseValidator
            };
            ExceptionFactory = Defaults.TypedBuilder.ExceptionFactory;
            DefaultResultFactory = Defaults.TypedBuilder.DefaultResultFactory;
            SuppressCancellationErrors = Defaults.TypedBuilder.SuppressCancellationErrors;
            MediaType = Defaults.TypedBuilder.MediaType;
            SuppressTypeMismatchExceptions = Defaults.TypedBuilder.SuppressTypeMismatchExceptions;
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
        public bool DeserializeResult { get; set; }
        public bool SuppressTypeMismatchExceptions { get; set; }
        public bool SuppressCancellationErrors { get; set; }
        public CancellationToken Token { get; set; }

        public bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return SuccessfulResponseValidators.Any() && SuccessfulResponseValidators.All(v => v(response));
        }

        public IList<Func<HttpResponseMessage, bool>> SuccessfulResponseValidators { get; }
        public Func<ErrorContext, Exception> ExceptionFactory { get; set; }
        public TypedHandlerRegister Handler { get; }
        public MediaTypeFormatterCollection MediaTypeFormatters { get; }

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