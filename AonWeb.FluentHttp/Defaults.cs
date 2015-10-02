using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp
{
    public static class Defaults
    {
        static Defaults()
        {
            Client = HttpClientSettings.Empty;
            Builder = new HttpBuilderDefaults();
            TypedBuilder = new TypedBuilderDefaults();
            Caching = new CachingDefaults();
            Handlers = new HttpHandlerDefaults();
            Factory = new FactoryDefaults();

            Reset();
        }

        public static readonly HttpClientSettings Client;
        public static readonly HttpBuilderDefaults Builder;
        public static readonly TypedBuilderDefaults TypedBuilder;
        public static readonly CachingDefaults Caching;
        public static readonly HttpHandlerDefaults Handlers;
        public static readonly FactoryDefaults Factory;

        public class HttpBuilderDefaults
        {
            public HttpMethod HttpMethod { get; set; }
            public HttpCompletionOption CompletionOption { get; set; }
            public string MediaType { get; set; }
            public Encoding ContentEncoding { get; set; }
            public Func<HttpResponseMessage, bool> SuccessfulResponseValidator { get; set; }
            public Func<HttpResponseMessage, Exception> ExceptionFactory { get; set; }
            public bool SuppressCancellationErrors { get; set; }
            public bool AutoDecompressionEnabled { get; set; }
            public Func<IReadOnlyCollection<IHttpHandler>> HandlerFactory { get; set; }
            public Func<IReadOnlyCollection<IHttpHandler>> ChildHandlerFactory { get; set; }
        }

        public class TypedBuilderDefaults
        {
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
            public Func<IReadOnlyCollection<ITypedHandler>> HandlerFactory { get; set; }
            public Func<IReadOnlyCollection<ITypedHandler>> ChildHandlerFactory { get; set; }
            public Func<ErrorContext, Exception> ExceptionFactory { get; set; }
            public bool SuppressTypeMismatchExceptions { get; set; }

            public Exception DefaultExceptionFactory(ErrorContext context)
            {
                var openExType = typeof(HttpErrorException<>);

                var exType = openExType.MakeGenericType(context.ErrorType);

                return (Exception)Activator.CreateInstance(exType, context.Error, context.StatusCode);
            }

            public object DefaultDefaultResultFactory(Type type)
            {
                return TypeHelpers.GetDefaultValueForType(type);
            }

            public HashSet<HttpStatusCode> ValidStatusCodes { get; internal set; }

            public bool DefaultSuccessfulResponseValidator(HttpResponseMessage response)
            {
                return ValidStatusCodes.Contains(response.StatusCode);
            }
        }

        public class CachingDefaults
        {
            public bool Enabled { get; set; }
            public TimeSpan? DefaultDurationForCacheableResults { get; set; }
            public ISet<string> VaryByHeaders { get; set; }
            public ISet<HttpStatusCode> CacheableHttpStatusCodes { get; set; }
            public ISet<HttpMethod> CacheableHttpMethods { get; set; }
            public Func<IHttpCacheStore> CacheStoreFactory { get; set; }
            public Func<IVaryByStore> VaryByStoreFactory { get; set; }
            public Func<ICacheContext, ResponseInfo, ResponseValidationResult> ResponseValidator { get; set; }
            public Func<ICacheContext, bool> CacheValidator { get; set; }
            public Func<ICacheContext, ResponseInfo, bool> RevalidateValidator { get; set; }
            public Func<ICacheContext, ResponseInfo, bool> AllowStaleResultValidator { get; set; }

            public Func<IReadOnlyCollection<ITypedCacheHandler>> TypedHandlerFactory { get; set; }
            public Func<IReadOnlyCollection<IHttpCacheHandler>> HttpHandlerFactory { get; set; }
            public Func<ICacheKeyBuilder> CacheKeyBuilderFactory { get; set; }

            public bool DefaultCacheValidator(ICacheContext context)
            {
                if (!context.Enabled)
                    return false;

                if (context.Uri == null)
                    return false;

                var request = context.Request;

                if (!context.CacheableHttpMethods.Contains(request.Method))
                    return false;

                // client can tell HttpCallCacheHandler not to do caching for a particular request
                // rather than expiring here and facing a thundering herd, let a success repopulate
                if (request.Headers.CacheControl != null)
                {
                    if (request.Headers.CacheControl.NoStore)
                        return false;
                }

                if (typeof(IEmptyResult).IsAssignableFrom(context.ResultType))
                    return false;

                return true;
            }

            public bool DefaultRevalidateValidator(ICacheContext context, ResponseInfo responseInfo)
            {
                if (!context.Enabled)
                    return false;

                var request = context.Request;

                if (request == null)
                    return false;

                if (!context.CacheableHttpMethods.Contains(request.Method))
                    return false;

                return responseInfo != null && responseInfo.StatusCode == HttpStatusCode.NotModified;
            }

            public ResponseValidationResult DefaultResponseValidator(ICacheContext context, ResponseInfo responseInfo)
            {
                //This is almost verbatim from the CacheCow HttpCallCacheHandler's ResponseValidator func

                // 13.4
                //Unless specifically constrained by a cache-control (section 14.9) directive, a caching system MAY always store 
                // a successful response (see section 13.8) as a cache entry, MAY return it without validation if it 
                // is fresh, and MAY return it after successful validation. If there is neither a cache validator nor an 
                // explicit expiration time associated with a response, we do not expect it to be cached, but certain caches MAY violate this expectation 
                // (for example, when little or no network connectivity is available).

                // 14.9.1
                // If the no-cache directive does not specify a field-name, then a cache MUST NOT use the response to satisfy a subsequent request without 
                // successful revalidation with the origin server. This allows an origin server to prevent caching 
                // even by caches that have been configured to return stale responses to client requests.
                //If the no-cache directive does specify one or more field-names, then a cache MAY use the response 
                // to satisfy a subsequent request, subject to any other restrictions on caching. However, the specified 
                // field-name(s) MUST NOT be sent in the response to a subsequent request without successful revalidation 
                // with the origin server. This allows an origin server to prevent the re-use of certain header fields in a result, while still allowing caching of the rest of the response.

                if (!context.CacheableHttpStatusCodes.Contains(responseInfo.StatusCode))
                    return ResponseValidationResult.NotCacheable;

                if (responseInfo.NoStore)
                    return ResponseValidationResult.NotCacheable;

                if (!responseInfo.HasContent)
                    return ResponseValidationResult.NotCacheable;

                if (!responseInfo.Expiration.HasValue)
                    return ResponseValidationResult.NotCacheable;

                if (responseInfo.NoCache)
                    return ResponseValidationResult.MustRevalidate;

                if (responseInfo.Expiration < DateTimeOffset.UtcNow)
                    return responseInfo.ShouldRevalidate ? ResponseValidationResult.MustRevalidate : ResponseValidationResult.Stale;

                return ResponseValidationResult.OK;
            }

            public bool DefaultAllowStaleResultValidator(ICacheContext context, ResponseInfo responseInfo)
            {
                var request = context.Request;

                //This is almost verbatim from the CacheCow HttpCallCacheHandler's  IsFreshOrStaleAcceptable 

                if (responseInfo == null)
                    throw new ArgumentException(SR.CacheContextResponseInfoRequiredError, nameof(context));

                if (request == null)
                    throw new ArgumentException(SR.CacheContextRequestMessageRequiredError, nameof(context));

                if (responseInfo.HasContent)
                    return false;
                
                if (responseInfo.Expiration < DateTimeOffset.UtcNow)
                    return false;

                var responseDate = responseInfo.LastModified ?? responseInfo.Date;
                var staleness = DateTimeOffset.UtcNow - responseDate;

                if (request.Headers.CacheControl == null)
                    return staleness < TimeSpan.Zero;

                if (request.Headers.CacheControl.MinFresh.HasValue)
                    return -staleness > request.Headers.CacheControl.MinFresh.Value; // staleness is negative if still fresh

                if (request.Headers.CacheControl.MaxStale) // stale acceptable
                    return true;

                if (request.Headers.CacheControl.MaxStaleLimit.HasValue)
                    return staleness < request.Headers.CacheControl.MaxStaleLimit.Value;

                if (request.Headers.CacheControl.MaxAge.HasValue)
                    return responseDate.Add(request.Headers.CacheControl.MaxAge.Value) > DateTimeOffset.Now;

                return false;
            }
        }

        public class HttpHandlerDefaults
        {
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
        }

        public class FactoryDefaults
        {
            public Action<IHttpBuilder> DefaultHttpBuilderConfiguration { get; set; }
            public Action<ITypedBuilder> DefaultTypedBuilderConfiguration { get; set; }
        }

        public static void Reset()
        {
            // Client Settings
            Client.DecompressionMethods = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            //Builder Defaults
            Builder.HttpMethod = HttpMethod.Get;
            Builder.CompletionOption = HttpCompletionOption.ResponseContentRead;
            Builder.SuppressCancellationErrors = false;
            Builder.MediaType = "application/json";
            Builder.ContentEncoding = Encoding.UTF8;
            Builder.AutoDecompressionEnabled = true;
            Builder.HandlerFactory = () => new IHttpHandler[] { new RetryHandler(), new RedirectHandler(), new FollowLocationHandler(), new HttpCacheConfigurationHandler() };
            Builder.ChildHandlerFactory = () => new IHttpHandler[] { new RetryHandler(), new RedirectHandler(), new FollowLocationHandler() };

            //Typed Builder Defaults
            TypedBuilder.HttpMethod = HttpMethod.Get;
            TypedBuilder.CompletionOption = HttpCompletionOption.ResponseContentRead;
            TypedBuilder.SuppressCancellationErrors = false;
            TypedBuilder.SuppressTypeMismatchExceptions = false;
            TypedBuilder.SuccessfulResponseValidator = TypedBuilder.DefaultSuccessfulResponseValidator;
            TypedBuilder.MediaType = "application/json";
            TypedBuilder.ContentEncoding = Encoding.UTF8;
            TypedBuilder.AutoDecompressionEnabled = true;
            TypedBuilder.MediaTypeFormatters = () => new MediaTypeFormatterCollection().FluentAdd(new StringMediaFormatter());
            TypedBuilder.ResultType = typeof(string);
            TypedBuilder.ErrorType = typeof(string);
            TypedBuilder.ContentType = typeof(EmptyRequest);
            TypedBuilder.DefaultResultFactory = TypedBuilder.DefaultDefaultResultFactory;
            TypedBuilder.ExceptionFactory = TypedBuilder.DefaultExceptionFactory;
            TypedBuilder.HandlerFactory = () => new ITypedHandler[] { new TypedCacheConfigurationHandler() };
            TypedBuilder.ChildHandlerFactory = () => new ITypedHandler[] { new TypedCacheConfigurationHandler() };
            TypedBuilder.ValidStatusCodes = new HashSet<HttpStatusCode>
            {
                HttpStatusCode.OK,
                HttpStatusCode.Created,
                HttpStatusCode.Accepted,
                HttpStatusCode.NonAuthoritativeInformation,
                HttpStatusCode.NoContent,
                HttpStatusCode.ResetContent,
                HttpStatusCode.PartialContent
            };

            //Caching Defaults
            Caching.Enabled = true;
            Caching.CacheableHttpMethods = new HashSet<HttpMethod> { HttpMethod.Get };
            Caching.CacheableHttpStatusCodes = new HashSet<HttpStatusCode> { HttpStatusCode.OK };
            Caching.VaryByHeaders = new HashSet<string> { "Accept" };
            Caching.DefaultDurationForCacheableResults = TimeSpan.FromMinutes(15);
            Caching.CacheStoreFactory = () => new InMemoryCacheStore();
            Caching.VaryByStoreFactory = () => new InMemoryVaryByStore();
            Caching.ResponseValidator = Caching.DefaultResponseValidator;
            Caching.CacheValidator = Caching.DefaultCacheValidator;
            Caching.RevalidateValidator = Caching.DefaultRevalidateValidator;
            Caching.AllowStaleResultValidator = Caching.DefaultAllowStaleResultValidator;
            Caching.CacheKeyBuilderFactory = () => new CacheKeyBuilder();

            //Auto Follow Location
            Handlers.AutoFollowLocationEnabled = true;
            Handlers.FollowedStatusCodes = new HashSet<HttpStatusCode>
            {
                HttpStatusCode.Created,
                HttpStatusCode.SeeOther
            };

            //Redirect Defaults
            Handlers.AutoRedirectEnabled = true;
            Handlers.MaxAutoRedirects = 2;
            Handlers.RedirectStatusCodes = new HashSet<HttpStatusCode>
            {
                HttpStatusCode.MultipleChoices,
                HttpStatusCode.Found,
                HttpStatusCode.Redirect,
                HttpStatusCode.MovedPermanently,
                HttpStatusCode.UseProxy
            };

            //Retry
            Handlers.AutoRetryEnabled = true;
            Handlers.MaxAutoRetries = 2;
            Handlers.DefaultRetryAfter = TimeSpan.FromMilliseconds(100);
            Handlers.MaxRetryAfter = TimeSpan.FromSeconds(5);
            Handlers.RetryStatusCodes = new HashSet<HttpStatusCode> { HttpStatusCode.ServiceUnavailable };

            Factory.DefaultHttpBuilderConfiguration = null;
            Factory.DefaultTypedBuilderConfiguration = null;
        }
    }
}