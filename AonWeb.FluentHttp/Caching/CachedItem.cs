using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Caching
{
    public class CachedItem
    {
        public CachedItem(IWritableResponseMetadata responseInfo)
        {
            ResponseInfo = responseInfo;
        }

        public IWritableResponseMetadata ResponseInfo { get; private set; }
        public object Result { get; set; }

        public bool IsHttpResponseMessage { get; set; }
    }
}