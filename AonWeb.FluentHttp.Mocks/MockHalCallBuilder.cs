using System;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHalCallBuilder :  HalCallBuilder,  IMockHalCallBuilder
    {
        private readonly IMockTypedHttpCallBuilder _innerBuilder;

        public MockHalCallBuilder()
            : this(new MockTypedHttpCallBuilder())
        { }

        protected MockHalCallBuilder(IMockTypedHttpCallBuilder builder)
            : base(builder)
        {
            _innerBuilder = builder;
        }

        public IMockHalCallBuilder WithResult<TResult>(Func<HttpResponseMessage, TypedHttpCallContext, TResult> resultFactory)
        {
            _innerBuilder.WithResult(resultFactory);

            return this;
        }

        public IMockHalCallBuilder WithResult<TResult>(TResult result) 
        {
            _innerBuilder.WithResult(result);

            return this;
        }

        public IMockHalCallBuilder WithResult<TResult>(TResult result, HttpStatusCode statusCode)
        {
            _innerBuilder.WithResult(result, statusCode);

            return this;
        }

        public IMockHalCallBuilder WithResult<TResult>(Func<HttpResponseMessage, TypedHttpCallContext, TResult> resultFactory, ResponseInfo response)
        {
            _innerBuilder.WithResult(resultFactory, response);

            return this;
        }

        public IMockHalCallBuilder WithError<TError>(Func<HttpResponseMessage, TypedHttpCallContext, TError> errorFactory)
        {
            _innerBuilder.WithError(errorFactory);

            return this;
        }

        public IMockHalCallBuilder WithError<TError>(TError error)
        {
            _innerBuilder.WithError(error);

            return this;
        }

        public IMockHalCallBuilder WithError<TError>(TError error, HttpStatusCode statusCode)
        {
            _innerBuilder.WithError(error, statusCode);

            return this;
        }

        public IMockHalCallBuilder WithError<TError>(Func<HttpResponseMessage, TypedHttpCallContext, TError> errorFactory, ResponseInfo response)
        {
            _innerBuilder.WithError(errorFactory, response);

            return this;
        }

        public IMockHalCallBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _innerBuilder.WithResponse(responseFactory);

            return this;
        }

        public IMockHalCallBuilder WithResponse(HttpResponseMessage response)
        {
            _innerBuilder.WithResponse(response);

            return this;
        }

        public IMockHalCallBuilder WithResponse(ResponseInfo response)
        {
            _innerBuilder.WithResponse(response);

            return this;
        }

        public IMockHalCallBuilder VerifyOnSending(Action<TypedHttpSendingContext<IHalResource, IHalRequest>> handler)
        {
            _innerBuilder.VerifyOnSending(handler);

            return this;
        }

        public IMockHalCallBuilder VerifyOnSending<TResult, TContent>(Action<TypedHttpSendingContext<TResult, TContent>> handler)
            where TResult : IHalResource
            where TContent : IHalRequest
        {
            _innerBuilder.VerifyOnSending(handler);

            return this;
        }

        public IMockHalCallBuilder VerifyOnSendingWithContent<TContent>(Action<TypedHttpSendingContext<IHalResource, TContent>> handler)
            where TContent : IHalRequest
        {
            return VerifyOnSending(handler);
        }

        public IMockHalCallBuilder VerifyOnSendingWithResult<TResult>(Action<TypedHttpSendingContext<TResult, IHalRequest>> handler)
            where TResult : IHalResource
        {
            return VerifyOnSending(handler);
        }

        public IMockHalCallBuilder VerifyOnSent(Action<TypedHttpSentContext<IHalResource>> handler)
        {
            _innerBuilder.VerifyOnSent(handler);

            return this;
        }

        public IMockHalCallBuilder VerifyOnSent<TResult>(Action<TypedHttpSentContext<TResult>> handler)
            where TResult : IHalResource
        {
            _innerBuilder.VerifyOnSent(handler);

            return this;
        }

        public IMockHalCallBuilder VerifyOnResult(Action<TypedHttpResultContext<IHalResource>> handler)
        {
            _innerBuilder.VerifyOnResult(handler);

            return this;
        }

        public IMockHalCallBuilder VerifyOnResult<TResult>(Action<TypedHttpResultContext<TResult>> handler)
            where TResult : IHalResource
        {
            _innerBuilder.VerifyOnResult(handler);

            return this;
        }

        public IMockHalCallBuilder VerifyOnError(Action<TypedHttpCallErrorContext<object>> handler)
        {
            _innerBuilder.VerifyOnError(handler);

            return this;
        }

        public IMockHalCallBuilder VerifyOnError<TError>(Action<TypedHttpCallErrorContext<TError>> handler)
        {
            _innerBuilder.VerifyOnError(handler);

            return this;
        }

        public IMockHalCallBuilder VerifyOnException(Action<TypedHttpCallExceptionContext> handler)
        {
            _innerBuilder.VerifyOnException(handler);

            return this;
        }

        public IMockHalCallBuilder WithAssertFailure(Action failureAction)
        {
            _innerBuilder.WithAssertFailure(failureAction);

            return this;
        }
    }
}
