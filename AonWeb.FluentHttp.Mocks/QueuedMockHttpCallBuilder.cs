using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class QueuedMockHttpCallBuilder : HttpCallBuilder, IMockBuilder<QueuedMockHttpCallBuilder>, IMockHttpCallBuilder
    {
        private readonly ResponseQueue<Func<HttpRequestMessage, HttpResponseMessage>> _responses;
        private readonly IList<IAssertAction> _asserts;
        private Action _assertFailure;

        public QueuedMockHttpCallBuilder()
            : this(new ResponseQueue<Func<HttpRequestMessage, HttpResponseMessage>>(r => new HttpResponseMessage(HttpStatusCode.OK))) { }

        public QueuedMockHttpCallBuilder(ResponseQueue<Func<HttpRequestMessage, HttpResponseMessage>> responses)
            : base(new MockHttpClientBuilder())
        {
            _responses = responses;
            _asserts = new List<IAssertAction>();
            _assertFailure = (() => { throw new Exception("assertion was never called"); });

            ConfigureClient(b => ((MockHttpClientBuilder)b).WithResponse(r => _responses.GetNext()(r)));
        }

        public static QueuedMockHttpCallBuilder CreateMock()
        {
            return new QueuedMockHttpCallBuilder().ConfigureMock();
        }

        public static QueuedMockHttpCallBuilder CreateMock(string baseUri)
        {
            var builder = CreateMock().WithBaseUri(baseUri);

            return (QueuedMockHttpCallBuilder)builder;
        }

        public static QueuedMockHttpCallBuilder CreateMock(Uri baseUri)
        {
            var builder = CreateMock().WithBaseUri(baseUri);

            return (QueuedMockHttpCallBuilder)builder;
        }

        public QueuedMockHttpCallBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responses.Add(responseFactory);

            return this;
        }

        public QueuedMockHttpCallBuilder WithResponse(HttpResponseMessage response)
        {
            return WithResponse(r => response);
        }

        public QueuedMockHttpCallBuilder WithResponse(ResponseInfo response)
        {
            return WithResponse(r => response.ToHttpResponseMessage());
        }

        public QueuedMockHttpCallBuilder VerifyOnSending(Action<HttpSendingContext> handler)
        {
            var assert = new AssertAction<HttpSendingContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnSending(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public QueuedMockHttpCallBuilder VerifyOnSent(Action<HttpSentContext> handler)
        {
            var assert = new AssertAction<HttpSentContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnSent(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public QueuedMockHttpCallBuilder VerifyOnException(Action<HttpExceptionContext> handler)
        {
            var assert = new AssertAction<HttpExceptionContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnException(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public QueuedMockHttpCallBuilder WithAssertFailure(Action failureAction)
        {
            _assertFailure = failureAction;

            return this;
        }

        #region IMockHttpCallBuilder

        IMockHttpCallBuilder IHttpMocker<IMockHttpCallBuilder>.WithResponse(ResponseInfo response)
        {
            return WithResponse(response);
        }

        IMockHttpCallBuilder IHttpMocker<IMockHttpCallBuilder>.WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            return WithResponse(responseFactory);
        }

        IMockHttpCallBuilder IHttpMocker<IMockHttpCallBuilder>.WithResponse(HttpResponseMessage response)
        {
            return WithResponse(response);
        }

        IMockHttpCallBuilder IMockHttpCallBuilder<IMockHttpCallBuilder>.VerifyOnSending(Action<HttpSendingContext> handler)
        {
            return VerifyOnSending(handler);
        }

        IMockHttpCallBuilder IMockHttpCallBuilder<IMockHttpCallBuilder>.VerifyOnSent(Action<HttpSentContext> handler)
        {
            return VerifyOnSent(handler);
        }

        IMockHttpCallBuilder IMockHttpCallBuilder<IMockHttpCallBuilder>.VerifyOnException(Action<HttpExceptionContext> handler)
        {
            return VerifyOnException(handler);
        }

        IMockHttpCallBuilder IMockHttpCallBuilder<IMockHttpCallBuilder>.WithAssertFailure(Action failureAction)
        {
            return WithAssertFailure(failureAction);
        }

        #endregion
    }
}