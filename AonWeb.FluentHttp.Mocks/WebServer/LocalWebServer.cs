using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Mocks.WebServer
{
    public class LocalWebServer : IDisposable
    {
        public const string DefaultListenerUri = "http://localhost:8889/";

        private readonly List<KeyValuePair<Predicate<ILocalRequestContext>, ILocalResponse>> _responses;
        private readonly AutoResetEvent _handle;
        private readonly IList<string> _prefixes;

        private HttpListener _listener = new HttpListener();
        private Action<ILocalRequestContext> _requestInspector;
        private long _totalCount;
        private readonly ConcurrentDictionary<string, long> _urlCount;
        private bool _loggingEnabled;

        public LocalWebServer()
            : this(DefaultListenerUri)
        { }


        public LocalWebServer(string listenerUri)
        {
            _loggingEnabled = true;
            _prefixes = new List<string> { listenerUri };
            _handle = new AutoResetEvent(false);
            _responses = new List<KeyValuePair<Predicate<ILocalRequestContext>, ILocalResponse>>();

            _totalCount = 0;

            _urlCount = new ConcurrentDictionary<string, long>();
        }

        public static LocalWebServer ListenInBackground()
        {
            return ListenInBackground(DefaultListenerUri);
        }

        public static LocalWebServer ListenInBackground(Uri listenerUri, params string[] additionalUris)
        {
            return ListenInBackground(listenerUri.OriginalString, additionalUris);
        }

        public static LocalWebServer ListenInBackground(string listenerUri, params string[] additionalUris)
        {
            var listener = new LocalWebServer(listenerUri);

            Task.Run(
                () =>
                    {
                        listener.Start(additionalUris);
                        listener.WaitHandle.WaitOne();
                    });

            return listener;
        }

        public WaitHandle WaitHandle => _handle;

        public LocalWebServer WithLogging(bool logging)
        {
            _loggingEnabled = logging;

            return this;
        }

        public LocalWebServer Start(params string[] additionalUris)
        {
            _listener = new HttpListener();

            foreach (var prefix in _prefixes.Concat(additionalUris).Distinct())
                _listener.Prefixes.Add(prefix);

            _listener.Start();

            while (_listener.IsListening)
            {
                var result = _listener.BeginGetContext(ListenerCallback, _listener);
                result.AsyncWaitHandle.WaitOne();
            }

            return this;
        }

        public LocalWebServer Stop()
        {
            if (_listener != null && _listener.IsListening)
            {
                _listener.Stop();
                _listener.Close();
            }

            _handle.Set();

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

        public LocalWebServer WithResponse(KeyValuePair<Predicate<ILocalRequestContext>, ILocalResponse> response)
        {
            _responses.Add(response);

            return this;
        }

        public LocalWebServer WithRequestInspector(Action<ILocalRequestContext> visitor)
        {
            _requestInspector = (Action<ILocalRequestContext>)Delegate.Combine(_requestInspector, visitor);

            return this;
        }

        private void ListenerCallback(IAsyncResult result)
        {
            HttpListenerContext context;
            try
            {
                var listener = (HttpListener)result.AsyncState;
                context = listener.EndGetContext(result);
            }
            catch (Exception)
            {
                return;
            }

            var request = context.Request;
            var url = request.Url.ToString();
            long urlCount;

            _urlCount.TryGetValue(url, out urlCount);

            var requestInfo = new LocalRequestSettings
            {
                ContentEncoding = request.ContentEncoding,
                ContentLength = request.ContentLength64,
                ContentType = request.ContentType,
                HasEntityBody = request.HasEntityBody,
                Headers = request.Headers,
                HttpMethod = request.HttpMethod,
                Url = request.Url,
                UrlReferrer = request.UrlReferrer,
                RawUrl = request.RawUrl,
                AcceptTypes = request.AcceptTypes,
                UserAgent = request.UserAgent,
                Body = request.ReadContents(),
                RequestCount = _totalCount,
                RequestCountForThisUrl = urlCount
            };

            Log(requestInfo);

            _requestInspector?.Invoke(requestInfo);

            var responseInfo = GetResponseInfo(requestInfo);

            CreateResponse(context, responseInfo);

            Interlocked.Increment(ref _totalCount);
            _urlCount.AddOrUpdate(url, 0, (u, c) => c + 1);
        }

        private void Log(ILocalRequestContext request)
        {
            if (!_loggingEnabled)
                return;

            Debug.WriteLine("Request: {0} - {1}", request.HttpMethod, request.Url);

            if (request.HasEntityBody)
            {
                if (request.ContentType != null)
                    Debug.WriteLine("   ContentType: {0}", request.ContentType);

                Debug.WriteLine("   ContentLength: {0}", request.ContentLength);
                Debug.WriteLine("   Content:");
                Debug.WriteLine(request.Body);
                Debug.WriteLine("---- End Request ----");
                Debug.WriteLine("");
            }
        }

        private void Log(HttpListenerResponse response, string body)
        {
            if (!_loggingEnabled)
                return;

            Debug.WriteLine("Response: {0} - {1}", response.StatusCode, response.StatusDescription);
            Debug.WriteLine("  Headers:");
            foreach (var name in response.Headers.AllKeys)
            {
                Debug.WriteLine("   {0}: {1}", name, response.Headers[name]);
            }

            if (!string.IsNullOrEmpty(body))
            {
                Debug.WriteLine("   Content: {0}", body);
            }

            Debug.WriteLine("---- End Response ----");
            Debug.WriteLine("");
        }

        private ILocalResponse GetResponseInfo(ILocalRequestContext context)
        {
            KeyValuePair<Predicate<ILocalRequestContext>, ILocalResponse> response;

            lock (_responses)
            {
                response = _responses.FirstOrDefault(kp => kp.Key(context));

                if (response.Value?.IsTransient == true)
                    _responses.Remove(response);
            }

            return response.Value ?? new LocalResponse();
        }

        private void CreateResponse(HttpListenerContext context, ILocalResponse responseInfo)
        {
            var response = context.Response;
            response.StatusCode = (int)responseInfo.StatusCode;
            response.StatusDescription = responseInfo.StatusDescription ?? responseInfo.StatusCode.ToString();

            foreach (var header in responseInfo.Headers)
                response.AddHeader(header.Key, header.Value);

            response.AddHeader("Date", DateTime.UtcNow.ToString("R"));

            if (!string.IsNullOrWhiteSpace(responseInfo.ContentType) && string.IsNullOrWhiteSpace(response.Headers["Content-Type"]))
                response.AddHeader("Content-Type", responseInfo.ContentType);

            try
            {
                var buffer = responseInfo.ContentEncoding.GetBytes(responseInfo.Content);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);

                Log(response, responseInfo.Content);
            }
            finally
            {
                response.OutputStream.Close();
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