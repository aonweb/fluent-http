using System;
using System.Collections;
using System.Collections.Generic;

namespace AonWeb.FluentHttp.Handlers
{
    public class ModifiableUriList : Modifiable, ICollection<Uri>
    {
        private readonly object _lock = new object();

        public ModifiableUriList()
            : base(new List<Uri>()) { }

        public ModifiableUriList(IEnumerable<Uri> value)
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
                    IsDirty = true;
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
                IsDirty = true;
            }
            
        }

        public void Clear()
        {
            lock (_lock)
            {
                Value.Clear();
                IsDirty = true;
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
                    IsDirty = true;

                return removed;
            }
        }

        public int Count => Value.Count;
        public bool IsReadOnly => Value.IsReadOnly;
    }
}