using System;
using System.Collections;
using System.Net.Http;
using System.Threading;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class CacheContext: CacheMetadata, ICacheContext
    {
        private readonly IHandlerContext _handlerContext;

        public CacheContext(ICacheContext context)
            : this(context, context.GetHandlerContext()) { }

        public CacheContext(ICacheValidator context, IHandlerContext handlerContext)
            : base(context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (handlerContext == null)
                throw new ArgumentNullException(nameof(handlerContext));

            _handlerContext = handlerContext;

            Request = handlerContext.Request;
            Uri = Request?.RequestUri.Normalize();
            Token = handlerContext.Token;

            HandlerRegister = context.HandlerRegister;
            ResultInspector = context.ResultInspector;
            RequestValidator = context.RequestValidator;
            ResponseValidator = context.ResponseValidator;
            RevalidateValidator = context.RevalidateValidator;
            AllowStaleResultValidator = context.AllowStaleResultValidator;
        }

        public IDictionary Items => _handlerContext.Items;
        public Type ResultType => _handlerContext.ResultType;

        public HttpRequestMessage Request { get; }

        public Uri Uri { get; }
        public CacheHandlerRegister HandlerRegister { get; }
        public Action<CacheEntry> ResultInspector { get; }
        public Func<ICacheContext, RequestValidationResult> RequestValidator { get; set; }
        public Func<ICacheContext, IResponseMetadata, ResponseValidationResult> ResponseValidator { get; }
        public Func<ICacheContext, IResponseMetadata, bool> RevalidateValidator { get; }
        public Func<ICacheContext, IResponseMetadata, bool> AllowStaleResultValidator { get; }
        public CancellationToken Token { get; }

        public IHandlerContext GetHandlerContext()
        {
            return _handlerContext;
        }
    }
}