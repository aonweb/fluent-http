using System;
using System.Collections.Generic;

namespace AonWeb.FluentHttp.Cache
{
    public interface ICachedHttpResult
    {
        TimeSpan? Duration { get; }

        IEnumerable<string> AssociatedUris { get; }
    }
}