namespace AonWeb.FluentHttp.Caching
{
    public class CachedItem
    {
        public CachedItem(ResponseInfo responseInfo)
        {
            ResponseInfo = responseInfo;
        }

        public ResponseInfo ResponseInfo { get; private set; }
        public object Result { get; set; }

        public bool IsHttpResponseMessage { get; set; }
    }
}