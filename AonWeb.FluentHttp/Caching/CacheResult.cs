using System.Net.Http;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheResult
    {
        public CacheResult() {  }

        public CacheResult(ResponseInfo responseInfo)
        {
            Found = true;
            ResponseInfo = responseInfo;
        }

        public CacheResult(object result, HttpResponseMessage response, CacheContext context)
        {
            Found = true;
            Result = result;
            ResponseInfo = CreateResponseInfo(result, response, context);
        }

        public bool Found { get; set; }
        public object Result { get; set; }
        public ResponseInfo ResponseInfo { get; private set; }
        public static CacheResult Empty = new CacheResult();

        public void UpdateResponseInfo(HttpResponseMessage response, CacheContext context)
        {
            if (ResponseInfo == null)
                ResponseInfo = CreateResponseInfo(Result, response, context);
            else
                ResponseInfo.UpdateExpiration(Result, response, context.DefaultExpiration);
        }

        private ResponseInfo CreateResponseInfo(object result, HttpResponseMessage response, CacheContext context)
        {
            return new ResponseInfo(result,
                response,
                context.DefaultExpiration,
                context.DefaultVaryByHeaders,
                context.MustRevalidateByDefault,
                context.DependentUris);
        }
    }
}