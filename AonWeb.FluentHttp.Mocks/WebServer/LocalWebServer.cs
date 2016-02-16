using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Mocks.WebServer
{
    public class LocalWebServer : IDisposable, IResponseMocker<LocalWebServer>
    {
        private static readonly ISet<int> PortsInUse = new HashSet<int>();
        private static int _minPort = 9000;
        private static int _maxPort = 10000;

        public const string DefaultListenerHost = "localhost";

        private readonly MockResponses<IMockRequestContext, IMockResponse> _responses;
        private readonly IList<string> _prefixes;

        private HttpListener _listener = new HttpListener();
        private readonly IList<Func<IMockRequestContext, Task>> _requestInspectors;
        private long _totalCount;
        private readonly ConcurrentDictionary<string, long> _urlCount;
        private bool _loggingEnabled;
        private IMockLogger _logger;

        public int Port { get; private set; }
        private UriBuilder ListeningUriBuilder { get; }
        public Uri ListeningUri => ListeningUriBuilder.Uri;

        public LocalWebServer(IMockLogger logger = null)
            : this(logger, DefaultListenerHost)
        { }

        public LocalWebServer(string listenerHost)
            : this(null, listenerHost)
        { }

        public LocalWebServer(IMockLogger logger, string listenerHost)
        {
            GenerateUniquePort();
            _loggingEnabled = true;
            _logger = logger ?? new NullMockLogger();

            ListeningUriBuilder = new UriBuilder
            {
                Scheme = "http",
                Host = listenerHost,
                Port = Port
            };

            _prefixes = new List<string> { ListeningUri.ToString() };
            _responses = new MockResponses<IMockRequestContext, IMockResponse>();
            _requestInspectors = new List<Func<IMockRequestContext, Task>>();

            _totalCount = 0;

            _urlCount = new ConcurrentDictionary<string, long>();
        }

        private void GenerateUniquePort()
        {
            lock (PortsInUse)
            {
                for (var port = _minPort; port <= _maxPort; port++)
                {
                    if (!PortsInUse.Contains(port))
                    {
                        Port = port;
                        PortsInUse.Add(port);
                        break;
                    }  
                }
            }
        }

        public static LocalWebServer ListenInBackground(IMockLogger logger = null, string listenerHost = null, params string[] additionalUris)
        {
            var listener = new LocalWebServer(logger ?? new NullMockLogger(), listenerHost ?? DefaultListenerHost);

            Task.Run(() => listener.Start(additionalUris));

            return listener;
        }

        public LocalWebServer WithLogging(bool logging)
        {
            _loggingEnabled = logging;

            return this;
        }

        public async Task Start(params string[] additionalUris)
        {
            _listener = new HttpListener();

            foreach (var prefix in _prefixes.Concat(additionalUris).Distinct())
                _listener.Prefixes.Add(prefix);

            _listener.Start();

            while (_listener.IsListening)
            {
                var context = await _listener.GetContextAsync();

                try
                {
                    await FulfillRequest(context);
                }
                catch (Exception ex)
                {
                    _listener.Stop();
                    if(!(ex is OperationCanceledException))
                        throw;
                }
                
            }
        }

        public LocalWebServer Stop()
        {
            if (_listener != null && _listener.IsListening)
            {
                _listener.Stop();
                _listener.Close();
            }

            lock (PortsInUse)
            {
                PortsInUse.Remove(Port);
            }

            return this;
        }

        public LocalWebServer Reset()
        {
            Stop();

            _loggingEnabled = true;

            _responses.Clear();

            _totalCount = 0;

            _urlCount.Clear();

            return this;
        }

        public LocalWebServer WithResponse(Predicate<IMockRequestContext> predicate, Func<IMockRequestContext, IMockResponse> responseFactory)
        {
            _responses.Add(predicate, responseFactory);

            return this;
        }

        public LocalWebServer WithRequestInspector(Action<IMockRequestContext> visitor)
        {
            return WithRequestInspector(ctx =>
            {
                visitor?.Invoke(ctx);

                return Task.FromResult<object>(null);
            });
        }

        public LocalWebServer WithRequestInspector(Func<IMockRequestContext, Task> visitor)
        {
            if (visitor != null)
                _requestInspectors.Add(visitor);

            return this;
        }

        private async Task FulfillRequest(HttpListenerContext context)
        {
            try
            {
                var listenerRequest = context.Request;
                var url = listenerRequest.Url.ToString();

                Interlocked.Increment(ref _totalCount);
                _urlCount.AddOrUpdate(url, 1, (u, c) =>
                {
                    return c + 1;
                });

                long urlCount;

                _urlCount.TryGetValue(url, out urlCount);

                var request = new MockHttpRequestMessage(listenerRequest)
                {
                    RequestCount = _totalCount,
                    RequestCountForThisUrl = urlCount
                };

                //get response before we await anything to make sure it matches request.
                var response = _responses.GetResponse(request);

                await Log(request);

                var visitTasks = _requestInspectors.Select(visitor => visitor(request)).ToList();

                await Task.WhenAll(visitTasks);

                await CreateResponse(context, response);
            }
            catch (Exception ex)
            {
                _logger.WriteLine("Error: " + ex.Message);
            }
            
        }

        private async Task Log(IMockRequestContext request)
        {
            if (!_loggingEnabled)
                return;

            _logger.WriteLine("Request: {0} - {1}", request.Method, request.RequestUri);

            if (request.Content != null)
            {
                if (request.ContentType != null)
                    _logger.WriteLine("   ContentType: {0}", request.Content.Headers.ContentType);

                _logger.WriteLine("   ContentLength: {0}", request.Content.Headers.ContentLength);
                _logger.WriteLine("   Content:");
                var content = await request.Content.ReadAsStringAsync();
                _logger.WriteLine(content);
                _logger.WriteLine("---- End Request ----");
                _logger.WriteLine("");
            }
        }

        private void Log(HttpListenerResponse response, byte[] content)
        {
            if (!_loggingEnabled)
                return;

            _logger.WriteLine("Response: {0} - {1}", response.StatusCode, response.StatusDescription);
            _logger.WriteLine("  Headers:");
            foreach (var name in response.Headers.AllKeys)
            {
                _logger.WriteLine("   {0}: {1}", name, response.Headers[name]);
            }

            if (content?.Length > 0)
            {
                _logger.WriteLine("   Content: {0}", Encoding.UTF8.GetString(content));
            }

            _logger.WriteLine("---- End Response ----");
            _logger.WriteLine("");
        }

        private async Task CreateResponse(HttpListenerContext context, IMockResponse response)
        {
            response = response ?? new MockHttpResponseMessage();
            var listenerResponse = context.Response;
            listenerResponse.StatusCode = (int)response.StatusCode;
            listenerResponse.StatusDescription = response.ReasonPhrase ?? response.StatusCode.ToString();

            foreach (var header in response.Headers)
                foreach (var value in header.Value)
                {
                    listenerResponse.AddHeader(header.Key, value);
                }

            listenerResponse.AddHeader("Date", DateTime.UtcNow.ToString("R"));

            if (!string.IsNullOrWhiteSpace(response.ContentType) && string.IsNullOrWhiteSpace(listenerResponse.Headers["Content-Type"]))
                listenerResponse.AddHeader("Content-Type", response.ContentType);

            try
            {
                byte[] buffer = null;

                if (response.Content != null)
                {
                    buffer = await response.Content.ReadAsByteArrayAsync();

                    buffer = buffer ?? new byte[0];

                    listenerResponse.ContentLength64 = buffer.Length;
                    listenerResponse.OutputStream.Write(buffer, 0, buffer.Length);
                } 

                Log(listenerResponse, buffer);
            }
            finally
            {
                listenerResponse.OutputStream.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
        }
    }
}