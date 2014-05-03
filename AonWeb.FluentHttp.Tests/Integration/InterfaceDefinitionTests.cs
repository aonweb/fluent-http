using System.Net.Http;
using AonWeb.FluentHttp.Tests.Helpers;
using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Integration
{
    [TestFixture]
    public class InterfaceDefinitionTests
    {
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

        [Test]
        public void SimpleCall()
        {
            var result = new HttpCallBuilder()
                .WithUri(TestUriString)
                .Result();
        }

        [Test]
        public void SimpleCallWithMethod()
        {
            var result = new HttpCallBuilder()
                .WithUri(TestUriString)
                .WithMethod(HttpMethod.Get)
                .Result();
        }

        [Test]
        public void OrderDoesntMatter()
        {
            var result = new HttpCallBuilder()
                .WithMethod("GET")
                .WithUri(TestUriString)
                .Result();
        }

        [Test]
        public void SimplePost()
        {
            var result = new HttpCallBuilder()
                .WithUri(TestUriString)
                .WithMethod(HttpMethod.Post)
                .WithContent("my data")
                .Result();
        }
    }
}