namespace AonWeb.FluentHttp.Mocks
{
    public class QueuedMockTypedHttpCallBuilder : MockTypedHttpCallBuilder
    {
        public QueuedMockTypedHttpCallBuilder()
            : base(new QueuedMockHttpCallBuilder(), new QueuedMockFormatter()) { }
    }
}