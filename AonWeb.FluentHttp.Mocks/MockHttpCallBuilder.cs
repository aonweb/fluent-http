using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpCallBuilder : HttpCallBuilder, IMockBuilder<MockHttpCallBuilder>, IMockHttpCallBuilder
    {
        private readonly IList<IAssertAction> _asserts;
        private Action _assertFailure;

        public MockHttpCallBuilder()
            : base(new MockHttpClientBuilder())
        {
            _asserts = new List<IAssertAction>();
            _assertFailure = (() => { throw new Exception("assertion was never called"); });
        }

        public static MockHttpCallBuilder CreateMock()
        {
            return new MockHttpCallBuilder().ConfigureMock();
        }

        public static MockHttpCallBuilder CreateMock(string baseUri)
        {
            var builder = CreateMock().WithBaseUri(baseUri);

            return (MockHttpCallBuilder)builder;
        }

        public static MockHttpCallBuilder CreateMock(Uri baseUri)
        {
            var builder = CreateMock().WithBaseUri(baseUri);
                
            return (MockHttpCallBuilder)builder;
        }

        public MockHttpCallBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            ConfigureClient(b => ((MockHttpClientBuilder)b).WithResponse(responseFactory));

            return this;
        }

        public MockHttpCallBuilder WithResponse(HttpResponseMessage response)
        {
            return WithResponse(r => response);
        }

        public MockHttpCallBuilder WithResponse(ResponseInfo response)
        {
            return WithResponse(r => response.ToHttpResponseMessage());
        }

        public MockHttpCallBuilder VerifyOnSending(Action<HttpSendingContext> handler)
        {
            var assert = new AssertAction<HttpSendingContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnSending(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public MockHttpCallBuilder VerifyOnSent(Action<HttpSentContext> handler)
        {
            var assert = new AssertAction<HttpSentContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnSent(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public MockHttpCallBuilder VerifyOnException(Action<HttpExceptionContext> handler)
        {
            var assert = new AssertAction<HttpExceptionContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnException(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public MockHttpCallBuilder WithAssertFailure(Action failureAction)
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

        public override async Task<HttpResponseMessage> ResultAsync()
        {
            try
            {
                return await base.ResultAsync();
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

    public interface IAssertAction {
        void DoAssert();
    }

    public class AssertAction<T> : IAssertAction {
        private bool _called;

        private readonly Action<T> _innerAction;
        private readonly Func<Action> _failurefactory;

        public AssertAction(Action<T> action, Func<Action> failurefactory)
        {
            _innerAction = action;
            _failurefactory = failurefactory;
        }

        public static implicit operator Action<T>(AssertAction<T> a)
        {
            if (!a._called)
                return obj =>
                {
                    a._called = true;
                    a._innerAction(obj);
                };

            return obj => { };
        }

        public void DoAssert()
        {
            if (!_called)
            {
                var failureAction = _failurefactory();

                failureAction();
            }
                
        }
    }
}