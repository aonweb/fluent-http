using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class LocalWebServer : IDisposable
    {
        public const string DefaultListenerUri = "http://localhost:8889/";
        private readonly HttpListener _listener = new HttpListener();
        private readonly Queue<Func<LocalWebServerRequestInfo, LocalWebServerResponseInfo>> _responses;
        private readonly AutoResetEvent _handle;
        private Func<LocalWebServerRequestInfo, LocalWebServerResponseInfo> _lastResponse;
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
            _handle = new AutoResetEvent(false);
            _listener.Prefixes.Add(listenerUri);
            _responses = new Queue<Func<LocalWebServerRequestInfo, LocalWebServerResponseInfo>>();

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
                _ =>
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

        public LocalWebServer AddResponse(Func<LocalWebServerRequestInfo, LocalWebServerResponseInfo> response)
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

            var requestInfo = GetRequestInfo(context.Request);

            Log(requestInfo);

            try
            {
                if (_requestInspector != null)
                    _requestInspector(requestInfo);
            }
            catch (Exception)
            {
                
            }
            

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
            Console.WriteLine("Request: {0} - {1}", request.HttpMethod, request.Url);

            if (request.HasEntityBody)
            {
                    if (request.ContentType != null)
                        Console.WriteLine("   ContentType: {0}", request.ContentType);

                    Console.WriteLine("   ContentLength: {0}", request.ContentLength);
                    Console.WriteLine("   Body:");
                    Console.WriteLine(request.Body);
            }

            Console.WriteLine("---- End Request ----");
            Console.WriteLine();
        }

        private LocalWebServerResponseInfo GetResponseInfo(LocalWebServerRequestInfo requestInfo)
        {
            if (_responses.Count > 0)
                _lastResponse = _responses.Dequeue();

            return _lastResponse(requestInfo);
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

        public void InspectRequest(Action<LocalWebServerRequestInfo> inspector)
        {
            _requestInspector = inspector;
        }
    }
}