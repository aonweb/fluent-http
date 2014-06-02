using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpClientBuilder : HttpClientBuilder
    {
        private Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        protected override IHttpClient GetClientInstance(HttpMessageHandler handler)
        {
            var client = new MockHttpClient();

            if (_responseFactory != null)
                client.ConfigureResponse(_responseFactory);

            return client;
        }

        public MockHttpClientBuilder ConfigureResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;

            return this;
        }
    }

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

    public class ResponseQueue<TRequest, TResponse>
        where TResponse : new()
    {
        private readonly Queue<Func<TRequest, TResponse>> _responses;
        private Func<TRequest, TResponse> _last;

        public ResponseQueue()
        {
            _responses = new Queue<Func<TRequest, TResponse>>();
            _last = request => new TResponse();
        }

        public ResponseQueue(IEnumerable<Func<TRequest, TResponse>> responses)
            :this()
        {
            AddRange(responses);
        }

        public Func<TRequest, TResponse> GetNext()
        {
           if (_responses.Count > 0)
                _last = _responses.Dequeue();

            return _last;
        }

        public Func<TRequest, TResponse> Replay()
        {
            return _last;
        }

        public ResponseQueue<TRequest, TResponse> RemoveNext()
        {
            if (_responses.Count > 0)
             _responses.Dequeue();

            return this;
        }

        public ResponseQueue<TRequest, TResponse> Add(Func<TRequest, TResponse> response)
        {
             _responses.Enqueue(response);

            return this;
        }

        public ResponseQueue<TRequest, TResponse> AddRange(IEnumerable<Func<TRequest, TResponse>> responses)
        {
            foreach (var response in responses)
                Add(response);

            return this;
        }
    }

    public class MockHttpCallBuilder : HttpCallBuilder
    {
        public MockHttpCallBuilder()
            : base(new MockHttpClientBuilder()) { }

        public MockHttpCallBuilder ConfigureResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            ConfigureClient(b => ((MockHttpClientBuilder)b).ConfigureResponse(responseFactory));

            return this;
        }

        public MockHttpCallBuilder WithResponse(HttpResponseMessage response)
        {
            return ConfigureResponse(r => response);
        }

        public MockHttpCallBuilder WithResponse(ResponseInfo response)
        {
            return ConfigureResponse(r => response.ToHttpResponseMessage());
        }

        public static MockHttpCallBuilder CreateMock()
        {
            return new MockHttpCallBuilder();
        }

        public static MockHttpCallBuilder CreateMock(string baseUri)
        {
            return (MockHttpCallBuilder)(CreateMock().WithBaseUri(baseUri));
        }

        public static MockHttpCallBuilder CreateMock(Uri baseUri)
        {
            return (MockHttpCallBuilder)(CreateMock().WithBaseUri(baseUri));
        }
    }

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

    public class MockFormatter<TResult, TContent, TError> : IHttpCallFormatter<TResult, TContent, TError>
    {
        private readonly IHttpCallFormatter<TResult, TContent, TError> _innerFormatter;

        private  Func<HttpResponseMessage,HttpCallContext<TResult, TContent, TError>, TResult> _resultFactory;
        private  Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TError> _errorFactory;

        public MockFormatter()
        {
            _innerFormatter = new HttpCallFormatter<TResult, TContent, TError>();
        }

        public Task<HttpContent> CreateContent<T>(T value, HttpCallContext<TResult, TContent, TError> context)
        {
            return _innerFormatter.CreateContent(value, context);
        }

        public Task<TResult> DeserializeResult(HttpResponseMessage response, HttpCallContext<TResult, TContent, TError> context)
        {
            if (_resultFactory != null) 
                return Task.FromResult(_resultFactory(response, context));

            return _innerFormatter.DeserializeResult(response, context);
        }

        public Task<TError> DeserializeError(HttpResponseMessage response, HttpCallContext<TResult, TContent, TError> context)
        {
            if (_errorFactory != null)
                return Task.FromResult(_errorFactory(response, context));

            return _innerFormatter.DeserializeError(response, context);
        }

        public MockFormatter<TResult, TContent, TError> ConfigureResult(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TResult> resultFactory)
        {
            _resultFactory = resultFactory;

            return this;
        }

        public MockFormatter<TResult, TContent, TError> ConfigureError(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TError> errorFactory)
        {
            _errorFactory = errorFactory;

            return this;
        }
    }

    public class MockHttpCallBuilder<TResult, TContent, TError> : HttpCallBuilder<TResult, TContent, TError>
    {
        private readonly MockFormatter<TResult, TContent, TError> _formatter;
        private readonly MockHttpCallBuilder _innerBuilder;

        public MockHttpCallBuilder()
            : this(new MockHttpCallBuilder(),new MockFormatter<TResult, TContent, TError>())
        { }

        private MockHttpCallBuilder(MockHttpCallBuilder builder, MockFormatter<TResult, TContent, TError> formatter)
            : base(builder, formatter)
        {
            _innerBuilder = builder;
            _formatter = formatter;
        }

        public MockHttpCallBuilder<TResult, TContent, TError> ConfigureResult(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TResult> resultFactory)
        {
            _formatter.ConfigureResult(resultFactory);

            return this;
        }

        public MockHttpCallBuilder<TResult, TContent, TError> WithResult(TResult result)
        {
            return WithResult(result, HttpStatusCode.OK);
        }

        public MockHttpCallBuilder<TResult, TContent, TError> WithResult(TResult result, HttpStatusCode statusCode)
        {
            _formatter.ConfigureResult((r, c) => result);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public MockHttpCallBuilder<TResult, TContent, TError> ConfigureError(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TError> errorFactory)
        {
            _formatter.ConfigureError(errorFactory);

            return this;
        }

        public MockHttpCallBuilder<TResult, TContent, TError> WithError(TError error)
        {
           return WithError(error, HttpStatusCode.InternalServerError);
        }

        public MockHttpCallBuilder<TResult, TContent, TError> WithError(TError error, HttpStatusCode statusCode)
        {
            _formatter.ConfigureError((r, c) => error);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public MockHttpCallBuilder<TResult, TContent, TError> ConfigureResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _innerBuilder.ConfigureResponse(responseFactory);

            return this;
        }

        public MockHttpCallBuilder<TResult, TContent, TError> WithResponse(HttpResponseMessage response)
        {
            return ConfigureResponse(r => response);
        }

        public MockHttpCallBuilder<TResult, TContent, TError> WithResponse(ResponseInfo response)
        {
            return ConfigureResponse(r => response.ToHttpResponseMessage());
        }

        public static MockHttpCallBuilder<TResult, TContent, TError> CreateMock()
        {
            return new MockHttpCallBuilder<TResult, TContent, TError>();
        }

        public static MockHttpCallBuilder<TResult, TContent, TError> CreateMock(string baseUri)
        {
            return (MockHttpCallBuilder<TResult, TContent, TError>)(CreateMock().WithBaseUri(baseUri));
        }

        public static MockHttpCallBuilder<TResult, TContent, TError> CreateMock(Uri baseUri)
        {
            return (MockHttpCallBuilder<TResult, TContent, TError>)(CreateMock().WithBaseUri(baseUri));
        }

    }

    //public class QueuedMockHttpCallBuilder<TResult, TContent, TError> : HttpCallBuilder<TResult, TContent, TError>
    //{
    //    private readonly ResponseQueue<HttpRequestMessage, HttpResponseMessage> _responses;

    //    public QueuedMockHttpCallBuilder()
    //        : base(new MockFormatter<TResult, TContent, TError>())
    //    {
    //        _result 
    //    }

    //    public QueuedMockHttpCallBuilder<TResult, TContent, TError> Add(Func<HttpRequestMessage, HttpResponseMessage> response)
    //    {
    //        _responses.Add(response);

    //        return this;
    //    }

    //    public QueuedMockHttpCallBuilder<TResult, TContent, TError> AddRange(IEnumerable<Func<HttpRequestMessage, HttpResponseMessage>> responses)
    //    {
    //        _responses.AddRange(responses);

    //        return this;
    //    }

    //    public QueuedMockHttpCallBuilder<TResult, TContent, TError> Add(Func<HttpRequestMessage, ResponseInfo> response)
    //    {
    //        return Add(r => response(r).ToHttpResponseMessage());
    //    }

    //    public QueuedMockHttpCallBuilder<TResult, TContent, TError> AddRange(IEnumerable<Func<HttpRequestMessage, ResponseInfo>> responses)
    //    {
    //        foreach (var response in responses)
    //        {
    //            Add(response);
    //        }

    //        return this;
    //    }

    //    public QueuedMockHttpCallBuilder<TResult, TContent, TError> Add(ResponseInfo response)
    //    {
    //        return Add(r => response.ToHttpResponseMessage());
    //    }

    //    public QueuedMockHttpCallBuilder<TResult, TContent, TError> AddRange(IEnumerable<ResponseInfo> responses)
    //    {
    //        foreach (var response in responses)
    //        {
    //            Add(response);
    //        }

    //        return this;
    //    }
    //}
}
