using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockTypedHttpCallBuilder : TypedHttpCallBuilder, IMockTypedHttpCallBuilder
    {
        private readonly IMockFormatter _formatter;
        private readonly IMockHttpCallBuilder _innerBuilder;
        private readonly IList<IAssertAction> _asserts;
        private Action _assertFailure;

        public MockTypedHttpCallBuilder()
            : this(new TypedHttpCallBuilderSettings())
        { }

        public MockTypedHttpCallBuilder(TypedHttpCallBuilderSettings settings)
            : this(settings, new MockHttpCallBuilder(), new MockFormatter())
        { }

        protected MockTypedHttpCallBuilder(IMockHttpCallBuilder builder, IMockFormatter formatter)
            : this(new TypedHttpCallBuilderSettings(), builder, formatter)
        { }

        protected MockTypedHttpCallBuilder(TypedHttpCallBuilderSettings settings, IMockHttpCallBuilder builder, IMockFormatter formatter)
            : base(settings, builder, formatter)
        {
            _innerBuilder = builder;
            _formatter = formatter;
            _asserts = new List<IAssertAction>();
            _assertFailure = (() => { throw new Exception("assertion was never called"); });
        }

        public IMockTypedHttpCallBuilder WithResult<TResult>(Func<HttpResponseMessage, TypedHttpCallContext, TResult> resultFactory)
        {
            return WithResult(resultFactory, new ResponseInfo(HttpStatusCode.OK));
        }

        public IMockTypedHttpCallBuilder WithResult<TResult>(Func<HttpResponseMessage, TypedHttpCallContext, TResult> resultFactory, ResponseInfo response)
        {
            _formatter.WithResult(resultFactory);

            return this.WithResponse(response);
        }

        public IMockTypedHttpCallBuilder WithError<TError>(Func<HttpResponseMessage, TypedHttpCallContext, TError> errorFactory)
        {
            return WithError(errorFactory, new ResponseInfo(HttpStatusCode.InternalServerError));
        }

        public IMockTypedHttpCallBuilder WithError<TError>(Func<HttpResponseMessage, TypedHttpCallContext, TError> errorFactory, ResponseInfo response)
        {
            _formatter.WithError(errorFactory);

            return this.WithResponse(response);
        }

        public IMockTypedHttpCallBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _innerBuilder.WithResponse(responseFactory);

            return this;
        }

        public IMockTypedHttpCallBuilder VerifyOnSending(Action<TypedHttpSendingContext<object, object>> handler)
        {
            return VerifyOnSending<object, object>(handler);
        }

        public IMockTypedHttpCallBuilder VerifyOnSending<TResult, TContent>(Action<TypedHttpSendingContext<TResult, TContent>> handler)
        {
            var assert = new AssertAction<TypedHttpSendingContext<TResult, TContent>>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnSending<TResult, TContent>(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public IMockTypedHttpCallBuilder VerifyOnSendingWithContent<TContent>(Action<TypedHttpSendingContext<object, TContent>> handler)
        {
            return VerifyOnSending(handler);
        }

        public IMockTypedHttpCallBuilder VerifyOnSendingWithResult<TResult>(Action<TypedHttpSendingContext<TResult, object>> handler)
        {
            return VerifyOnSending(handler);
        }

        public IMockTypedHttpCallBuilder VerifyOnSent(Action<TypedHttpSentContext<object>> handler)
        {
            return VerifyOnSent<object>(handler);
        }

        public IMockTypedHttpCallBuilder VerifyOnSent<TResult>(Action<TypedHttpSentContext<TResult>> handler)
        {
            var assert = new AssertAction<TypedHttpSentContext<TResult>>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnSent<TResult>(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public IMockTypedHttpCallBuilder VerifyOnResult(Action<TypedHttpResultContext<object>> handler)
        {
            return VerifyOnResult<object>(handler);
        }

        public IMockTypedHttpCallBuilder VerifyOnResult<TResult>(Action<TypedHttpResultContext<TResult>> handler)
        {
            var assert = new AssertAction<TypedHttpResultContext<TResult>>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnResult<TResult>(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public IMockTypedHttpCallBuilder VerifyOnError(Action<TypedHttpErrorContext<object>> handler)
        {
            return VerifyOnError<object>(handler);
        }

        public IMockTypedHttpCallBuilder VerifyOnError<TError>(Action<TypedHttpErrorContext<TError>> handler)
        {
            var assert = new AssertAction<TypedHttpErrorContext<TError>>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnError<TError>(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public IMockTypedHttpCallBuilder VerifyOnException(Action<TypedHttpExceptionContext> handler)
        {
            var assert = new AssertAction<TypedHttpExceptionContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnException(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public IMockTypedHttpCallBuilder WithAssertFailure(Action failureAction)
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