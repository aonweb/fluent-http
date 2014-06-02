using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpClient : IHttpClient
    {
        private readonly HttpClient _client = new HttpClient();

        private Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public MockHttpClient()
            : this(r => new HttpResponseMessage(HttpStatusCode.OK)) { }

        public MockHttpClient(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            ConfigureResponse(responseFactory);
        }

        public long MaxResponseContentBufferSize { get; set; }

        public TimeSpan Timeout { get; set; } 

        public IHttpClient ConfigureResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;

            return this;
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return SendAsync(request);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            return SendAsync(request);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return SendAsync(request);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return Task.FromResult(_responseFactory(request));
        }

        public void Dispose()
        {
            if (_client != null)
                _client.Dispose();
        }

        public HttpRequestHeaders DefaultRequestHeaders
        {
            get
            {
                return _client.DefaultRequestHeaders;
            }
        }

        public void CancelPendingRequests() { }


    }
}