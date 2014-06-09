namespace AonWeb.FluentHttp.Handlers
{
    public abstract class TypedHttpCallHandlerContext : TypedHttpCallContext
    {
        protected TypedHttpCallHandlerContext(TypedHttpCallContext context)
            : base(context) { }

        protected TypedHttpCallHandlerContext(IRecursiveTypedHttpCallBuilder builder, TypedHttpCallBuilderSettings settings)
            : base(builder, settings) { }

        public abstract ModifyTracker GetHandlerResult();
    }
}