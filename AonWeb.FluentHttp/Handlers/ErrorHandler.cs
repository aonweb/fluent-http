using System;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.Fluent.Http.Exceptions;
using AonWeb.Fluent.Http.Serialization;

namespace AonWeb.Fluent.Http.Handlers
{
    public class ErrorHandler<TError> : IErrorHandler<TError>
    {
        private readonly ErrorHandlerSettings<TError> _settings;
        private readonly ISerializerFactory<TError> _serializerFactory;

        public ErrorHandler()
            : this(new ErrorHandlerSettings<TError>(), new SerializerFactory<TError>()) { }

        internal ErrorHandler(ErrorHandlerSettings<TError> settings, ISerializerFactory<TError> serializerFactory)
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

        public IErrorHandler<TError> WithExceptionHandler(Action<HttpExceptionContext> handler)
        {
            return WithNewExceptionHandler(Utils.MergeAction(_settings.ExceptionHandler, handler));
        }
        public IErrorHandler<TError> WithNewExceptionHandler(Action<HttpExceptionContext> handler)
        {
            _settings.ExceptionHandler = handler;

            return this;
        }

        public HttpExceptionContext HandleException(Exception exception)
        {
            var ctx = new HttpExceptionContext
            {
                Exception = exception
            };

            if (_settings.ExceptionHandler != null)
                _settings.ExceptionHandler(ctx);

            return ctx;
        }

        public async Task<HttpErrorContext<TError>> HandleError(HttpResponseMessage response)
        {
            if (IsValidResponse(response))
                return null;

            var serializer = _serializerFactory.GetSerializer(response);
            var error = await serializer.Deserialize(response.Content);

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

        private bool IsValidResponse(HttpResponseMessage response)
        {
            return _settings.ValidStatusCodes.Contains(response.StatusCode);
        }
    }
}