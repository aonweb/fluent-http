using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Caching
{
    public interface IVaryByProvider
    {
        Task<IEnumerable<string>> Get(Uri uri);

        Task<bool> Put(Uri uri, IEnumerable<string> headers);

        Task<bool> Delete(Uri uri);
        Task DeleteAll();
    }
}