namespace AonWeb.FluentHttp.Handlers
{
    public class ModifyTracker : ModifyTracker<object>
    {
        public ModifyTracker() { }

        public ModifyTracker(object value)
            : base(value) { }

        internal ModifyTracker(object value, bool modified)
            : base(value)
        {
            Modified = modified;
        }
    }

    public class ModifyTracker<T>
    {
        private T _value;

        public ModifyTracker() { }

        public ModifyTracker(T value)
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

        public ModifyTracker ToResult()
        {
            return new ModifyTracker(Value, Modified);
        }
    }
}