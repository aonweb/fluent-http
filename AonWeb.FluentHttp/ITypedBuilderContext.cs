using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;

namespace AonWeb.FluentHttp
{
    public interface ITypedBuilderContext : IBuilderContext<IChildTypedBuilder, ITypedBuilderSettings>
    {
        IFormatter Formatter { get; }
        Func<object> ContentFactory { get; }
        Func<ITypedBuilderContext, object, Task<HttpContent>> HttpContentFactory { get; }
        Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, Task<object>> ResultFactory { get; }
        Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, ExceptionDispatchInfo, Task<object>> ErrorFactory { get; }
        Type ContentType { get; }
        Type ErrorType { get; }
        string MediaType { get; }
        Func<Type, object> DefaultResultFactory { get; }
        Func<Type, Exception, object> DefaultErrorFactory { get; }
        MediaTypeFormatterCollection MediaTypeFormatters { get; }
        TypedHandlerRegister HandlerRegister { get; }
        bool SuppressTypeMismatchExceptions { get; }
        Func<ExceptionCreationContext, Exception> ExceptionFactory { get; }
        bool DeserializeResult { get; }
        CancellationToken Token { get; }
    }
}