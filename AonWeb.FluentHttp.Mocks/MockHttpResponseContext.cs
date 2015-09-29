namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpResponseContext: MockHttpResponseMessage
    {
        public MockHttpResponseContext(MockHttpResponseMessage response)
            : base(response)
        {
            IsTransient = response.IsTransient;
            RequestCount = response.RequestCount;
            RequestCountForThisUrl = response.RequestCountForThisUrl;
        }
    }
}