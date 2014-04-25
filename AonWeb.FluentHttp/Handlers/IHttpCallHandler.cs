using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AonWeb.Fluent.Http.Handlers
{
    public interface IHttpCallHandler
    {
        Task Sending(HttpCallContext<IHttpCallBuilder> context);
        Task Sent(HttpCallContext<IHttpCallBuilder> context);
    }

    public class HttpCallContext<T>: HttpCallBuilderSettings
    {
        public HttpCallContext(T builder, HttpCallBuilderSettings settings)
        {
            Builder = builder;
            Items = new ConcurrentDictionary<string, object>();
        }

        public HttpResponseMessage Response { get; set; }
        public IDictionary<string, object> Items { get; set; }
        public T Builder { get; set; }
    }
}