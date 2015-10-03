using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AonWeb.FluentHttp
{
    public class UriQueryCollection : IUriQueryCollection
    {
        private bool _isCacheValid;
        private string _cache;
        private readonly IDictionary<string, ISet<string>> _inner;

        protected UriQueryCollection(IDictionary<string, ISet<string>> inner)
        {
            _inner = inner;
        }

        public UriQueryCollection()
            :this(new Dictionary<string, ISet<string>>(StringComparer.OrdinalIgnoreCase)) { }

        public UriQueryCollection(IReadonlyUriQueryCollection collection)
            : this(new Dictionary<string, ISet<string>>(collection.ToDictionary(), StringComparer.OrdinalIgnoreCase)) { }
        

        public IEnumerable<string> this[string name]
        {
            get
            {
                if (_inner.ContainsKey(name))
                    return _inner[name];

                return Enumerable.Empty<string>();
            }
            set { Set(name, value); }
        }

        public IUriQueryCollection Set(string name, string value)
        {
            return Set(name, new[] { value });
        }

        public IUriQueryCollection Set(string name, IEnumerable<string> values)
        {
            if (string.IsNullOrWhiteSpace(name))
                return this;

            lock (_inner)
            {
                _inner[name] = CreateItem(values);
                _isCacheValid = false;
            }

            return this;
        }

        public IUriQueryCollection Set(IEnumerable<KeyValuePair<string, string>> values)
        {
            if (values == null)
                return this;

            var lookup = values.ToLookup(p => p.Key, p => p.Value);

            foreach (var group in lookup)
                Set(group.Key, group.ToList());

            return this;
        }

        public IUriQueryCollection Add(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                return this;

            AddIfNeeded(name);

            if (!_inner[name].Contains(value))
            {
                lock (_inner[name])
                {
                    if (!_inner[name].Contains(value))
                    {
                        _inner[name].Add(value);
                        _isCacheValid = false;
                    }
                }
            }

            return this;
        }

        public IUriQueryCollection Add(string name, IEnumerable<string> values)
        {
            if (string.IsNullOrWhiteSpace(name))
                return this;

            AddIfNeeded(name);

            foreach (var value in values)
                Add(name, value);

            return this;
        }

        public IUriQueryCollection Add(IEnumerable<KeyValuePair<string, string>> values)
        {
            if (values == null)
                return this;

            foreach (var pair in values)
                Add(pair.Key, pair.Value);

            return this;
        }

        protected virtual ISet<string> CreateItem(IEnumerable<string> values)
        {
            return new HashSet<string>(values, StringComparer.Ordinal);
        }

        private void AddIfNeeded(string name)
        {
            if (!_inner.ContainsKey(name))
            {
                lock (_inner)
                {
                    if (!_inner.ContainsKey(name))
                    {
                        _inner[name] = CreateItem(Enumerable.Empty<string>());
                        _isCacheValid = false;
                    }
                }
            }
        }

        public string ToEncodedString()
        {
            if (!_isCacheValid)
            {
                lock (_inner)
                {
                    var sb = new StringBuilder();
                    foreach (var item in _inner)
                    {
                        foreach (var value in item.Value)
                        {
                            if (sb.Length != 0)
                                sb.Append("&");

                            sb.Append(Uri.EscapeDataString(item.Key));
                            sb.Append("=");
                            sb.Append(Uri.EscapeDataString(value ?? string.Empty));
                        }
                    }

                    _cache = sb.ToString();
                    _isCacheValid = true;
                }
            }

            return _cache;
        }

        public virtual IDictionary<string, ISet<string>> ToDictionary()
        {
            return new Dictionary<string, ISet<string>>(_inner, StringComparer.OrdinalIgnoreCase);
        }

        public IUriQueryCollection Clear()
        {
            _inner.Clear();

            return this;
        }

        //implemented from of System.Web.HttpValueCollection.FillFromString
        public static UriQueryCollection FromQueryString(string queryString)
        {
            var collection = new UriQueryCollection();

            collection.FillFromString(queryString);

            return collection;
        }

        protected void FillFromString(string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
                return;

            var length = queryString.Length;
            var currentIndex = queryString.IndexOf('?') + 1;

            while (currentIndex < length)
            {
                var startIndex = currentIndex;
                var splitIndex = -1;

                while (currentIndex < length)
                {
                    var currentCharacter = queryString[currentIndex];

                    if (currentCharacter == '=')
                    {
                        if (splitIndex < 0)
                            splitIndex = currentIndex;
                    }
                    else if (currentCharacter == '&')
                    {
                        break;
                    }

                    currentIndex++;
                }

                string name;
                string value = null;

                if (splitIndex >= 0)
                {
                    name = queryString.Substring(startIndex, splitIndex - startIndex);
                    value = queryString.Substring(splitIndex + 1, currentIndex - splitIndex - 1);
                }
                else
                {
                    name = queryString.Substring(startIndex, currentIndex - startIndex);
                }

                // add name / value pair to the collection
                Add(Uri.UnescapeDataString(name), Uri.UnescapeDataString(value ?? string.Empty));

                currentIndex++;
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _inner.SelectMany(kp => kp.Value.Select(v => new KeyValuePair<string, string>(kp.Key, v))).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}