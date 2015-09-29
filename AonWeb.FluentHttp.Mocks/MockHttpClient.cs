using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpClient : IHttpClient, IResponseMocker<MockHttpClient>
    {
        private readonly HttpClient _client = new HttpClient();

        private readonly MockResponses<IMockRequestContext, IMockResponse> _responses;
        private static readonly ConcurrentDictionary<string, long> UrlCount = new ConcurrentDictionary<string, long>();

        public MockHttpClient()
            : this(new MockResponses<IMockRequestContext, IMockResponse>(() => new MockHttpResponseMessage())) { }

        public MockHttpClient(MockResponses<IMockRequestContext, IMockResponse> responses)
        {
            _responses = responses;
        }

        public long MaxResponseContentBufferSize { get; set; }

        public TimeSpan Timeout { get; set; } 

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
            var url = request.RequestUri.ToString();
            
            UrlCount.AddOrUpdate(url, 1, (u, c) =>
            {
                return c + 1;
            });

            long urlCount;

            UrlCount.TryGetValue(url, out urlCount);

            var context = new MockHttpRequestMessage(request)
            {
                RequestCount = 1,
                RequestCountForThisUrl = urlCount
            };

            var response = _responses.GetResponse(context);


            return Task.FromResult<HttpResponseMessage>(response.ToHttpResponseMessage());
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        public HttpRequestHeaders DefaultRequestHeaders => _client.DefaultRequestHeaders;

        public void CancelPendingRequests() { }
        public MockHttpClient WithResponse(Predicate<IMockRequestContext> predicate, Func<IMockRequestContext, IMockResponse> responseFactory)
        {
            _responses.Add(predicate, responseFactory);

            return this;
        }
    }
}