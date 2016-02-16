using System;
using System.Collections.Generic;

namespace AonWeb.FluentHttp.Serialization
{
    public static class HttpUriBuilderExtensions
    {
        public static HttpUriBuilder WithUri(this HttpUriBuilder builder, string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentException("Uri must not be null or empty.", nameof(uri));

            return builder.WithUri(new Uri(uri));
        }

        public static HttpUriBuilder WithUri(this HttpUriBuilder builder, Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));


            builder.Uri = uri;

            return builder;
        }

        public static HttpUriBuilder WithBaseUri(this HttpUriBuilder builder, string uri)
        {
            return builder.WithUri(uri);
        }

        public static HttpUriBuilder WithBaseUri(this HttpUriBuilder builder, Uri uri)
        {
            return builder.WithUri(uri);
        }

        public static HttpUriBuilder WithPath(this HttpUriBuilder builder, string pathAndQuery)
        {

            builder.Path = pathAndQuery;

            return builder;
        }

        public static HttpUriBuilder WithQueryString(this HttpUriBuilder builder, IEnumerable<KeyValuePair<string, IEnumerable<string>>> values)
        {
            return builder.WithQueryConfiguration(query => query.Set(values));
        }

        public static HttpUriBuilder WithQueryString(this HttpUriBuilder builder, string name, string value)
        {

            return builder.WithQueryConfiguration(query => query.Set(name, value));
        }

        public static HttpUriBuilder WithQueryString(this HttpUriBuilder builder, IEnumerable<KeyValuePair<string, string>> values)
        {
            return builder.WithQueryConfiguration(query => query.Set(values));
        }

        public static HttpUriBuilder WithAppendQueryString(this HttpUriBuilder builder, IEnumerable<KeyValuePair<string, IEnumerable<string>>> values)
        {
            return builder.WithQueryConfiguration(query => query.Add(values));
        }

        public static HttpUriBuilder WithAppendQueryString(this HttpUriBuilder builder, string name, string value)
        {
            return builder.WithQueryConfiguration(query => query.Add(name, value));
        }

        public static HttpUriBuilder WithAppendQueryString(this HttpUriBuilder builder, IEnumerable<KeyValuePair<string, string>> values)
        {
            return builder.WithQueryConfiguration(query => query.Add(values));
        }

        public static HttpUriBuilder WithOptionalQueryString<TValue>(this HttpUriBuilder builder, string name, TValue value, Func<TValue, bool> nullCheck = null, Func<TValue, string> toString = null)
        {

            if (nullCheck == null)
                nullCheck = v => value == null;

            if (toString == null)
                toString = v => v.ToString();

            if (nullCheck(value))
                return builder;

            builder.WithQueryString(name, toString(value));


            return builder;
        }

        public static HttpUriBuilder WithAppendOptionalQueryString<TValue>(this HttpUriBuilder builder, string name, TValue value, Func<TValue, bool> nullCheck = null, Func<TValue, string> toString = null)
        {

            if (nullCheck == null)
                nullCheck = v => value == null;

            if (toString == null)
                toString = v => v.ToString();

            if (nullCheck(value))
                return builder;

            builder.WithAppendQueryString(name, toString(value));


            return builder;
        }
    }
}