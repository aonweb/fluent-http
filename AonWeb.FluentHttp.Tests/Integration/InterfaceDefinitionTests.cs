using System.Net.Http;
using System.Threading.Tasks;

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
        public async Task SimpleCall()
        {
            var result = await HttpCallBuilder.Create()
                .WithUri(TestUriString)
                .ResultAsync();
        }

        [Test]
        public async Task SimpleCallWithMethod()
        {
            var result = await HttpCallBuilder.Create()
                .WithUri(TestUriString)
                .AsGet()
                .ResultAsync();
        }

        [Test]
        public async Task OrderDoesntMatter()
        {
            var result = await HttpCallBuilder.Create()
                .AsGet()
                .WithUri(TestUriString)
                .ResultAsync();
        }

        [Test]
        public async Task SimplePost()
        {
            var result = await HttpCallBuilder.Create()
                .WithUri(TestUriString)
                .AsPost()
                .WithContent("my data")
                .ResultAsync();
        }
    }
}