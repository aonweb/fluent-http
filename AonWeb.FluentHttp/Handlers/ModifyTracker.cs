using System;
using System.Collections;
using System.Collections.Generic;

namespace AonWeb.FluentHttp.Handlers
{
    public class ModifyTracker<T> : ModifyTracker
    {
        public ModifyTracker() { }

        public ModifyTracker(object value)
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
                Modified = true;
            }
        }
    }

    public class ModifyTracker
    {
        private object _value;

        public ModifyTracker() { }

        public ModifyTracker(object value)
        {
            _value = value;
        }

        internal ModifyTracker(object value, bool modified)
            : this(value)
        {
            Modified = modified;
        }

        public bool Modified { get; protected set; }

        public virtual object Value
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

    public class UriListModifyTracker : ModifyTracker, ICollection<Uri>
    {
        private readonly object _lock = new object();

        public UriListModifyTracker()
            : base(new List<Uri>()) { }

        public UriListModifyTracker(IEnumerable<Uri> value)
            : base(new List<Uri>(value)) { }

        public new ICollection<Uri> Value
        {
            get
            {
                return ValueInternal;
            }
            set
            {
                ValueInternal = new List<Uri>(value);
            }
        }

        private List<Uri> ValueInternal
        {
            get
            {
                return (List<Uri>)base.Value;
            }
            set
            {
                lock (_lock)
                {
                    base.Value = value;
                    Modified = true;
                }
                
            }
        }

        public IEnumerator<Uri> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Uri item)
        {
            lock (_lock)
            {
                 Value.Add(item);
                 Modified = true;
            }
            
        }

        public void Clear()
        {
            lock (_lock)
            {
                Value.Clear();
                Modified = true;
            }
        }

        public bool Contains(Uri item)
        {
            return Value.Contains(item);
        }

        public void CopyTo(Uri[] array, int arrayIndex)
        {
            Value.CopyTo(array, arrayIndex);
        }

        public bool Remove(Uri item)
        {
            lock (_lock)
            {
                var removed = Value.Remove(item);

                if (removed)
                    Modified = true;

                return removed;
            }
        }

        public int Count { get { return Value.Count; } }
        public bool IsReadOnly { get { return Value.IsReadOnly; } }
    }
}