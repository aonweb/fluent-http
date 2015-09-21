namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class CacheMissContext<TResult> : CacheMissContext
    {
        public CacheMissContext(ICacheContext context)
            : base(context) { }

        internal CacheMissContext(CacheMissContext context)
            : base(context) { }

        public TResult Result
        {
            get { return (TResult)ResultInternal; }
            set { ResultInternal = value; }
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