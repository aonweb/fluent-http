using System;
using System.Collections.Generic;
using System.Net.Http;

namespace AonWeb.FluentHttp.Mocks
{
    public class QueuedMockHttpCallBuilder : HttpCallBuilder
    {
        private readonly ResponseQueue<HttpRequestMessage, HttpResponseMessage> _responses;

        public QueuedMockHttpCallBuilder()
            : base(new MockHttpClientBuilder())
        {
            _responses = new ResponseQueue<HttpRequestMessage, HttpResponseMessage>();

            ConfigureClient(b => ((MockHttpClientBuilder)b).ConfigureResponse(r => _responses.GetNext()(r)));
        }

        public virtual QueuedMockHttpCallBuilder ConfigureResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            return this;
        }

        public QueuedMockHttpCallBuilder Add(Func<HttpRequestMessage, HttpResponseMessage> response)
        {
            _responses.Add(response);

            return this;
        }

        public QueuedMockHttpCallBuilder AddRange(IEnumerable<Func<HttpRequestMessage, HttpResponseMessage>> responses)
        {
            _responses.AddRange(responses);

            return this;
        }

        public QueuedMockHttpCallBuilder Add(Func<HttpRequestMessage, ResponseInfo> response)
        {
            return Add(r => response(r).ToHttpResponseMessage());
        }

        public QueuedMockHttpCallBuilder AddRange(IEnumerable<Func<HttpRequestMessage, ResponseInfo>> responses)
        {
            foreach (var response in responses)
            {
                Add(response);
            }

            return this;
        }

        public QueuedMockHttpCallBuilder Add(ResponseInfo response)
        {
            return Add(r => response.ToHttpResponseMessage());
        }

        public QueuedMockHttpCallBuilder AddRange(IEnumerable<ResponseInfo> responses)
        {
            foreach (var response in responses)
            {
                Add(response);
            }

            return this;
        }
    }
}