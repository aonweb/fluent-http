using System.Net.Http;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheEntry
    {
        private CacheEntry()
            : this(new ResponseMetadata())
        {
            IsEmpty = true;
        }

        public CacheEntry(object value, HttpRequestMessage request, HttpResponseMessage response, ICacheMetadata metadata)
            : this(value, CachingHelpers.CreateResponseMetadata(value, request, response, metadata)) { }

        public CacheEntry(object value, IResponseMetadata metadata)
            :this(metadata)
        {
            Value = value;
            IsHttpResponseMessage = value is HttpResponseMessage;
        }

        public CacheEntry(IResponseMetadata metadata)
        {
            Metadata = metadata;
        }

        public IResponseMetadata Metadata { get; private set; }
        public object Value { get; set; }
        public bool IsHttpResponseMessage { get; set; }
        public bool IsEmpty { get; }

        public void UpdateResponseMetadata(HttpRequestMessage request, HttpResponseMessage response, ICacheMetadata metadata)
        {
            Metadata = CachingHelpers.CreateResponseMetadata(Value, request, response, metadata);
        }

        public static CacheEntry Empty = new CacheEntry();
    }
}