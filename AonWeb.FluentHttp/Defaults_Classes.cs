using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;
using StringMediaFormatter = AonWeb.FluentHttp.StringMediaFormatter;

namespace AonWeb.FluentHttp
{
    public partial class Defaults
    {
        public class HttpBuilderDefaults
        {
            public HttpBuilderDefaults(Defaults defaults)
            {
                Reset();

                defaults.ResetRequested += (sender, args) => Reset();
            }

            public HttpMethod HttpMethod { get; set; }
            public HttpCompletionOption CompletionOption { get; set; }
            public string MediaType { get; set; }
            public Encoding ContentEncoding { get; set; }
            public Func<HttpResponseMessage, bool> SuccessfulResponseValidator { get; set; }
            public Func<HttpResponseMessage, Exception> ExceptionFactory { get; set; }
            public bool SuppressCancellationErrors { get; set; }
            public bool AutoDecompressionEnabled { get; set; }
            public HandlerFactoryDefaults<IHttpHandler, IHttpBuilderSettings> Handlers { get; set; }
            public HandlerFactoryDefaults<IHttpHandler, IHttpBuilderSettings> ChildHandlers { get; set; }
            public Action<IHttpBuilder> DefaultBuilderConfiguration { get; set; }

            private void Reset()
            {
                HttpMethod = HttpMethod.Get;
                CompletionOption = HttpCompletionOption.ResponseContentRead;
                SuppressCancellationErrors = false;
                MediaType = "application/json";
                ContentEncoding = Encoding.UTF8;
                AutoDecompressionEnabled = true;
                Handlers = new HandlerFactoryDefaults<IHttpHandler, IHttpBuilderSettings>
                {
                    Factory = settings => new IHttpHandler[] { new RetryHandler(), new RedirectHandler(), new FollowLocationHandler(), new HttpCacheConfigurationHandler(settings.CacheSettings) }

                };
                ChildHandlers = new HandlerFactoryDefaults<IHttpHandler, IHttpBuilderSettings>
                {
                    Factory = settings => new IHttpHandler[] { new RetryHandler(), new RedirectHandler(), new FollowLocationHandler() }
                };
                DefaultBuilderConfiguration = null;
            }
        }

        public class TypedBuilderDefaults
        {
            public TypedBuilderDefaults(Defaults defaults)
            {
                Reset();

                defaults.ResetRequested += (sender, args) => Reset();
            }

            public HttpMethod HttpMethod { get; set; }
            public HttpCompletionOption CompletionOption { get; set; }
            public string MediaType { get; set; }
            public Encoding ContentEncoding { get; set; }
            public Func<HttpResponseMessage, bool> SuccessfulResponseValidator { get; set; }
            public bool SuppressCancellationErrors { get; set; }
            public bool AutoDecompressionEnabled { get; set; }
            public Func<MediaTypeFormatterCollection> MediaTypeFormatters { get; set; }
            public Type ResultType { get; set; }
            public Type ErrorType { get; set; }
            public Type ContentType { get; set; }
            public Func<Type, object> DefaultResultFactory { get; set; }
            public Func<Type, Exception, object> DefaultErrorFactory { get; set; }
            public HandlerFactoryDefaults<ITypedHandler, ITypedBuilderSettings> Handlers { get; set; }
            public HandlerFactoryDefaults<ITypedHandler, ITypedBuilderSettings> ChildHandlers { get; set; }
            public Func<ExceptionCreationContext, Exception> ExceptionFactory { get; set; }
            public bool SuppressTypeMismatchExceptions { get; set; }
            public Action<ITypedBuilder> DefaultBuilderConfiguration { get; set; }

            public HashSet<HttpStatusCode> ValidStatusCodes { get; internal set; }
            public Func<ITypedBuilderContext, object, Task<HttpContent>> HttpContentFactory { get; set; }
            public Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, Task<object>> ResultFactory { get; set; }
            public Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, ExceptionDispatchInfo, Task<object>> ErrorFactory { get; set; }

            public bool DefaultSuccessfulResponseValidator(HttpResponseMessage response)
            {
                return ValidStatusCodes.Contains(response.StatusCode);
            }

            private void Reset()
            {
                //Typed Builder Defaults
                HttpMethod = HttpMethod.Get;
                CompletionOption = HttpCompletionOption.ResponseContentRead;
                SuppressCancellationErrors = false;
                SuppressTypeMismatchExceptions = false;
                SuccessfulResponseValidator = DefaultSuccessfulResponseValidator;
                MediaType = "application/json";
                ContentEncoding = Encoding.UTF8;
                AutoDecompressionEnabled = true;
                MediaTypeFormatters = () => new MediaTypeFormatterCollection().FluentAdd(new StringMediaFormatter());
                ResultType = typeof(string);
                ErrorType = typeof(string);
                ContentType = typeof(EmptyRequest);
                DefaultResultFactory = TypeHelpers.GetDefaultValueForType;
                DefaultErrorFactory = (type, exception) => TypeHelpers.GetDefaultValueForType(type);
                ExceptionFactory = ObjectHelpers.CreateException;
                Handlers = new HandlerFactoryDefaults<ITypedHandler, ITypedBuilderSettings>
                {
                    Factory = settings => new ITypedHandler[] { new TypedCacheConfigurationHandler(settings.CacheSettings) }
                };
                ChildHandlers = new HandlerFactoryDefaults<ITypedHandler, ITypedBuilderSettings>
                {
                    Factory = settings => new ITypedHandler[] { new TypedCacheConfigurationHandler(settings.CacheSettings) }
                };
                ValidStatusCodes = new HashSet<HttpStatusCode>
                {
                    HttpStatusCode.OK,
                    HttpStatusCode.Created,
                    HttpStatusCode.Accepted,
                    HttpStatusCode.NonAuthoritativeInformation,
                    HttpStatusCode.NoContent,
                    HttpStatusCode.ResetContent,
                    HttpStatusCode.PartialContent
                };
                DefaultBuilderConfiguration = null;
                HttpContentFactory = ObjectHelpers.CreateHttpContent;
                ResultFactory = ObjectHelpers.CreateResult;
                ErrorFactory = ObjectHelpers.CreateError;
            }
        }

        public class CachingDefaults
        {
            public CachingDefaults(Defaults defaults)
            {
                Reset();

                defaults.ResetRequested += (sender, args) => Reset();
            }

            public bool Enabled { get; set; }
            public TimeSpan? DefaultDurationForCacheableResults { get; set; }
            public ISet<string> VaryByHeaders { get; set; }
            public ISet<HttpStatusCode> CacheableHttpStatusCodes { get; set; }
            public ISet<HttpMethod> CacheableHttpMethods { get; set; }
            public Func<IHttpCacheStore> CacheStoreFactory { get; set; }
            public Func<IVaryByStore> VaryByStoreFactory { get; set; }
            public Func<ICacheContext, IResponseMetadata, ResponseValidationResult> ResponseValidator { get; set; }
            public Func<ICacheContext, bool> CacheValidator { get; set; }
            public Func<ICacheContext, IResponseMetadata, bool> RevalidateValidator { get; set; }
            public Func<ICacheContext, IResponseMetadata, bool> AllowStaleResultValidator { get; set; }

            public HandlerFactoryDefaults<ITypedCacheHandler, ICacheSettings> TypedHandlers { get; private set; }
            public HandlerFactoryDefaults<IHttpCacheHandler, ICacheSettings> HttpHandlers { get; private set; }
            public Func<ICacheKeyBuilder> CacheKeyBuilderFactory { get; set; }
            public bool MustRevalidate { get; set; }

            private void Reset()
            {
                //Caching Defaults
                Enabled = true;
                CacheableHttpMethods = new HashSet<HttpMethod> { HttpMethod.Get };
                CacheableHttpStatusCodes = new HashSet<HttpStatusCode> { HttpStatusCode.OK };
                VaryByHeaders = new HashSet<string> { "Accept" };
                DefaultDurationForCacheableResults = TimeSpan.FromMinutes(15);
                CacheStoreFactory = () => new InMemoryCacheStore();
                VaryByStoreFactory = () => new InMemoryVaryByStore();
                ResponseValidator = (ctx, res) => CachingHelpers.ValidateResponse(res, ctx.CacheableHttpStatusCodes);
                CacheValidator = CachingHelpers.ValidateCacheContext;
                RevalidateValidator = (ctx, res) => CachingHelpers.ShouldRevalidate(ctx, ctx.Request, res);
                AllowStaleResultValidator = (ctx, res) => CachingHelpers.AllowStale(ctx.Request, res);
                CacheKeyBuilderFactory = () => new CacheKeyBuilder();
                TypedHandlers = new HandlerFactoryDefaults<ITypedCacheHandler, ICacheSettings>();
                HttpHandlers = new HandlerFactoryDefaults<IHttpCacheHandler, ICacheSettings>();
                MustRevalidate = false;
            }
        }

        public class HttpHandlerDefaults
        {
            public HttpHandlerDefaults(Defaults defaults)
            {
                Reset();

                defaults.ResetRequested += (sender, args) => Reset();
            }

            public bool AutoRedirectEnabled { get; set; }
            public int MaxAutoRedirects { get; set; }
            public ISet<HttpStatusCode> RedirectStatusCodes { get; set; }

            public bool AutoFollowLocationEnabled { get; set; }
            public ISet<HttpStatusCode> FollowedStatusCodes { get; set; }

            public bool AutoRetryEnabled { get; set; }
            public int MaxAutoRetries { get; set; }
            public TimeSpan DefaultRetryAfter { get; set; }
            public TimeSpan MaxRetryAfter { get; set; }
            public ISet<HttpStatusCode> RetryStatusCodes { get; set; }

            private void Reset()
            {
                //Auto Follow Location
                AutoFollowLocationEnabled = true;
                FollowedStatusCodes = new HashSet<HttpStatusCode>
            {
                HttpStatusCode.Created,
                HttpStatusCode.SeeOther
            };

                //Redirect Defaults
                AutoRedirectEnabled = true;
                MaxAutoRedirects = 2;
                RedirectStatusCodes = new HashSet<HttpStatusCode>
            {
                HttpStatusCode.MultipleChoices,
                HttpStatusCode.Found,
                HttpStatusCode.Redirect,
                HttpStatusCode.MovedPermanently,
                HttpStatusCode.UseProxy
            };

                //Retry
                AutoRetryEnabled = true;
                MaxAutoRetries = 2;
                DefaultRetryAfter = TimeSpan.FromMilliseconds(100);
                MaxRetryAfter = TimeSpan.FromSeconds(5);
                RetryStatusCodes = new HashSet<HttpStatusCode> { HttpStatusCode.ServiceUnavailable };
            }
        }
    }
}