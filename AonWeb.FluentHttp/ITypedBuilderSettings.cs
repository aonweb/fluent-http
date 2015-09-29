using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public interface ITypedBuilderSettings : ITypedBuilderContext
    {
        new Func<object> ContentFactory { get; set; }
        ITypedBuilderSettings WithContentType(Type type);
        ITypedBuilderSettings WithDefiniteContentType(Type type);
        ITypedBuilderSettings WithResultType(Type type);
        ITypedBuilderSettings WithDefiniteResultType(Type type);
        ITypedBuilderSettings WithErrorType(Type type);
        ITypedBuilderSettings WithDefiniteErrorType(Type type);
        new string MediaType { get; set; }
        new Func<Type, object> DefaultResultFactory { get; set; }
        new bool DeserializeResult { get; set; }
        new bool SuppressTypeMismatchExceptions { get; set; }
        new bool SuppressCancellationErrors { get; set; }
        IList<Func<HttpResponseMessage, bool>> SuccessfulResponseValidators { get; }
        new Func<ErrorContext, Exception> ExceptionFactory { get; set; }
        new CancellationToken Token { get; set; }
        void Reset();
    }
}