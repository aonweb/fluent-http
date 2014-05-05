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
            var result = HttpCallBuilder.Create()
                .WithUri(TestUriString)
                .Result();
        }

        [Test]
        public void SimpleCallWithMethod()
        {
            var result = HttpCallBuilder.Create()
                .WithUri(TestUriString)
                .AsGet()
                .Result();
        }

        [Test]
        public void OrderDoesntMatter()
        {
            var result = HttpCallBuilder.Create()
                .AsGet()
                .WithUri(TestUriString)
                .Result();
        }

        [Test]
        public void SimplePost()
        {
            var result = HttpCallBuilder.Create()
                .WithUri(TestUriString)
                .AsPost()
                .WithContent("my data")
                .Result();
        }
    }
}