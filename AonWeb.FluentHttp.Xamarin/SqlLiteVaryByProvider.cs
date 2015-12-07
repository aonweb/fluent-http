using System;
using System.Collections.Generic;
using AonWeb.FluentHttp.Caching;

namespace AonWeb.FluentHttp.Xamarin
{
    public class SqlLiteVaryByProvider: IVaryByProvider
    {
        public IEnumerable<string> Get(Uri uri)
        {
            throw new NotImplementedException();
        }

        public bool Put(Uri uri, IEnumerable<string> headers)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Remove(Uri uri)
        {
            throw new NotImplementedException();
        }
    }
}