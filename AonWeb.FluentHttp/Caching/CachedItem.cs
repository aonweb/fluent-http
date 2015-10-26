using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Caching
{
    public class CachedItem
    {
        public CachedItem(IResponseMetadata responseInfo)
        {
            ResponseInfo = responseInfo;
        }

        public IResponseMetadata ResponseInfo { get; private set; }
        public object Result { get; set; }

        public bool IsHttpResponseMessage { get; set; }
    }
}