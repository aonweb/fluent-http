namespace AonWeb.FluentHttp.Mocks
{
    public class QueuedMockTypedHttpCallBuilder : MockTypedHttpCallBuilder
    {
        public QueuedMockTypedHttpCallBuilder()
            : this(new TypedHttpCallBuilderSettings())
        { }

        public QueuedMockTypedHttpCallBuilder(TypedHttpCallBuilderSettings settings)
            : base(settings, new QueuedMockHttpCallBuilder(), new QueuedMockFormatter())
        { }
    }
}