using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class LocalWebServer : IDisposable
    {
        public const string DefaultListenerUri = "http://localhost:8889/";
        private readonly HttpListener _listener = new HttpListener();
        private readonly Queue<Func<HttpListenerRequest, LocalWebServerResponseInfo>> _responses;
        private readonly AutoResetEvent _handle;
        private Func<HttpListenerRequest, LocalWebServerResponseInfo> _lastResponse;
        private Action<HttpListenerRequest> _requestInspector;

        public LocalWebServer()
            : this(DefaultListenerUri) { }

        public LocalWebServer(string listenerUri)
            : this(listenerUri, new Func<HttpListenerRequest, LocalWebServerResponseInfo>[0]) { }

        public LocalWebServer(Func<HttpListenerRequest, LocalWebServerResponseInfo>[] responses)
            : this(DefaultListenerUri, responses) { }

        public LocalWebServer(params LocalWebServerResponseInfo[] responses)
            : this(DefaultListenerUri, responses.Select(r => (Func<HttpListenerRequest, LocalWebServerResponseInfo>)(request => r)).ToArray()) { }

        public LocalWebServer(string listenerUri, params Func<HttpListenerRequest, LocalWebServerResponseInfo>[] responses)
        {
            _handle = new AutoResetEvent(false);
            _listener.Prefixes.Add(listenerUri);
            _responses = new Queue<Func<HttpListenerRequest, LocalWebServerResponseInfo>>();

            if (responses.Length > 0)
            {
                foreach (var response in responses)
                {
                    _responses.Enqueue(response);
                }
            }
            else
            {
                _lastResponse = request => new LocalWebServerResponseInfo();
            }
        }

        public static LocalWebServer ListenInBackground()
        {
            return ListenInBackground(DefaultListenerUri);
        }

        public static LocalWebServer ListenInBackground(string listenerUri)
        {
            var listener = new LocalWebServer(listenerUri);

            ThreadPool.QueueUserWorkItem(
                (o) =>
                {
                    listener.Start();
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

        public void Start()
        {
            _listener.Start();

            while (_listener.IsListening)
                ProcessRequest();
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
            _handle.Set();
        }

        public LocalWebServer AddResponse(LocalWebServerResponseInfo response)
        {
            return AddResponse(request => response);
        }

        public LocalWebServer AddResponse(Func<HttpListenerRequest, LocalWebServerResponseInfo> response)
        {
            _responses.Enqueue(response);

            return this;
        }

        private void ProcessRequest()
        {
            var result = _listener.BeginGetContext(ListenerCallback, _listener);
            result.AsyncWaitHandle.WaitOne();
        }

        private void ListenerCallback(IAsyncResult result)
        {
            var context = _listener.EndGetContext(result);

            Log(context.Request);

            if (_requestInspector != null)
                _requestInspector(context.Request);

            var responseInfo = GetResponseInfo(context);

            CreateResponse(context, responseInfo);
        }

        private void Log(HttpListenerRequest request)
        {
            Console.WriteLine("Request: {0} - {1}", request.HttpMethod, request.Url);

            if (request.HasEntityBody)
            {
                var encoding = request.ContentEncoding;
                using (var bodyStream = request.InputStream)
                using (var streamReader = new StreamReader(bodyStream, encoding))
                {
                    if (request.ContentType != null)
                        Console.WriteLine("   ContentType: {0}", request.ContentType);

                    Console.WriteLine("   ContentLength: {0}", request.ContentLength64);
                    Console.WriteLine("   Body:");
                    Console.WriteLine(streamReader.ReadToEnd());
                }
            }

            Console.WriteLine("---- End Request ----");
            Console.WriteLine();
        }

        private LocalWebServerResponseInfo GetResponseInfo(HttpListenerContext context)
        {
            if (_responses.Count > 0)
                _lastResponse = _responses.Dequeue();

            return _lastResponse(context.Request);
        }

        private void CreateResponse(HttpListenerContext context, LocalWebServerResponseInfo responseInfo)
        {
            var response = context.Response;
            response.StatusCode = (int)responseInfo.StatusCode;
            response.StatusDescription = responseInfo.StatusDescription ?? responseInfo.StatusCode.ToString();

            foreach (var header in responseInfo.Headers)
                response.AddHeader(header.Key, header.Value);

            try
            {
                var buffer = responseInfo.ContentEncoding.GetBytes(responseInfo.Body);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
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

        public void InspectRequest(Action<HttpListenerRequest> inspector)
        {
            _requestInspector = inspector;
        }
    }
}