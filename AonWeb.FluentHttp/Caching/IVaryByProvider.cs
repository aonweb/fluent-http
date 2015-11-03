using System;
using System.Collections.Generic;

namespace AonWeb.FluentHttp.Caching
{
    public interface IVaryByProvider
    {
        IEnumerable<string> Get(Uri uri);

        bool Put(Uri uri, IEnumerable<string> headers);

        void Clear();

        bool Remove(Uri uri);
    }
}