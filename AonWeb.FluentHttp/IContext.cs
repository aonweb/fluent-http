using System;
using System.Collections;

namespace AonWeb.FluentHttp
{
    public interface IContext
    {
        IDictionary Items { get; }
        Type ResultType { get; }
    }
}