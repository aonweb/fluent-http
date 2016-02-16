using System;
using System.Collections;
using System.Threading;

namespace AonWeb.FluentHttp
{
    public interface IContext
    {
        IDictionary Items { get; }
        Type ResultType { get; }
        CancellationToken Token { get; }
    }
}