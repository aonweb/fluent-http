using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Mocks
{
    public class QueuedMockHttpCallBuilder : HttpCallBuilder, IMockBuilder<QueuedMockHttpCallBuilder>
    {
        private readonly ResponseQueue<Func<HttpRequestMessage, HttpResponseMessage>> _responses;

        public QueuedMockHttpCallBuilder()
            : this(new ResponseQueue<Func<HttpRequestMessage, HttpResponseMessage>>(r => new HttpResponseMessage(HttpStatusCode.OK))) { }

        public QueuedMockHttpCallBuilder(ResponseQueue<Func<HttpRequestMessage, HttpResponseMessage>> responses)
            : base(new MockHttpClientBuilder())
        {
            _responses = responses;

            ConfigureClient(b => ((MockHttpClientBuilder)b).WithResponse(r => _responses.GetNext()(r)));
        }

        public static QueuedMockHttpCallBuilder CreateMock()
        {
            return new QueuedMockHttpCallBuilder().ConfigureMock();
        }

        public static QueuedMockHttpCallBuilder CreateMock(string baseUri)
        {
            var builder = CreateMock().WithBaseUri(baseUri);

            return (QueuedMockHttpCallBuilder)builder;
        }

        public static QueuedMockHttpCallBuilder CreateMock(Uri baseUri)
        {
            var builder = CreateMock().WithBaseUri(baseUri);

            return (QueuedMockHttpCallBuilder)builder;
        }

        public QueuedMockHttpCallBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responses.Add(responseFactory);

            return this;
        }

        public QueuedMockHttpCallBuilder WithResponse(HttpResponseMessage response)
        {
            return WithResponse(r => response);
        }

        public QueuedMockHttpCallBuilder WithResponse(ResponseInfo response)
        {
            return WithResponse(r => response.ToHttpResponseMessage());
        }
    }
}