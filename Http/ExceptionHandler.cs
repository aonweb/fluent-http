using System;
using System.Net;
using System.Net.Http;
using AonWeb.Fluent.Http.Exceptions;

namespace AonWeb.Fluent.Http
{
    public class ErrorHandlerSettings<TError>
    {
        public ErrorHandlerSettings()
        {
        }

        public Action<HttpErrorContext<TError>> ErrorHandler { get; set; }
    }

    public class HttpErrorContext<TError>
    {
        public HttpStatusCode StatusCode { get; set; }
        public TError Error { get; set; }
        public bool ErrorHandled { get; set; }
    }

    public interface IErrorHandler<TError>
    {
        IErrorHandler<TError> WithErrorHandler(Action<HttpErrorContext<TError>> handler);
        IErrorHandler<TError> WithNewErrorHandler(Action<HttpErrorContext<TError>> handler);
        HttpErrorContext<TError> HandleError(HttpCallBuilder builder, HttpResponseMessage response);
    }

    public class ErrorHandler<TError> : IErrorHandler<TError>
    {

        private readonly ErrorHandlerSettings<TError> _settings;
        private readonly ISerializerFactory _serializerFactory;

        public ErrorHandler()
            : this(new ErrorHandlerSettings<TError>(), new SerializerFactory()) { }

        internal ErrorHandler(ErrorHandlerSettings<TError> settings, ISerializerFactory serializerFactory)
        {
            _settings = settings;
            _serializerFactory = serializerFactory;
        }

        public IErrorHandler<TError> WithErrorHandler(Action<HttpErrorContext<TError>> handler)
        {
            return WithNewErrorHandler(Utils.MergeAction(_settings.ErrorHandler, handler));
        }

        public IErrorHandler<TError> WithNewErrorHandler(Action<HttpErrorContext<TError>> handler)
        {
            _settings.ErrorHandler = handler;

            return this;
        }

        public HttpErrorContext<TError> HandleError(HttpCallBuilder builder, HttpResponseMessage response)
        {

            //TODO: revaluate

                //validate response

                //if validation fails, create error object - need serialization factory
            var serializer = _serializerFactory.GetSerializer(response.Content.Headers.ContentType.MediaType);
            var error = serializer.Deserialize<TError>(response.Content);

            var ctx = new HttpErrorContext<TError>
            {
                StatusCode = response.StatusCode,
                Error = error
            };

            if (_settings.ErrorHandler != null)
                _settings.ErrorHandler(ctx);

            if (!ctx.ErrorHandled)
                throw new HttpErrorException<TError>(ctx.Error, ctx.StatusCode);

            return ctx;
        } 

        //TODO: handle exceptions too.
    }
}