using System.Net.Http;

using AonWeb.Fluent.Http;

using NUnit.Framework;

namespace AonWeb.Fluent.Tests.Http
{
    [TestFixture]
    public class InterfaceDefinitionTests
    {
        private string TestUriString = "http://google.com";
        private string TestMethod = "GET";

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
                .WithMethod(TestMethod)
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