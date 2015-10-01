using System.Net.Http;
using AonWeb.FluentHttp.Handlers.Caching;

namespace AonWeb.FluentHttp.Caching
{
    public struct CacheResult
    {
        public CacheResult(object result, HttpRequestMessage request, HttpResponseMessage response, ICacheContext context)
            : this(result, new ResponseInfo(result, request, response, context)) { }

        public CacheResult(object result, ResponseInfo responseInfo)
        {
            Found = true;
            Result = result;
            ResponseInfo = responseInfo;
        }

        public bool Found { get; set; }
        public object Result { get; set; }
        public ResponseInfo ResponseInfo { get; private set; }

        public static CacheResult Empty = new CacheResult();

        public void UpdateResponseInfo(HttpRequestMessage request, HttpResponseMessage response, ICacheContext context)
        {
            if (ResponseInfo == null)
                ResponseInfo = new ResponseInfo(Result, request, response, context);
            else
                ResponseInfo.UpdateExpiration(Result, response, context);
        }
    }
}