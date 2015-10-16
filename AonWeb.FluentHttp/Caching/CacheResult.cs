using System.Net.Http;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Caching
{
    public struct CacheResult
    {
        public CacheResult(object result, HttpRequestMessage request, HttpResponseMessage response, ICacheContext context)
            : this(result, CachingHelpers.CreateResponseMetadata(result, request, response, context)) { }

        public CacheResult(object result, IWritableResponseMetadata responseInfo)
        {
            Found = true;
            Result = result;
            ResponseInfo = responseInfo;
        }

        public bool Found { get; set; }
        public object Result { get; set; }
        public IWritableResponseMetadata ResponseInfo { get; private set; }

        public static CacheResult Empty = new CacheResult();

        public void UpdateResponseInfo(HttpRequestMessage request, HttpResponseMessage response, ICacheContext context)
        {
            ResponseInfo = CachingHelpers.CreateResponseMetadata(Result, request, response, context);
        }
    }
}