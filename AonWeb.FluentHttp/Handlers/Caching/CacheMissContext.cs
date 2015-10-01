using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class CacheMissContext<TResult> : CacheMissContext
    {
        public CacheMissContext(ICacheContext context)
            : base(context) { }

        internal CacheMissContext(CacheMissContext context)
            : base(context) { }

        public new TResult Result
        {
            get { return ObjectHelpers.CheckType<TResult>(base.Result, SuppressTypeMismatchExceptions); }
            set { base.Result = value; }
        }
    }

    public abstract class CacheMissContext : CacheHandlerContext
    {
        private readonly Modifiable _result;

        protected CacheMissContext(ICacheContext context)
            : base(context)
        {
            _result = new Modifiable();
        }

        protected CacheMissContext(CacheMissContext context)
            : base(context)
        {
            _result = context._result;
        }

        public object Result
        {
            get { return _result.Value; }
            protected set { _result.Value = value; }
        }

        public override Modifiable GetHandlerResult()
        {
            return _result;
        }
    }
}