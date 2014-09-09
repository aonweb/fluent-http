using System.Net.Http;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheResult
    {
        private CacheResult() {  }

        public CacheResult(object result, HttpResponseMessage response, CacheContext context)
            : this(result, new ResponseInfo(result, response, context)) { }

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

        public void UpdateResponseInfo(HttpResponseMessage response, CacheContext context)
        {
            if (ResponseInfo == null)
                ResponseInfo = new ResponseInfo(Result, response, context);
            else
                ResponseInfo.UpdateExpiration(Result, response, context.DefaultExpiration);
        }
    }
}