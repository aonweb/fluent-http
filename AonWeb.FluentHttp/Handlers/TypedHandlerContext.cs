using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public abstract class TypedHandlerContext : TypedBuilderContext, IHandlerContext
    {
        protected TypedHandlerContext(TypedHandlerContext context)
            : this(context, context.Request) { }

        protected TypedHandlerContext(ITypedBuilderContext context, HttpRequestMessage request)
            : base(context)
        {
            Request = request;
        }

        public abstract Modifiable GetHandlerResult();

        public HttpRequestMessage Request { get;}
    }
}