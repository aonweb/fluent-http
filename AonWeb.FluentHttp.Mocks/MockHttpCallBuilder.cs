using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpBuilder : HttpBuilder, IMockHttpBuilder
    {
        private readonly IList<IAssertAction> _asserts;
        private Action _assertFailure;

        public MockHttpBuilder(IHttpBuilderSettings settings, IHttpClientBuilder clientBuilder,
            IReadOnlyCollection<IHandler> defaultHandlers)
            : base(settings, clientBuilder, defaultHandlers)
        {
             _asserts = new List<IAssertAction>();
            _assertFailure = (() => { throw new Exception("assertion was never called"); });

            ConfigureMock();
        }

        public IMockHttpBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            WithClientConfiguration(b => ((MockHttpClientBuilder)b).WithResponse(responseFactory));

            return this;
        }

        public IMockHttpBuilder VerifyOnSending(Action<SendingContext> handler)
        {
            var assert = new AssertAction<SendingContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            this.OnSending(HandlerPriority.Last, assert);

            return this;
        }

        public IMockHttpBuilder VerifyOnSent(Action<SentContext> handler)
        {
            var assert = new AssertAction<SentContext>(handler, () => _assertFailure);

            _asserts.Add(assert);

            this.OnSent(HandlerPriority.Last, assert);

            return this;
        }

        public IMockHttpBuilder VerifyOnException(Action<ExceptionContext> handler)
        {
            var assert = new AssertAction<ExceptionContext>(handler, () => _assertFailure);

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