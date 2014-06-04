using System;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class QueuedMockHttpCallBuilder : HttpCallBuilder, IMockBuilder
    {
        private readonly ResponseQueue<Func<HttpRequestMessage, HttpResponseMessage>> _responses;

        public QueuedMockHttpCallBuilder()
            : this(new ResponseQueue<Func<HttpRequestMessage, HttpResponseMessage>>(r => new HttpResponseMessage(HttpStatusCode.OK))) { }

        public QueuedMockHttpCallBuilder(ResponseQueue<Func<HttpRequestMessage, HttpResponseMessage>> responses)
            : base(new MockHttpClientBuilder())
        {
            _responses = responses;

            ConfigureClient(b => ((MockHttpClientBuilder)b).WithResponse(r => _responses.GetNext()(r)));
        }

        public IMockBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responses.Add(responseFactory);

            return this;
        }

        public IMockBuilder WithResponse(HttpResponseMessage response)
        {
            return WithResponse(r => response);
        }

        public IMockBuilder WithResponse(ResponseInfo response)
        {
            return WithResponse(r => response.ToHttpResponseMessage());
        }
    }

    public class QueuedMockHttpCallBuilder<TResult, TContent, TError> :
        HttpCallBuilder<TResult, TContent, TError>,
        IMockBuilder<TResult, TContent, TError>
    {
        private readonly QueuedMockFormatter<TResult, TContent, TError> _formatter;
        private readonly QueuedMockHttpCallBuilder _innerBuilder;

        protected internal QueuedMockHttpCallBuilder()
        {
            _innerBuilder = new QueuedMockHttpCallBuilder();
            _formatter = new QueuedMockFormatter<TResult, TContent, TError>();
        }

        public static QueuedMockHttpCallBuilder<TResult, TContent, TError> CreateMock()
        {
            return new QueuedMockHttpCallBuilder<TResult, TContent, TError>();
        }

        public static QueuedMockHttpCallBuilder<TResult, TContent, TError> CreateMock(string baseUri)
        {
            return (QueuedMockHttpCallBuilder<TResult, TContent, TError>)(CreateMock().WithBaseUri(baseUri));
        }

        public static QueuedMockHttpCallBuilder<TResult, TContent, TError> CreateMock(Uri baseUri)
        {
            return (QueuedMockHttpCallBuilder<TResult, TContent, TError>)(CreateMock().WithBaseUri(baseUri));
        }

        public IMockBuilder<TResult, TContent, TError> WithResult(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TResult> resultFactory)
        {
            _formatter.WithResult(resultFactory);

            return this;
        }

        public IMockBuilder<TResult, TContent, TError> WithResult(TResult result)
        {
            return WithResult(result, HttpStatusCode.OK);
        }

        public IMockBuilder<TResult, TContent, TError> WithResult(TResult result, HttpStatusCode statusCode)
        {
            _formatter.WithResult((r, c) => result);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public IMockBuilder<TResult, TContent, TError> WithError(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TError> errorFactory)
        {
            _formatter.WithError(errorFactory);

            return this;
        }

        public IMockBuilder<TResult, TContent, TError> WithError(TError error)
        {
            return WithError(error, HttpStatusCode.InternalServerError);
        }

        public IMockBuilder<TResult, TContent, TError> WithError(TError error, HttpStatusCode statusCode)
        {
            _formatter.WithError((r, c) => error);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public IMockBuilder<TResult, TContent, TError> WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _innerBuilder.WithResponse(responseFactory);

            return this;
        }

        public IMockBuilder<TResult, TContent, TError> WithResponse(HttpResponseMessage response)
        {
            return WithResponse(r => response);
        }

        public IMockBuilder<TResult, TContent, TError> WithResponse(ResponseInfo response)
        {
            return WithResponse(r => response.ToHttpResponseMessage());
        }
    }
}