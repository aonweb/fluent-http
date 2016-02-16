namespace AonWeb.FluentHttp.Handlers
{
    public class Modifiable<T> : Modifiable
    {
        public Modifiable() { }

        public Modifiable(object value)
            : base(value) { }

        public new T Value
        {
            get
            {
                return (T)base.Value;
            }
            set
            {
                base.Value = value;
                IsDirty = true;
            }
        }
    }

    public class Modifiable
    {
        private object _value;

        public Modifiable() { }

        public Modifiable(object value)
        {
            _value = value;
        }

        internal Modifiable(object value, bool isDirty)
            : this(value)
        {
            IsDirty = isDirty;
        }

        public bool IsDirty { get; protected set; }

        public virtual object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                IsDirty = true;
            }
        }

        public Modifiable ToResult()
        {
            return new Modifiable(Value, IsDirty);
        }
    }
}