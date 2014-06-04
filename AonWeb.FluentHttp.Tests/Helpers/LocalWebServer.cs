using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Mocks;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class LocalWebServer : IDisposable
    {
        public const string DefaultListenerUri = "http://localhost:8889/";
       
        private readonly ResponseQueue<Func<LocalWebServerRequestInfo, LocalWebServerResponseInfo>> _responses;
        private readonly AutoResetEvent _handle;
        private readonly IList<string> _prefixes;

        private HttpListener _listener = new HttpListener();
        private Action<LocalWebServerRequestInfo> _requestInspector;

        public LocalWebServer()
            : this(DefaultListenerUri) { }

        public LocalWebServer(string listenerUri)
            : this(listenerUri, new Func<LocalWebServerRequestInfo, LocalWebServerResponseInfo>[0]) { }

        public LocalWebServer(Func<LocalWebServerRequestInfo, LocalWebServerResponseInfo>[] responses)
            : this(DefaultListenerUri, responses) { }

        public LocalWebServer(params LocalWebServerResponseInfo[] responses)
            : this(DefaultListenerUri, responses.Select(r => (Func<LocalWebServerRequestInfo, LocalWebServerResponseInfo>)(request => r)).ToArray()) { }

        public LocalWebServer(string listenerUri, params Func<LocalWebServerRequestInfo, LocalWebServerResponseInfo>[] responses)
        {
            EnableLogging = true;
            _prefixes = new List<string> { listenerUri };
            _handle = new AutoResetEvent(false);
            _responses = new ResponseQueue<Func<LocalWebServerRequestInfo, LocalWebServerResponseInfo>>(r => new LocalWebServerResponseInfo(), responses);
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

        public WaitHandle WaitHandle
        {
            get
            {
                return _handle;
            }
        }

        public bool EnableLogging { get; set; }

        public void Start(params string[] additionalUris)
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
        }

        public void Stop()
        {
            if (_listener != null)
            {
                _listener.Stop();
                _listener.Close();
            }

            _handle.Set();
        }

        public LocalWebServer AddResponse(LocalWebServerResponseInfo response)
        {
            return AddResponse(request => response);
        }

        public LocalWebServer AddResponse(Func<LocalWebServerRequestInfo, LocalWebServerResponseInfo> response)
        {
            _responses.Add(response);

            return this;
        }

        public LocalWebServer InspectRequest(Action<LocalWebServerRequestInfo> inspector)
        {
            _requestInspector = inspector;

            return this;
        }

        public LocalWebServer RemoveNextResponse()
        {
             _responses.RemoveNext();

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

            var requestInfo = GetRequestInfo(context.Request);

            Log(requestInfo);

            if (_requestInspector != null)
                _requestInspector(requestInfo);

            var responseInfo = GetResponseInfo(requestInfo);

            CreateResponse(context, responseInfo);
        }

        private LocalWebServerRequestInfo GetRequestInfo(HttpListenerRequest request)
        {
            return new LocalWebServerRequestInfo
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
                Body = request.ReadContents()
            };
        }

        private void Log(LocalWebServerRequestInfo request)
        {
            if (!EnableLogging)
                return;

            Console.WriteLine("Request: {0} - {1}", request.HttpMethod, request.Url);

            if (request.HasEntityBody)
            {
                if (request.ContentType != null)
                    Console.WriteLine("   ContentType: {0}", request.ContentType);

                Console.WriteLine("   ContentLength: {0}", request.ContentLength);
                Console.WriteLine("   Body:");
                Console.WriteLine(request.Body);
                Console.WriteLine("---- End Request ----");
                Console.WriteLine();
            } 
        }

        private void Log(HttpListenerResponse response, string body)
        {
            if (!EnableLogging)
                return;

            Console.WriteLine("Response: {0} - {1}", response.StatusCode, response.StatusDescription);
            Console.WriteLine("  Headers:");
            foreach (var name in response.Headers.AllKeys)
            {
                Console.WriteLine("   {0}: {1}", name, response.Headers[name]);
            }

            if (!string.IsNullOrEmpty(body))
            {
                Console.WriteLine("   Body: {0}", body);
            }

            Console.WriteLine("---- End Response ----");
            Console.WriteLine();
        }

        private LocalWebServerResponseInfo GetResponseInfo(LocalWebServerRequestInfo requestInfo)
        {
            return _responses.GetNext()(requestInfo);
        }

        private void CreateResponse(HttpListenerContext context, LocalWebServerResponseInfo responseInfo)
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
                var buffer = responseInfo.ContentEncoding.GetBytes(responseInfo.Body);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);

                Log(response, responseInfo.Body);
            }
            finally
            {
                response.OutputStream.Close();
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}