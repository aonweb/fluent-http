using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockTypedBuilder : TypedBuilder, IMockTypedBuilder
    {
        private readonly IMockFormatter _formatter;
        private readonly IMockHttpBuilder _innerBuilder;
        private readonly IList<IAssertAction> _asserts;
        private Action _assertFailure;

        public MockTypedBuilder(ITypedBuilderSettings settings, IMockHttpBuilder builder, IMockFormatter formatter, IReadOnlyCollection<ITypedHandler> defaultHandlers) 
            : base(settings, builder, formatter, defaultHandlers)
        {
            _innerBuilder = builder;
            _formatter = formatter;
            _asserts = new List<IAssertAction>();
            _assertFailure = (() => { throw new Exception("assertion was never called"); });
        }

        public IMockTypedBuilder WithResponse(Predicate<IMockRequestContext> predicate, Func<IMockRequestContext, IMockResponse> responseFactory)
        {
            _innerBuilder.WithResponse(predicate, responseFactory);

            return this;
        }

        public IMockTypedBuilder WithResult<TResult>(
            Predicate<IMockTypedRequestContext> resultPredicate,
            Func<IMockTypedRequestContext, IMockResult<TResult>> resultFactory,
            Predicate<IMockRequestContext> responsePredicate,
            Func<IMockRequestContext, IMockResponse> responseFactory)
        {
            _formatter.WithResult(resultPredicate, resultFactory);
            _innerBuilder.WithResponse(responsePredicate, responseFactory);

            return this;
        }

        public IMockTypedBuilder WithError<TError>(
            Predicate<IMockTypedRequestContext> errorPredicate,
            Func<IMockTypedRequestContext, IMockResult<TError>> errorFactory,
            Predicate<IMockRequestContext> responsePredicate,
            Func<IMockRequestContext, IMockResponse> responseFactory)
        {
            _formatter.WithError(errorPredicate, errorFactory);
            _innerBuilder.WithResponse(responsePredicate, responseFactory);

            return this;
        }

        public IMockTypedBuilder VerifyOnSending(Action<TypedSendingContext<object, object>> handler)
        {
            return VerifyOnSending<object, object>(handler);
        }

        public IMockTypedBuilder VerifyOnSending<TResult, TContent>(Action<TypedSendingContext<TResult, TContent>> handler)
        {
            var assert = new AssertAction<TypedSendingContext<TResult, TContent>>(handler, () => _assertFailure);

            _asserts.Add(assert);

            this.OnSending<TResult, TContent>(HandlerPriority.Last, assert);

            return this;
        }

        public IMockTypedBuilder VerifyOnSendingWithContent<TContent>(Action<TypedSendingContext<object, TContent>> handler)
        {
            return VerifyOnSending(handler);
        }

        public IMockTypedBuilder VerifyOnSendingWithResult<TResult>(Action<TypedSendingContext<TResult, object>> handler)
        {
            return VerifyOnSending(handler);
        }

        public IMockTypedBuilder VerifyOnSent(Action<TypedSentContext<object>> handler)
        {
            return VerifyOnSent<object>(handler);
        }

        public IMockTypedBuilder VerifyOnSent<TResult>(Action<TypedSentContext<TResult>> handler)
        {
            var assert = new AssertAction<TypedSentContext<TResult>>(handler, () => _assertFailure);

            _asserts.Add(assert);

            this.OnSent<TResult>(HandlerPriority.Last, assert);

            return this;
        }

        public IMockTypedBuilder VerifyOnResult(Action<TypedResultContext<object>> handler)
        {
            return VerifyOnResult<object>(handler);
        }

        public IMockTypedBuilder VerifyOnResult<TResult>(Action<TypedResultContext<TResult>> handler)
        {
            var assert = new AssertAction<TypedResultContext<TResult>>(handler, () => _assertFailure);

            _asserts.Add(assert);

            this.OnResult<TResult>(HandlerPriority.Last, assert);

            return this;
        }

        public IMockTypedBuilder VerifyOnError(Action<TypedErrorContext<object>> handler)
        {
            return VerifyOnError<object>(handler);
        }

        public IMockTypedBuilder VerifyOnError<TError>(Action<TypedErrorContext<TError>> handler)
        {
            var assert = new AssertAction<TypedErrorContext<TError>>(handler, () => _assertFailure);

            _asserts.Add(assert);

            this.OnError<TError>(HandlerPriority.Last, assert);

            return this;
        }

        public IMockTypedBuilder VerifyOnException(Action<TypedExceptionContext> handler)
        {
            var assert = new AssertAction<TypedExceptionContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            this.OnException(HandlerPriority.Last, assert);

            return this;
        }

        public IMockTypedBuilder WithAssertFailure(Action failureAction)
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
    }
}