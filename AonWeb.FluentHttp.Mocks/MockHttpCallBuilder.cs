using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpCallBuilder : HttpCallBuilder, IMockHttpCallBuilder
    {
        private readonly IList<IAssertAction> _asserts;
        private Action _assertFailure;

        public MockHttpCallBuilder()
            : this(new MockHttpClientBuilder()) { }

        protected MockHttpCallBuilder(IMockHttpClientBuilder clientBuilder)
            : base(clientBuilder)
        {
            _asserts = new List<IAssertAction>();
            _assertFailure = (() => { throw new Exception("assertion was never called"); });

            ConfigureMock();
        }

        public IMockHttpCallBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            ConfigureClient(b => ((MockHttpClientBuilder)b).WithResponse(responseFactory));

            return this;
        }

        public IMockHttpCallBuilder VerifyOnSending(Action<HttpSendingContext> handler)
        {
            var assert = new AssertAction<HttpSendingContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnSending(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public IMockHttpCallBuilder VerifyOnSent(Action<HttpSentContext> handler)
        {
            var assert = new AssertAction<HttpSentContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnSent(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public IMockHttpCallBuilder VerifyOnException(Action<HttpExceptionContext> handler)
        {
            var assert = new AssertAction<HttpExceptionContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            OnException(HttpCallHandlerPriority.Last, assert);

            return this;
        }

        public IMockHttpCallBuilder WithAssertFailure(Action failureAction)
        {
            _assertFailure = failureAction;

            return this;
        }

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

        private void ConfigureMock()
        {
            OnSending(HttpCallHandlerPriority.Last, context => context.Items["MockRequest"] = context.Request);
            OnSent(HttpCallHandlerPriority.First, context => context.Result.RequestMessage = context.Items["MockRequest"] as HttpRequestMessage);
        }
    }
}