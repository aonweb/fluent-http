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

        public CacheResult(TResult result, HttpResponseMessage response, CacheSettings<TResult> settings)
        {
            Found = true;
            Result = result;
            ResponseInfo = CreateResponseInfo(result,  response, settings);
        }

        public bool Found { get; set; }
        public TResult Result { get; set; }
        public ResponseInfo ResponseInfo { get; private set; }
        public static CacheResult<TResult> Empty = new CacheResult<TResult>();

        public void UpdateResponseInfo(HttpResponseMessage response, CacheSettings<TResult> settings)
        {
            if (ResponseInfo == null)
                ResponseInfo = CreateResponseInfo(Result, response, settings);
            else
                ResponseInfo.UpdateExpiration(Result, response, settings.DefaultExpiration);
        }

        private ResponseInfo CreateResponseInfo(TResult result, HttpResponseMessage response, CacheSettings<TResult> settings)
        {
            return new ResponseInfo(result,
                response,
                settings.DefaultExpiration,
                settings.DefaultVaryByHeaders,
                settings.MustRevalidateByDefault,
                settings.DependentUris);
        }
    }
}