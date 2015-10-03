using System;
using System.Collections.Generic;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp
{
    public class HttpUriBuilder
    {
        private UriBuilder _builder = new UriBuilder();
        private NormalizedUriQueryCollection _query = new NormalizedUriQueryCollection();

        public HttpUriBuilder()
        {
            IsSet = false;
        }

        public bool IsSet { get; private set; }

        public Uri Uri
        {
            get
            {
                if (_builder.Query.TrimStart('?') != _query.ToEncodedString())
                    _builder.Query = _query.ToEncodedString();

                return _builder.Uri;
            }
            set
            {
                _builder = new UriBuilder(value);
                _query = NormalizedUriQueryCollection.FromQueryString(_builder.Query);

                IsSet = true;
            }
        }

        public string Scheme
        {
            get { return _builder.Scheme; }
            set
            {
                _builder.Scheme = value;
                IsSet = true;
            }
        }

        public string Host
        {
            get { return _builder.Host; }
            set
            {
                _builder.Host = value;
                IsSet = true;
            }
        }
        public int Port
        {
            get { return _builder.Port; }
            set
            {
                _builder.Port = value;
                IsSet = true;
            }
        }
        public string Path
        {
            get { return _builder.Path; }
            set
            {
                SetPath(value);
                IsSet = true;
            }
        }

        public string Fragment
        {
            get { return _builder.Fragment; }
            set
            {
                _builder.Fragment = value;
                IsSet = true;
            }
        }

        public string UserName
        {
            get { return _builder.UserName; }
            set
            {
                _builder.UserName = value;
                IsSet = true;
            }
        }

        public string Password
        {
            get { return _builder.Password; }
            set
            {
                _builder.Password = value;
                IsSet = true;
            }
        }

        public IReadonlyUriQueryCollection Query
        {
            get { return _query; }
            set
            {
                _query = new NormalizedUriQueryCollection(value);
                IsSet = true;
            }
        }

        public string QueryString
        {
            get { return _query.ToEncodedString(); }
            set
            {
                _query = NormalizedUriQueryCollection.FromQueryString(value);
                IsSet = true;
            }
        }

        public HttpUriBuilder WithQueryString(string name, string value)
        {
            _query.Set(name, value);
            IsSet = true;
            return this;
        }

        public HttpUriBuilder WithQueryString(IEnumerable<KeyValuePair<string, string>> values)
        {
            _query.Set(values);
            IsSet = true;
            return this;
        }

        public HttpUriBuilder WithAppendQueryString(string name, string value)
        {
            _query.Add(name, value);
            IsSet = true;
            return this;
        }

        public HttpUriBuilder WithAppendQueryString(IEnumerable<KeyValuePair<string, string>> values)
        {
            _query.Add(values);
            IsSet = true;
            return this;
        }

        private void SetPath(string pathAndQuery)
        {
            if (string.IsNullOrEmpty(pathAndQuery))
            {
                pathAndQuery = "/";
            }

            if (!UriHelpers.IsAbsolutePath(pathAndQuery))
                pathAndQuery = UriHelpers.CombineVirtualPaths(_builder.Path, pathAndQuery);

            if (!UriHelpers.IsAbsolutePath(pathAndQuery))
                throw new ArgumentException(SR.ArgumentPathMustBeAbsoluteError, nameof(pathAndQuery));

            var index = pathAndQuery.IndexOf("?", StringComparison.Ordinal);

            if (index > -1)
            {
                QueryString = pathAndQuery.Substring(index + 1);
                pathAndQuery = pathAndQuery.Substring(0, index);
            }

            _builder.Path = pathAndQuery;
        }
    }
}