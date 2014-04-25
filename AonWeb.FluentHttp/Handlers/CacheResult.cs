using System;
using System.Collections.Generic;

namespace AonWeb.Fluent.Http.Cache
{
    public interface ICachedHttpResult
    {
        TimeSpan? Duration { get; }

        IEnumerable<string> AssociatedUris { get; }
    }
}