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

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return await _client.SendAsync(request, completionOption, cancellationToken).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            return await _client.SendAsync(request, completionOption).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await _client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await _client.SendAsync(request).ConfigureAwait(false);
        }

        public void CancelPendingRequests()
        {
            _client.CancelPendingRequests();
        }

        public HttpRequestHeaders DefaultRequestHeaders => _client.DefaultRequestHeaders;

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