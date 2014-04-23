using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AonWeb.Fluent.Http.Handlers
{
    public interface IErrorHandler<TError>
    {
        IErrorHandler<TError> WithErrorHandler(Action<HttpErrorContext<TError>> handler);
        IErrorHandler< TError> WithNewErrorHandler(Action<HttpErrorContext<TError>> handler);
        Task<HttpErrorContext<TError>> HandleError(HttpResponseMessage response);

        IErrorHandler<TError> WithExceptionHandler(Action<HttpExceptionContext> handler);
        IErrorHandler<TError> WithNewExceptionHandler(Action<HttpExceptionContext> handler);
        HttpExceptionContext HandleException(Exception exception);
    }
}