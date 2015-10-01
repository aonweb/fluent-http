using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public abstract class HttpHandlerContext : HttpBuilderContext, IHandlerContext
    {
        protected HttpHandlerContext(HttpHandlerContext context)
            : this(context, context.Request)
        { }

        protected HttpHandlerContext(IHttpBuilderContext context, HttpRequestMessage request)
            : base(context)
        {
            Request = request;
        }

        public abstract Modifiable GetHandlerResult();

        public HttpRequestMessage Request { get; }
    }
}