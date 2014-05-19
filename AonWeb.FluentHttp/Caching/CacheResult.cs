using System.Net.Http;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheResult<TResult>
    {
        public CacheResult() {  }

        public CacheResult(ResponseInfo responseInfo)
        {
            Found = true;
            ResponseInfo = responseInfo;
        }

        public CacheResult(TResult result, HttpResponseMessage response, CacheContext<TResult> context)
        {
            Found = true;
            Result = result;
            ResponseInfo = CreateResponseInfo(result, response, context);
        }

        public bool Found { get; set; }
        public TResult Result { get; set; }
        public ResponseInfo ResponseInfo { get; private set; }
        public static CacheResult<TResult> Empty = new CacheResult<TResult>();

        public void UpdateResponseInfo(HttpResponseMessage response, CacheContext<TResult> context)
        {
            if (ResponseInfo == null)
                ResponseInfo = CreateResponseInfo(Result, response, context);
            else
                ResponseInfo.UpdateExpiration(Result, response, context.DefaultExpiration);
        }

        private ResponseInfo CreateResponseInfo(TResult result, HttpResponseMessage response, CacheContext<TResult> context)
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