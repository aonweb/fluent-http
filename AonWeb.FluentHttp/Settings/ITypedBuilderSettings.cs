using System;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Settings
{
    public interface ITypedBuilderSettings : ITypedBuilderContext
    {
        new Func<object> ContentFactory { get; set; }
        new Func<ITypedBuilderContext, object, Task<HttpContent>> HttpContentFactory { get; set; }
        new Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, Task<object>> ResultFactory { get; set; }
        new Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, Exception, Task<object>> ErrorFactory { get; set; }
        ITypedBuilderSettings WithContentType(Type type);
        ITypedBuilderSettings WithDefiniteContentType(Type type);
        ITypedBuilderSettings WithResultType(Type type);
        ITypedBuilderSettings WithDefiniteResultType(Type type);
        ITypedBuilderSettings WithErrorType(Type type);
        ITypedBuilderSettings WithDefiniteErrorType(Type type);
        new string MediaType { get; set; }
        new Func<Type, object> DefaultResultFactory { get; set; }
        new Func<Type, Exception, object> DefaultErrorFactory { get; set; }
        new bool DeserializeResult { get; set; }
        new bool SuppressTypeMismatchExceptions { get; set; }
        new bool SuppressCancellationErrors { get; set; }
        new Func<ExceptionCreationContext, Exception> ExceptionFactory { get; set; }
        new CancellationToken Token { get; set; }
        ICacheSettings CacheSettings { get; }
        void Reset();
        new ITypedBuilder Builder { get; set; }
    }
}