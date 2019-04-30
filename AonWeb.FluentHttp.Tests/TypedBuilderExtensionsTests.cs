using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp;

namespace AonWeb.FluentHttp.Tests
{
    public class TypedBuilderExtensionsTests
    {
        public ITypedBuilder CreateBuilder()
        {
            return new TypedBuilderFactory().Create().Advanced.WithCaching(true);
        }

        public void BasicExtensions()
        {
            var builder = CreateBuilder();

            builder
                .AsGet()
                .AsPut()
                .AsPost()
                .AsDelete()
                .AsPatch()
                .AsHead()
                .WithUri("http://localhost")
                .WithUri(new Uri("http://localhost"))
                .WithBaseUri("http://localhost")
                .WithBaseUri(new Uri("http://localhost"))
                .WithPath("/some/path")
                .WithQueryString("name", "value")
                .WithQueryString(new Dictionary<string, string> { { "name1", "value1" }, { "name2", "value2" } })
                .WithAppendQueryString("name", "value")
                .WithAppendQueryString(new Dictionary<string, string> { { "name1", "value1" }, { "name2", "value2" } })
                .WithOptionalQueryString("name", "value", v => v == null, v => v?.ToString() ?? string.Empty)
                .WithAppendOptionalQueryString("name", "value", v => v == null, v => v?.ToString() ?? string.Empty)
                //.WithContent("content")
                //.WithContent("content", Encoding.UTF8)
                //.WithContent("content", Encoding.UTF8, "application/json")
                //.WithContent(() => "content")
                //.WithContent(() => "content", Encoding.UTF8)
                //.WithContent(() => "content", Encoding.UTF8, "application/json")
                //.WithContent(() => new StringContent("content"))
                //.WithContent(() => Task.FromResult<HttpContent>(new StringContent("content")))
                //.WithContent((ctx) => new StringContent("content", ctx.ContentEncoding, ctx.MediaType))
                //.WithContent((ctx) => Task.FromResult<HttpContent>(new StringContent("content", ctx.ContentEncoding, ctx.MediaType)))
                ;

            //builder.ResultAsync();
            builder.SendAsync();

        }

        public void BasicWithAdvancedExtensions()
        {
            var builder = CreateBuilder();

            builder
                .Advanced
                .AsGet()
                .AsPut()
                .AsPost()
                .AsDelete()
                .AsPatch()
                .AsHead()
                .WithUri("http://localhost")
                .WithUri(new Uri("http://localhost"))
                .WithBaseUri("http://localhost")
                .WithBaseUri(new Uri("http://localhost"))
                .WithPath("/some/path")
                .WithQueryString("name", "value")
                .WithQueryString(new Dictionary<string, string> { { "name1", "value1" }, { "name2", "value2" } })
                .WithAppendQueryString("name", "value")
                .WithAppendQueryString(new Dictionary<string, string> { { "name1", "value1" }, { "name2", "value2" } })
                .WithOptionalQueryString("name", "value", v => v == null, v => v?.ToString() ?? string.Empty)
                .WithAppendOptionalQueryString("name", "value", v => v == null, v => v?.ToString() ?? string.Empty)
                //.WithContent("content")
                //.WithContent("content", Encoding.UTF8)
                //.WithContent("content", Encoding.UTF8, "application/json")
                //.WithContent(() => "content")
                //.WithContent(() => "content", Encoding.UTF8)
                //.WithContent(() => "content", Encoding.UTF8, "application/json")
                //.WithContent(() => new StringContent("content"))
                //.WithContent(() => Task.FromResult<HttpContent>(new StringContent("content")))
                //.WithContent(ctx => new StringContent("content", ctx.ContentEncoding, ctx.MediaType))
                //.WithContent(ctx => Task.FromResult<HttpContent>(new StringContent("content", ctx.ContentEncoding, ctx.MediaType)))
                ;

            //builder.ResultAsync();
            builder.SendAsync();
        }

        public void AdvancedExtensions()
        {
            var builder = CreateBuilder();

            builder
                .Advanced
                // Client
                .WithAutoDecompression()
                .WithDecompressionMethods(DecompressionMethods.GZip)
                .WithClientCertificateOptions(ClientCertificateOption.Automatic)
                .WithCheckCertificateRevocationList(true)
                .WithClientCertificates(new List<X509Certificate> {new X509Certificate()})
                .WithClientCertificates(new X509Certificate())
                .WithUseCookies()
                .WithUseCookies(new CookieContainer())
                .WithCredentials(new NetworkCredential())
                // .WithMaxConnectionsPerServer(int.MaxValue)
                // .WithMaxResponseHeadersLength(int.MaxValue)
                // .WithMaxRequestContentBufferSize(long.MaxValue)
                // .WithMaxResponseContentBufferSize(long.MaxValue)
                // .WithPreAuthenticate(true)
                // .WithProperty("key", "value")
                // .WithProperties(new Dictionary<string, object> { { "key", new object() } })
                // .WithProperties(new KeyValuePair<string, object>("key", new object()))
                // .WithNoProperties()
                // .WithProxy(new WebProxy())
                // .WithProxyCredentials(new NetworkCredential())
                // .WithServerCertificateCustomValidationCallback((message, certificate2, arg3, arg4) => true)
                // .WithSslProtocols(SslProtocols.Tls12)
                .WithTimeout(TimeSpan.Zero)
                // Request
                .WithScheme("http")
                .WithHost("mydomain")
                .WithPort(8080)
                .WithMethod("GET")
                .WithMethod(HttpMethod.Head)
                .WithAcceptHeaderValue("text/html")
                .WithAcceptCharSet(Encoding.ASCII)
                .WithAcceptCharSet("utf8")
                .WithHeadersConfiguration(h => h.IfModifiedSince = DateTimeOffset.UtcNow)
                .WithHeader("Pragma", "no-cache")
                .WithHeader("Pragma", new List<string> {"no-cache"})
                .WithAppendHeader("Pragma", "no-cache")
                .WithAppendHeader("Pragma", new List<string> {"no-cache"})
                .WithNoCache()
                .WithContentEncoding(Encoding.UTF8)
                .WithMediaType("application/json")
                // .WithContentHeadersConfiguration(h => h.ContentType = new MediaTypeHeaderValue("application/json"))
                // .WithContentHeader("Content-Type", "application/json")
                // .WithContentHeader("Allow", new List<string> { "GET", "POST" })
                // .WithAppendContentHeader("Allow", "DELETE")
                // .WithAppendContentHeader("Allow", new List<string> { "PUT", "PATCH" });
                ;

            //Builder
            //.WithCaching(true)
            //.WithSuppressCancellationExceptions(true)
            //.WithRetryConfiguration(x => { })
            //.WithRedirectConfiguration(x => { })
            //.WithAutoFollowConfiguration(x => { })
            //.WithHandler(null)
            //.WithHandlerConfiguration(x => { })
            //.WithOptionalHandlerConfiguration(x => { })
            //.WithSuccessfulResponseValidator(x => true)
            //.WithExceptionFactory(x => new Exception())
            //.WithContextItem("key", "value")
            //.OnSending(x => { })
            //.OnSending(HandlerPriority.Default, x => { })
            //.OnSendingAsync(x => Task.CompletedTask)
            //.OnSendingAsync(HandlerPriority.Default, x => Task.CompletedTask)
            //.OnSent(x => { })
            //.OnSent(HandlerPriority.Default, x => { })
            //.OnSentAsync(x => Task.CompletedTask)
            //.OnSentAsync(HandlerPriority.Default, x => Task.CompletedTask)
            //.OnException(x => { })
            //.OnException(HandlerPriority.Default, x => { })
            //.OnExceptionAsync(x => Task.CompletedTask)
            //.OnExceptionAsync(HandlerPriority.Default, x => Task.CompletedTask)
            //// Caching Events
            //// Hit
            //.OnCacheHit(x => { })
            //.OnCacheHit(HandlerPriority.Default, x => { })
            //.OnCacheHitAsync(x => Task.CompletedTask)
            //.OnCacheHitAsync(HandlerPriority.Default, x => Task.CompletedTask)
            //// Miss
            //.OnCacheMiss(x => { })
            //.OnCacheMiss(HandlerPriority.Default, x => { })
            //.OnCacheMissAsync(x => Task.CompletedTask)
            //.OnCacheMissAsync(HandlerPriority.Default, x => Task.CompletedTask)
            //// Store
            //.OnCacheStore(x => { })
            //.OnCacheStore(HandlerPriority.Default, x => { })
            //.OnCacheStoreAsync(x => Task.CompletedTask)
            //.OnCacheStoreAsync(HandlerPriority.Default, x => Task.CompletedTask)
            //// Expiring
            //.OnCacheExpiring(x => { })
            //.OnCacheExpiring(HandlerPriority.Default, x => { })
            //.OnCacheExpiringAsync(x => Task.CompletedTask)
            //.OnCacheExpiringAsync(HandlerPriority.Default, x => Task.CompletedTask)
            //// Expired
            //.OnCacheExpired(x => { })
            //.OnCacheExpired(HandlerPriority.Default, x => { })
            //.OnCacheExpiredAsync(x => Task.CompletedTask)
            //.OnCacheExpiredAsync(HandlerPriority.Default, x => Task.CompletedTask);

        }


    }
}
