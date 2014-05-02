using System;
using System.Collections.Generic;

namespace AonWeb.FluentHttp.Caching
{
    public interface ICachedHttpResult
    {
        TimeSpan? Duration { get; }

        IEnumerable<string> AssociatedUris { get; }
    }
}