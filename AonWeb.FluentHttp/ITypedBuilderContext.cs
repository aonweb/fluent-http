using System;
using System.Net.Http.Formatting;
using System.Threading;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public interface ITypedBuilderContext : IBuilderContext<IChildTypedBuilder, ITypedBuilderSettings>
    {
        Func<object> ContentFactory { get; }
        Type ContentType { get; }
        Type ErrorType { get; }
        string MediaType { get; }
        Func<Type, object> DefaultResultFactory { get; }
        MediaTypeFormatterCollection MediaTypeFormatters { get; }
        TypedHandlerRegister Handler { get; }
        bool SuppressHandlerTypeExceptions { get; }
        Func<ErrorContext, Exception> ExceptionFactory { get; }
        bool DeserializeResult { get; }
        CancellationToken Token { get; }
    }
}