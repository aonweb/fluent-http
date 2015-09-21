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

        public UriQueryCollection()
        {
            _inner = new SortedDictionary<string, ISet<string>>(StringComparer.OrdinalIgnoreCase);
        }

        public UriQueryCollection(IReadonlyUriQueryCollection collection)
        {
            _inner = collection.ToDictionary();
        }

        public UriQueryCollection(string queryString)
            : this()
        {
            FillFromString(queryString);
        }

        public IUriQueryCollection Add(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                return this;

            if (!_inner.ContainsKey(name))
            {
                lock (_inner)
                {
                    if (!_inner.ContainsKey(name))
                    {
                        _inner[name] = new SortedSet<string>(StringComparer.Ordinal);
                        _isCacheValid = false;
                    }
                }
            }

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

        public IUriQueryCollection Add(IEnumerable<KeyValuePair<string, string>> values)
        {
            if (values == null)
                return this;

            foreach (var pair in values)
                Add(pair.Key, pair.Value);

            return this;
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

        public IDictionary<string, ISet<string>> ToDictionary()
        {
            return new SortedDictionary<string, ISet<string>>(_inner, StringComparer.OrdinalIgnoreCase);
        }

        public IUriQueryCollection Clear()
        {
            _inner.Clear();

            return this;
        }

        //implemented from of System.Web.HttpValueCollection.FillFromString
        public static UriQueryCollection ParseQueryString(string queryString)
        {
            var collection = new UriQueryCollection();

            collection.FillFromString(queryString);

            return collection;
        }

        private void FillFromString(string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
                return;

            var length = queryString.Length;
            var currentIndex = 0;

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