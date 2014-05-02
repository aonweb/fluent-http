using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using AonWeb.FluentHttp.Client;
using Moq;

namespace AonWeb.FluentHttp.Tests
{
    public static class AssertExtensions
    {
        public static void VerifyRequest(this Mock<IHttpClient> mock, Expression<Func<HttpRequestMessage, bool>> messagePredicate)
        {
            mock.VerifyRequest(messagePredicate, Times.Once());
        }

        public static void VerifyRequest(this Mock<IHttpClient> mock, Expression<Func<HttpRequestMessage, bool>> messagePredicate, Times times)
        {
            mock.Verify(m => m.SendAsync(It.Is(messagePredicate), It.IsAny<HttpCompletionOption>(), It.IsAny<CancellationToken>()), times);
        }
    }
}