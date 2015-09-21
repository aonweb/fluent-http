using System;
using System.Collections.Generic;

namespace AonWeb.FluentHttp.Caching
{
    public interface IVaryByStore
    {
        IEnumerable<string> Get(Uri uri);

        void AddOrUpdate(Uri uri, IEnumerable<string> headers);

        void Clear();

        bool TryRemove(Uri uri);
    }
}