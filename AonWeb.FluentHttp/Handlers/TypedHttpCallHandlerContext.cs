using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{

    public interface IHttpCallHandlerContext : IHttpCallContext, IHandlerContext
    {
        HttpRequestMessage Request { get; }
    }

    public interface IHandlerContext
    {
        ModifyTracker GetHandlerResult();
    }

    public abstract class TypedHttpCallHandlerContext : TypedHttpCallContext, IHttpCallHandlerContext
    {
        protected TypedHttpCallHandlerContext(TypedHttpCallHandlerContext context)
            : this(context, context.Request) { }

        protected TypedHttpCallHandlerContext(TypedHttpCallContext context, HttpRequestMessage request)
            : base(context)
        {
            Request = request;
        }

        protected TypedHttpCallHandlerContext(
            IRecursiveTypedHttpCallBuilder builder,
            TypedHttpCallBuilderSettings settings,
            HttpRequestMessage request)
            : base(builder, settings)
        {
            Request = request;
        }

        public abstract ModifyTracker GetHandlerResult();

        public HttpRequestMessage Request { get; private set; }
    }


}