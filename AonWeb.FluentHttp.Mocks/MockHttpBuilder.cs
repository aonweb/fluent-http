using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpBuilder : HttpBuilder, IMockHttpBuilder
    {
        private readonly IList<IAssertAction> _asserts;
        private Action _assertFailure;
        private readonly MockHttpClientBuilder _clientBuilder;

        public MockHttpBuilder(IHttpBuilderSettings settings, MockHttpClientBuilder clientBuilder,
            IEnumerable<IHttpHandler> defaultHandlers)
            : base(settings, clientBuilder, defaultHandlers)
        {
            _clientBuilder = clientBuilder;
            _asserts = new List<IAssertAction>();
            _assertFailure = (() => { throw new Exception("assertion was never called"); });

            ConfigureMock();
        }

        public IMockHttpBuilder WithResponse(Predicate<IMockRequestContext> predicate, Func<IMockRequestContext, IMockResponse> responseFactory)
        {
            _clientBuilder.WithResponse(predicate, responseFactory);

            return this;
        }

        public IMockHttpBuilder VerifyOnSending(Action<HttpSendingContext> handler)
        {
            var assert = new AssertAction<HttpSendingContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            this.OnSending(HandlerPriority.Last, assert);

            return this;
        }

        public IMockHttpBuilder VerifyOnSent(Action<HttpSentContext> handler)
        {
            var assert = new AssertAction<HttpSentContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            this.OnSent(HandlerPriority.Last, assert);

            return this;
        }

        public IMockHttpBuilder VerifyOnException(Action<HttpExceptionContext> handler)
        {
            var assert = new AssertAction<HttpExceptionContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            this.OnException(HandlerPriority.Last, assert);

            return this;
        }

        public IMockHttpBuilder WithAssertFailure(Action failureAction)
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
            this.OnSending(HandlerPriority.Last, context => context.Items["MockRequest"] = context.Request);
            this.OnSent(HandlerPriority.First, context => context.Result.RequestMessage = context.Items["MockRequest"] as HttpRequestMessage);
        }
    }
}