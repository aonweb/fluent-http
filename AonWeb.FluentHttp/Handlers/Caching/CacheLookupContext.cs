using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class CacheLookupContext<TResult> : CacheLookupContext
    {
        public CacheLookupContext(ICacheContext context)
            : base(context)
        { }

        internal CacheLookupContext(CacheLookupContext context)
            : base(context)
        { }

        public TResult Result
        {
            get { return ObjectHelpers.CheckType<TResult>(ResultInternal, SuppressTypeMismatchExceptions); }
            set { ResultInternal = value; }
        }
    }

    public abstract class CacheLookupContext : CacheHandlerContext
    {
        private readonly Modifiable _result;

        protected CacheLookupContext(ICacheContext context)
            : base(context)
        {
            _result = new Modifiable();
        }

        protected CacheLookupContext(CacheLookupContext context)
            : base(context)
        {
            _result = context._result;
        }

        protected object ResultInternal
        {
            get { return _result.Value; }
            set { _result.Value = value; }
        }

        public override Modifiable GetHandlerResult()
        {
            return _result;
        }
    }
}