using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Client
{
    public class HttpClientWrapper : IHttpClient
    {
        public void Dispose()
        {
            _client.Dispose();
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return _client.SendAsync(request, completionOption, cancellationToken);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            return _client.SendAsync(request, completionOption);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _client.SendAsync(request, cancellationToken);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return _client.SendAsync(request);
        }

        public void CancelPendingRequests()
        {
            _client.CancelPendingRequests();
        }

        public HttpRequestHeaders DefaultRequestHeaders
        {
            get { return _client.DefaultRequestHeaders; }
        }

        public long MaxResponseContentBufferSize
        {
            get { return _client.MaxResponseContentBufferSize; }
            set { _client.MaxResponseContentBufferSize = value; }
        }

        public TimeSpan Timeout
        {
            get { return _client.Timeout; }
            set { _client.Timeout = value; }
        }

        private readonly HttpClient _client;

        public HttpClientWrapper(HttpClient client)
        {
            _client = client;
        }
    }
}