using System;
using System.Net;
using System.Net.Http;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks 
{
    public class QueuedMockHalCallBuilder :
        HalCallBuilder,
        IMockHalCallBuilder<QueuedMockHalCallBuilder>,
        IMockHalCallBuilder 
    {
        private readonly QueuedMockTypedHttpCallBuilder _innerBuilder;

        protected QueuedMockHalCallBuilder()
        {
            _innerBuilder = new QueuedMockTypedHttpCallBuilder();
        }

        public static QueuedMockHalCallBuilder CreateMock()
        {
            return new QueuedMockHalCallBuilder();
        }

        public QueuedMockHalCallBuilder WithResult<TResult>(Func<HttpResponseMessage, TypedHttpCallContext, TResult> resultFactory)
        {
            _innerBuilder.WithResult(resultFactory);

            return this;
        }

        public QueuedMockHalCallBuilder WithResult<TResult>(TResult result)
        {
            return WithResult(result, HttpStatusCode.OK);
        }

        public QueuedMockHalCallBuilder WithResult<TResult>(TResult result, HttpStatusCode statusCode)
        {
            _innerBuilder.WithResult((r, c) => result);

            return this;
        }

        public QueuedMockHalCallBuilder WithError<TError>(Func<HttpResponseMessage, TypedHttpCallContext, TError> errorFactory)
        {
            _innerBuilder.WithError(errorFactory);

            return this;
        }

        public QueuedMockHalCallBuilder WithError<TError>(TError error)
        {
            return WithError(error, HttpStatusCode.InternalServerError);
        }

        public QueuedMockHalCallBuilder WithError<TError>(TError error, HttpStatusCode statusCode)
        {
            _innerBuilder.WithError((r, c) => error);

            return this;
        }

        public QueuedMockHalCallBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _innerBuilder.WithResponse(responseFactory);

            return this;
        }

        public QueuedMockHalCallBuilder WithResponse(HttpResponseMessage response)
        {
            return WithResponse(r => response);
        }

        public QueuedMockHalCallBuilder WithResponse(ResponseInfo response)
        {
            return WithResponse(r => response.ToHttpResponseMessage());
        }

        public QueuedMockHalCallBuilder VerifyOnSending(Action<TypedHttpSendingContext<IHalResource, IHalRequest>> handler)
        {
            _innerBuilder.VerifyOnSending(handler);

            return this;
        }

        public QueuedMockHalCallBuilder VerifyOnSending<TResult, TContent>(Action<TypedHttpSendingContext<TResult, TContent>> handler) 
            where TResult : IHalResource 
            where TContent : IHalRequest
        {
            _innerBuilder.VerifyOnSending(handler);

            return this;
        }

        public QueuedMockHalCallBuilder VerifyOnSendingWithContent<TContent>(Action<TypedHttpSendingContext<IHalResource, TContent>> handler) 
            where TContent : IHalRequest
        {
            return VerifyOnSending(handler);
        }

        public QueuedMockHalCallBuilder VerifyOnSendingWithResult<TResult>(Action<TypedHttpSendingContext<TResult, IHalRequest>> handler) 
            where TResult : IHalResource
        {
            return VerifyOnSending(handler);
        }

        public QueuedMockHalCallBuilder VerifyOnSent(Action<TypedHttpSentContext<IHalResource>> handler)
        {
            _innerBuilder.VerifyOnSent(handler);

            return this;
        }

        public QueuedMockHalCallBuilder VerifyOnSent<TResult>(Action<TypedHttpSentContext<TResult>> handler) 
            where TResult : IHalResource
        {
            _innerBuilder.VerifyOnSent(handler);

            return this;
        }

        public QueuedMockHalCallBuilder VerifyOnResult(Action<TypedHttpResultContext<IHalResource>> handler)
        {
            _innerBuilder.VerifyOnResult(handler);

            return this;
        }

        public QueuedMockHalCallBuilder VerifyOnResult<TResult>(Action<TypedHttpResultContext<TResult>> handler) 
            where TResult : IHalResource
        {
            _innerBuilder.VerifyOnResult(handler);

            return this;
        }

        public QueuedMockHalCallBuilder VerifyOnError(Action<TypedHttpCallErrorContext<object>> handler)
        {
            _innerBuilder.VerifyOnError(handler);

            return this;
        }

        public QueuedMockHalCallBuilder VerifyOnError<TError>(Action<TypedHttpCallErrorContext<TError>> handler)
        {
            _innerBuilder.VerifyOnError(handler);

            return this;
        }

        public QueuedMockHalCallBuilder VerifyOnException(Action<TypedHttpCallExceptionContext> handler)
        {
            _innerBuilder.VerifyOnException(handler);

            return this;
        }

        public QueuedMockHalCallBuilder WithAssertFailure(Action failureAction)
        {
            _innerBuilder.WithAssertFailure(failureAction);

            return this;
        }

        #region IMockHalCallBuilder

        IMockHalCallBuilder IHttpTypedMocker<IMockHalCallBuilder>.WithError<TError>(TError error)
        {
            return WithError(error);
        }

        IMockHalCallBuilder IHttpTypedMocker<IMockHalCallBuilder>.WithResult<TResult>(Func<HttpResponseMessage, TypedHttpCallContext, TResult> resultFactory)
        {
            return WithResult(resultFactory);
        }

        IMockHalCallBuilder IHttpTypedMocker<IMockHalCallBuilder>.WithError<TError>(Func<HttpResponseMessage, TypedHttpCallContext, TError> errorFactory)
        {
            return WithError(errorFactory);
        }


        IMockHalCallBuilder IMockTypedBuilder<IMockHalCallBuilder>.WithError<TError>(TError error, HttpStatusCode statusCode)
        {
            return WithError(error, statusCode);
        }

        IMockHalCallBuilder IHttpTypedMocker<IMockHalCallBuilder>.WithResult<TResult>(TResult result)
        {
            return WithResult(result);
        }

        IMockHalCallBuilder IMockTypedBuilder<IMockHalCallBuilder>.WithResult<TResult>(TResult result, HttpStatusCode statusCode)
        {
            return WithResult(result, statusCode);
        }

        IMockHalCallBuilder IHttpMocker<IMockHalCallBuilder>.WithResponse(ResponseInfo response)
        {
            return WithResponse(response);
        }

        IMockHalCallBuilder IHttpMocker<IMockHalCallBuilder>.WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            return WithResponse(responseFactory);
        }

        IMockHalCallBuilder IHttpMocker<IMockHalCallBuilder>.WithResponse(HttpResponseMessage response)
        {
            return WithResponse(response);
        }

        IMockHalCallBuilder IMockHalCallBuilder<IMockHalCallBuilder>.VerifyOnSending(Action<TypedHttpSendingContext<IHalResource, IHalRequest>> handler)
        {
            return VerifyOnSending(handler);
        }

        IMockHalCallBuilder IMockHalCallBuilder<IMockHalCallBuilder>.VerifyOnSending<TResult, TContent>(Action<TypedHttpSendingContext<TResult, TContent>> handler)
        {
            return VerifyOnSending(handler);
        }

        IMockHalCallBuilder IMockHalCallBuilder<IMockHalCallBuilder>.VerifyOnSendingWithContent<TContent>(Action<TypedHttpSendingContext<IHalResource, TContent>> handler)
        {
            return VerifyOnSendingWithContent(handler);
        }

        IMockHalCallBuilder IMockHalCallBuilder<IMockHalCallBuilder>.VerifyOnSendingWithResult<TResult>(Action<TypedHttpSendingContext<TResult, IHalRequest>> handler)
        {
            return VerifyOnSendingWithResult(handler);
        }

        IMockHalCallBuilder IMockHalCallBuilder<IMockHalCallBuilder>.VerifyOnSent(Action<TypedHttpSentContext<IHalResource>> handler)
        {
            return VerifyOnSent(handler);
        }

        IMockHalCallBuilder IMockHalCallBuilder<IMockHalCallBuilder>.VerifyOnSent<TResult>(Action<TypedHttpSentContext<TResult>> handler)
        {
            return VerifyOnSent(handler);
        }

        IMockHalCallBuilder IMockHalCallBuilder<IMockHalCallBuilder>.VerifyOnResult(Action<TypedHttpResultContext<IHalResource>> handler)
        {
            return VerifyOnResult(handler);
        }

        IMockHalCallBuilder IMockHalCallBuilder<IMockHalCallBuilder>.VerifyOnResult<TResult>(Action<TypedHttpResultContext<TResult>> handler)
        {
            return VerifyOnResult(handler);
        }

        IMockHalCallBuilder IMockHalCallBuilder<IMockHalCallBuilder>.VerifyOnError(Action<TypedHttpCallErrorContext<object>> handler)
        {
            return VerifyOnError(handler);
        }

        IMockHalCallBuilder IMockHalCallBuilder<IMockHalCallBuilder>.VerifyOnError<TError>(Action<TypedHttpCallErrorContext<TError>> handler)
        {
            return VerifyOnError(handler);
        }

        IMockHalCallBuilder IMockHalCallBuilder<IMockHalCallBuilder>.VerifyOnException(Action<TypedHttpCallExceptionContext> handler)
        {
            return VerifyOnException(handler);
        }

        IMockHalCallBuilder IMockHalCallBuilder<IMockHalCallBuilder>.WithAssertFailure(Action failureAction)
        {
            return WithAssertFailure(failureAction);
        }

        #endregion
    }
}