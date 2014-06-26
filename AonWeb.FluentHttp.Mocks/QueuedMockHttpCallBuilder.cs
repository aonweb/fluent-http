using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Mocks
{
    public class QueuedMockHttpCallBuilder : MockHttpCallBuilder
    {
        private readonly ResponseQueue<Func<HttpRequestMessage, HttpResponseMessage>> _responses;

        public QueuedMockHttpCallBuilder()
            : this(new ResponseQueue<Func<HttpRequestMessage, HttpResponseMessage>>(r => new HttpResponseMessage(HttpStatusCode.OK))) { }

        public QueuedMockHttpCallBuilder(ResponseQueue<Func<HttpRequestMessage, HttpResponseMessage>> responses)
        {
            _responses = responses;

            ConfigureClient(b => ((MockHttpClientBuilder)b).WithResponse(r => _responses.GetNext()(r)));
        }
    }
}