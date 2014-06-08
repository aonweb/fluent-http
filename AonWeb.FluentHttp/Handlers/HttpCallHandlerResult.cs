namespace AonWeb.FluentHttp.Handlers
{
    public class HttpCallHandlerResult : HttpCallHandlerResult<object>
    {
        public HttpCallHandlerResult() { }

        public HttpCallHandlerResult(object value)
            : base(value) { }

        internal HttpCallHandlerResult(object value, bool modified)
            : base(value)
        {
            Modified = modified;
        }
    }

    public class HttpCallHandlerResult<T>
    {
        private T _value;

        public HttpCallHandlerResult() { }

        public HttpCallHandlerResult(T value)
        {
            _value = value;
        }

        public bool Modified { get; protected set; }

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                Modified = true;
            }
        }

        public HttpCallHandlerResult ToResult()
        {
            return new HttpCallHandlerResult(Value, Modified);
        }
    }
}