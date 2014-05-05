using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using AonWeb.FluentHttp;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Tests.Helpers;
using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Http
{
    [TestFixture]
    public class HttpCallBuilderAdvancedMethodTests
    {
        #region Declarations, Set up, & Tear Down
        
        private const string TestUriString = LocalWebServer.DefaultListenerUri;

        private LocalWebServer _server;

        [SetUp]
        public void Setup()
        {
            _server = LocalWebServer.ListenInBackground(TestUriString);
        }

        [TearDown]
        public void TearDown()
        {
            if (_server != null)
                _server.Stop();
        }

        #endregion

        #region WithMethod

        [Test]
        public void WithMethod_WhenValidString_ExpectResultUsesMethod()
        {
            //arrange
            var method = "GET";
            var builder = HttpCallBuilder.Create().WithUri(TestUriString).Advanced.WithMethod(method);

            // assert (called after act)
            _server.InspectRequest(r => Assert.AreEqual(r.HttpMethod, method));

            //act
            builder.Result();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WithMethod_WhenNullString_ExpectException()
        {
            //arrange
            string method = null;
            var builder = HttpCallBuilder.Create();

            //act
            builder.Advanced.WithMethod(method);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WithMethod_WhenEmptyString_ExpectException()
        {
            //arrange
            var method = string.Empty;
            var builder = HttpCallBuilder.Create();

            //act
            builder.Advanced.WithMethod(method);
        }

        [Test]
        public void WithMethod_WhenValidMethod_ExpectResultUsesMethod()
        {
            //arrange
            var method = HttpMethod.Get;
            var builder = HttpCallBuilder.Create().WithUri(TestUriString).Advanced.WithMethod(method);

            // assert (called after act)
            _server.InspectRequest(r => Assert.AreEqual(r.HttpMethod, method.Method));

            //act
            builder.Result();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithMethod_WhenNullMethod_ExpectException()
        {
            //arrange
            HttpMethod method = null;
            var builder = HttpCallBuilder.Create().Advanced.WithMethod(method);

            //act
            builder.Result();
        }

        [Test]
        public void WithMethod_WhenCalledMultipleTimes_ExpectLastWins()
        {
            //arrange
            var method1 = "POST";
            var method2 = "GET";
            var builder = HttpCallBuilder.Create().WithUri(TestUriString).Advanced.WithMethod(method1).Advanced.WithMethod(method2);

            // assert (called after act)
            _server.InspectRequest(r => Assert.AreEqual(r.HttpMethod, method2));

            //act
            builder.Result();
        }

        #endregion

        [Test]
        public void WithConfiguration_WhenAction_ExpectConfigurationApplied()
        {
            Action<IHttpClient> config = client => client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var builder = HttpCallBuilder.Create();

            // assert (called after act)
            _server.InspectRequest(r => Assert.AreEqual(r.Headers["Accept"], "application/json"));

            //act
            builder.Advanced.ConfigureClient(config).ResultAsync();


        }

        /*
         IHttpCallBuilder WithScheme(string scheme);
        IHttpCallBuilder WithHost(string host);
        IHttpCallBuilder WithPort(int port);
        IHttpCallBuilder WithPath(string absolutePathAndQuery);
        IHttpCallBuilder WithEncoding(Encoding encoding);
        IHttpCallBuilder WithMediaType(string mediaType);

        IHttpCallBuilder ConfigureClient(Action<IHttpClient> configuration);
        IHttpCallBuilder ConfigureClient(Action<IHttpClientBuilder> configuration);
        IHttpCallBuilder ConfigureRetries(Action<RetryHandler> configuration);
        IHttpCallBuilder ConfigureRedirect(Action<RedirectHandler> configuration);

        IHttpCallBuilder WithHandler(IHttpCallHandler handler);
        IHttpCallBuilder ConfigureHandler<THandler>(Action<THandler> configure) 
            where THandler : class, IHttpCallHandler;
        IHttpCallBuilder WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IHttpCallBuilder WithExceptionFactory(Func<HttpResponseMessage, Exception> factory);
        IHttpCallBuilder WithNoCache();

        IHttpCallBuilder OnSending(Action<HttpSendingContext> handler);
        IHttpCallBuilder OnSending(HttpCallHandlerPriority priority, Action<HttpSendingContext> handler);
        IHttpCallBuilder OnSending(Func<HttpSendingContext, Task> handler);
        IHttpCallBuilder OnSending(HttpCallHandlerPriority priority, Func<HttpSendingContext, Task> handler);

        IHttpCallBuilder OnSent(Action<HttpSentContext> handler);
        IHttpCallBuilder OnSent(HttpCallHandlerPriority priority, Action<HttpSentContext> handler);
        IHttpCallBuilder OnSent(Func<HttpSentContext, Task> handler);
        IHttpCallBuilder OnSent(HttpCallHandlerPriority priority, Func<HttpSentContext, Task> handler);

        IHttpCallBuilder OnException(Action<HttpExceptionContext> handler);
        IHttpCallBuilder OnException(HttpCallHandlerPriority priority, Action<HttpExceptionContext> handler);
        IHttpCallBuilder OnException(Func<HttpExceptionContext, Task> handler);
        IHttpCallBuilder OnException(HttpCallHandlerPriority priority, Func<HttpExceptionContext, Task> handler);
         */
    }
}
