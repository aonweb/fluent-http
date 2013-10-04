using AonWeb.Fluent.Http;

using NUnit.Framework;

namespace AonWeb.Fluent.Tests.Http
{
    [TestFixture]
    public class InterfaceDefinitionTests
    {
        [Test]
        public void Tests()
        {
            var builder = new HttpCallBuilder();


            builder.ConfigureClient(
                (IHttpClientBuilder c) =>
                {
                    c.WithAutoRedirect(1);
                });

            builder.ConfigureClient(
                (IHttpClient client) =>
                {
                   
                });


        }
    }
}