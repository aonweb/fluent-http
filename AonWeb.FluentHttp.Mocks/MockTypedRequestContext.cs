using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockTypedRequestContext : IMockTypedRequestContext
    {
        public MockTypedRequestContext(ITypedBuilderContext context, IMockRequestContext request)
        {
            BuilderContext = context;
            Request = request;
        }

        public IMockRequestContext Request { get; }
        public ITypedBuilderContext BuilderContext { get; }

        HttpContent IMockRequestContext.Content => Request?.Content;

        Encoding IMockRequestContext.ContentEncoding => Request?.ContentEncoding;

        string IMockRequestContext.ContentType => Request?.ContentType;

        HttpMethod IMockRequestContext.Method => Request?.Method;

        Uri IMockRequestContext.RequestUri => Request?.RequestUri;

        HttpRequestHeaders IMockRequestContext.Headers => Request?.Headers;

        long IMockRequestContext.RequestCount => Request?.RequestCount ?? 0;

        long IMockRequestContext.RequestCountForThisUrl => Request?.RequestCountForThisUrl ?? 0;
    }
}