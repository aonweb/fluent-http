using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class QueuedMockTypedHttpCallBuilder :
        TypedHttpCallBuilder,
        IMockTypedBuilder<QueuedMockTypedHttpCallBuilder>,
        IMockTypedHttpCallBuilder
    {
        private readonly QueuedMockFormatter _formatter;
        private readonly QueuedMockHttpCallBuilder _innerBuilder;
        private readonly IList<IAssertAction> _asserts;
        private Action _assertFailure;

        protected internal QueuedMockTypedHttpCallBuilder()
        {
            _innerBuilder = new QueuedMockHttpCallBuilder().ConfigureMock();
            _formatter = new QueuedMockFormatter();
            _asserts = new List<IAssertAction>();
            _assertFailure = (() => { throw new Exception("assertion was never called"); });
        }

        public static QueuedMockTypedHttpCallBuilder CreateMock()
        {
            return new QueuedMockTypedHttpCallBuilder();
        }

        public static QueuedMockTypedHttpCallBuilder CreateMock(string baseUri)
        {
            return (QueuedMockTypedHttpCallBuilder)(CreateMock().WithBaseUri(baseUri));
        }

        public static QueuedMockTypedHttpCallBuilder CreateMock(Uri baseUri)
        {
            return (QueuedMockTypedHttpCallBuilder)(CreateMock().WithBaseUri(baseUri));
        }

        public QueuedMockTypedHttpCallBuilder WithResult<TResult>(Func<HttpResponseMessage, TypedHttpCallContext, TResult> resultFactory)
        {
            _formatter.WithResult(resultFactory);

            return this;
        }

        public QueuedMockTypedHttpCallBuilder WithResult<TResult>(TResult result)
        {
            return WithResult(result, HttpStatusCode.OK);
        }

        public QueuedMockTypedHttpCallBuilder WithResult<TResult>(TResult result, HttpStatusCode statusCode)
        {
            _formatter.WithResult((r, c) => result);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public QueuedMockTypedHttpCallBuilder WithError<TError>(Func<HttpResponseMessage, TypedHttpCallContext, TError> errorFactory)
        {
            _formatter.WithError(errorFactory);

            return this;
        }

        public QueuedMockTypedHttpCallBuilder WithError<TError>(TError error)
        {
            return WithError(error, HttpStatusCode.InternalServerError);
        }

        public QueuedMockTypedHttpCallBuilder WithError<TError>(TError error, HttpStatusCode statusCode)
        {
            _formatter.WithError((r, c) => error);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public QueuedMockTypedHttpCallBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _innerBuilder.WithResponse(responseFactory);

            return this;
        }

        public QueuedMockTypedHttpCallBuilder WithResponse(HttpResponseMessage response)
        {
            return WithResponse(r => response);
        }

        public QueuedMockTypedHttpCallBuilder WithResponse(ResponseInfo response)
        {
            return WithResponse(r => response.ToHttpResponseMessage());
        }

        public QueuedMockTypedHttpCallBuilder VerifyOnSending(Action<TypedHttpSendingContext<object, object>> handler)
        {
            return VerifyOnSending<object, object>(handler);
        }

        public QueuedMockTypedHttpCallBuilder VerifyOnSending<TResult, TContent>(Action<TypedHttpSendingContext<TResult, TContent>> handler)
        {
            var assert = new AssertAction<TypedHttpSendingContext<TResult, TContent>>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnSending<TResult, TContent>(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public QueuedMockTypedHttpCallBuilder VerifyOnSendingWithContent<TContent>(Action<TypedHttpSendingContext<object, TContent>> handler)
        {
            return VerifyOnSending(handler);
        }

        public QueuedMockTypedHttpCallBuilder VerifyOnSendingWithResult<TResult>(Action<TypedHttpSendingContext<TResult, object>> handler)
        {
            return VerifyOnSending(handler);
        }

        public QueuedMockTypedHttpCallBuilder VerifyOnSent(Action<TypedHttpSentContext<object>> handler)
        {
            return VerifyOnSent<object>(handler);
        }

        public QueuedMockTypedHttpCallBuilder VerifyOnSent<TResult>(Action<TypedHttpSentContext<TResult>> handler)
        {
            var assert = new AssertAction<TypedHttpSentContext<TResult>>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnSent<TResult>(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public QueuedMockTypedHttpCallBuilder VerifyOnResult(Action<TypedHttpResultContext<object>> handler)
        {
            return VerifyOnResult<object>(handler);
        }

        public QueuedMockTypedHttpCallBuilder VerifyOnResult<TResult>(Action<TypedHttpResultContext<TResult>> handler)
        {
            var assert = new AssertAction<TypedHttpResultContext<TResult>>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnResult<TResult>(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public QueuedMockTypedHttpCallBuilder VerifyOnError(Action<TypedHttpCallErrorContext<object>> handler)
        {
            return VerifyOnError<object>(handler);
        }

        public QueuedMockTypedHttpCallBuilder VerifyOnError<TError>(Action<TypedHttpCallErrorContext<TError>> handler)
        {
            var assert = new AssertAction<TypedHttpCallErrorContext<TError>>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnError<TError>(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public QueuedMockTypedHttpCallBuilder VerifyOnException(Action<TypedHttpCallExceptionContext> handler)
        {
            var assert = new AssertAction<TypedHttpCallExceptionContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnException(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public QueuedMockTypedHttpCallBuilder WithAssertFailure(Action failureAction)
        {
            _assertFailure = failureAction;

            return this;
        }

        public override async Task<TResult> ResultAsync<TResult>()
        {
            try
            {
                return await base.ResultAsync<TResult>();
            }
            finally
            {
                Verify();
            }
        }

        private void Verify()
        {
            foreach (var assert in _asserts)
                assert.DoAssert();
        }

        #region IMockTypedHttpCallBuilder

        IMockTypedHttpCallBuilder IHttpTypedMocker<IMockTypedHttpCallBuilder>.WithError<TError>(TError error)
        {
            return WithError(error);
        }

        IMockTypedHttpCallBuilder IHttpTypedMocker<IMockTypedHttpCallBuilder>.WithResult<TResult>(Func<HttpResponseMessage, TypedHttpCallContext, TResult> resultFactory)
        {
            return WithResult(resultFactory);
        }

        IMockTypedHttpCallBuilder IHttpTypedMocker<IMockTypedHttpCallBuilder>.WithError<TError>(Func<HttpResponseMessage, TypedHttpCallContext, TError> errorFactory)
        {
            return WithError(errorFactory);
        }


        IMockTypedHttpCallBuilder IMockTypedBuilder<IMockTypedHttpCallBuilder>.WithError<TError>(TError error, HttpStatusCode statusCode)
        {
            return WithError(error, statusCode);
        }

        IMockTypedHttpCallBuilder IHttpTypedMocker<IMockTypedHttpCallBuilder>.WithResult<TResult>(TResult result)
        {
            return WithResult(result);
        }

        IMockTypedHttpCallBuilder IMockTypedBuilder<IMockTypedHttpCallBuilder>.WithResult<TResult>(TResult result, HttpStatusCode statusCode)
        {
            return WithResult(result, statusCode);
        }

        IMockTypedHttpCallBuilder IHttpMocker<IMockTypedHttpCallBuilder>.WithResponse(ResponseInfo response)
        {
            return WithResponse(response);
        }

        IMockTypedHttpCallBuilder IHttpMocker<IMockTypedHttpCallBuilder>.WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            return WithResponse(responseFactory);
        }

        IMockTypedHttpCallBuilder IHttpMocker<IMockTypedHttpCallBuilder>.WithResponse(HttpResponseMessage response)
        {
            return WithResponse(response);
        }

        IMockTypedHttpCallBuilder IMockTypedHttpCallBuilder<IMockTypedHttpCallBuilder>.VerifyOnSending(Action<TypedHttpSendingContext<object, object>> handler)
        {
            return VerifyOnSending(handler);
        }

        IMockTypedHttpCallBuilder IMockTypedHttpCallBuilder<IMockTypedHttpCallBuilder>.VerifyOnSending<TResult, TContent>(Action<TypedHttpSendingContext<TResult, TContent>> handler)
        {
            return VerifyOnSending(handler);
        }

        IMockTypedHttpCallBuilder IMockTypedHttpCallBuilder<IMockTypedHttpCallBuilder>.VerifyOnSendingWithContent<TContent>(Action<TypedHttpSendingContext<object, TContent>> handler)
        {
            return VerifyOnSendingWithContent(handler);
        }

        IMockTypedHttpCallBuilder IMockTypedHttpCallBuilder<IMockTypedHttpCallBuilder>.VerifyOnSendingWithResult<TResult>(Action<TypedHttpSendingContext<TResult, object>> handler)
        {
            return VerifyOnSendingWithResult(handler);
        }

        IMockTypedHttpCallBuilder IMockTypedHttpCallBuilder<IMockTypedHttpCallBuilder>.VerifyOnSent(Action<TypedHttpSentContext<object>> handler)
        {
            return VerifyOnSent(handler);
        }

        IMockTypedHttpCallBuilder IMockTypedHttpCallBuilder<IMockTypedHttpCallBuilder>.VerifyOnSent<TResult>(Action<TypedHttpSentContext<TResult>> handler)
        {
            return VerifyOnSent(handler);
        }

        IMockTypedHttpCallBuilder IMockTypedHttpCallBuilder<IMockTypedHttpCallBuilder>.VerifyOnResult(Action<TypedHttpResultContext<object>> handler)
        {
            return VerifyOnResult(handler);
        }

        IMockTypedHttpCallBuilder IMockTypedHttpCallBuilder<IMockTypedHttpCallBuilder>.VerifyOnResult<TResult>(Action<TypedHttpResultContext<TResult>> handler)
        {
            return VerifyOnResult(handler);
        }

        IMockTypedHttpCallBuilder IMockTypedHttpCallBuilder<IMockTypedHttpCallBuilder>.VerifyOnError(Action<TypedHttpCallErrorContext<object>> handler)
        {
            return VerifyOnError(handler);
        }

        IMockTypedHttpCallBuilder IMockTypedHttpCallBuilder<IMockTypedHttpCallBuilder>.VerifyOnError<TError>(Action<TypedHttpCallErrorContext<TError>> handler)
        {
            return VerifyOnError(handler);
        }

        IMockTypedHttpCallBuilder IMockTypedHttpCallBuilder<IMockTypedHttpCallBuilder>.VerifyOnException(Action<TypedHttpCallExceptionContext> handler)
        {
            return VerifyOnException(handler);
        }

        IMockTypedHttpCallBuilder IMockTypedHttpCallBuilder<IMockTypedHttpCallBuilder>.WithAssertFailure(Action failureAction)
        {
            return WithAssertFailure(failureAction);
        }

        #endregion
    }
}