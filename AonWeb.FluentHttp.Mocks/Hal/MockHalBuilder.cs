using System;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Serialization;

namespace AonWeb.FluentHttp.Mocks.Hal
{
    public class MockHalBuilder: HalBuilder, IMockHalBuilder
    {
        private readonly IMockTypedBuilder _innerBuilder;

        public MockHalBuilder(IMockTypedBuilder innerBuilder) : base(innerBuilder)
        {
            _innerBuilder = innerBuilder;
        }

        public IMockHalBuilder WithResponse(Predicate<IMockRequestContext> predicate, Func<IMockRequestContext, IMockResponse> responseFactory)
        {
            _innerBuilder.WithResponse(predicate, responseFactory);

            return this;
        }

        public IMockHalBuilder WithResult<TResult>(Predicate<IMockTypedRequestContext> resultPredicate, Func<IMockTypedRequestContext, IMockResult<TResult>> resultFactory, Predicate<IMockRequestContext> responsePredicate, Func<IMockRequestContext, IMockResponse> responseFactory)
        {
            _innerBuilder.WithResult(resultPredicate, resultFactory, responsePredicate, responseFactory);

            return this;
        }

        public IMockHalBuilder WithError<TError>(Predicate<IMockTypedRequestContext> errorPredicate, Func<IMockTypedRequestContext, IMockResult<TError>> errorFactory, Predicate<IMockRequestContext> responsePredicate, Func<IMockRequestContext, IMockResponse> responseFactory)
        {
            _innerBuilder.WithError(errorPredicate, errorFactory, responsePredicate, responseFactory);

            return this;
        }

        public IMockHalBuilder VerifyOnSending(Action<TypedSendingContext<IHalResource, IHalRequest>> handler)
        {
            _innerBuilder.VerifyOnSending(handler);

            return this;
        }

        public IMockHalBuilder VerifyOnSending<TResult, TContent>(Action<TypedSendingContext<TResult, TContent>> handler) where TResult : IHalResource where TContent : IHalRequest
        {
            _innerBuilder.VerifyOnSending(handler);

            return this;
        }

        public IMockHalBuilder VerifyOnSendingWithContent<TContent>(Action<TypedSendingContext<IHalResource, TContent>> handler) where TContent : IHalRequest
        {
            return VerifyOnSending(handler);
        }

        public IMockHalBuilder VerifyOnSendingWithResult<TResult>(Action<TypedSendingContext<TResult, IHalRequest>> handler) where TResult : IHalResource
        {
            return VerifyOnSending(handler);
        }

        public IMockHalBuilder VerifyOnSent(Action<TypedSentContext<IHalResource>> handler)
        {
            _innerBuilder.VerifyOnSent(handler);

            return this;
        }

        public IMockHalBuilder VerifyOnSent<TResult>(Action<TypedSentContext<TResult>> handler) where TResult : IHalResource
        {
            _innerBuilder.VerifyOnSent(handler);

            return this;
        }

        public IMockHalBuilder VerifyOnResult(Action<TypedResultContext<IHalResource>> handler)
        {
            _innerBuilder.VerifyOnResult(handler);

            return this;
        }

        public IMockHalBuilder VerifyOnResult<TResult>(Action<TypedResultContext<TResult>> handler) where TResult : IHalResource
        {
            _innerBuilder.VerifyOnResult(handler);

            return this;
        }

        public IMockHalBuilder VerifyOnError(Action<TypedErrorContext<object>> handler)
        {
            _innerBuilder.VerifyOnError(handler);

            return this;
        }

        public IMockHalBuilder VerifyOnError<TError>(Action<TypedErrorContext<TError>> handler)
        {
            _innerBuilder.VerifyOnError(handler);

            return this;
        }

        public IMockHalBuilder VerifyOnException(Action<TypedExceptionContext> handler)
        {
            _innerBuilder.VerifyOnException(handler);

            return this;
        }

        public IMockHalBuilder WithAssertFailure(Action failureAction)
        {
            _innerBuilder.WithAssertFailure(failureAction);

            return this;
        }
    }
}