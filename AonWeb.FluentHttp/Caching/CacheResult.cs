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

        public CacheResult(object result, IResponseMetadata responseMetadata)
        {
            Found = true;
            Result = result;
            ResponseMetadata = responseMetadata;
        }

        public bool Found { get; set; }
        public object Result { get; set; }
        public IResponseMetadata ResponseMetadata { get; private set; }

        public static CacheResult Empty = new CacheResult();

        public void UpdateResponseInfo(HttpRequestMessage request, HttpResponseMessage response, ICacheContext context)
        {
            ResponseMetadata = CachingHelpers.CreateResponseMetadata(Result, request, response, context);
        }
    }
}