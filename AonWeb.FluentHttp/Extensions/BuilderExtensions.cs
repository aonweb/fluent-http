using System;
using System.Collections.Generic;

namespace AonWeb.FluentHttp
{
    public static class BuilderExtensions
    {
        public static TBuilder WithUri<TBuilder>(this TBuilder builder, string uri)
            where TBuilder : IConfigurable<IHttpBuilderSettings>
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentException(SR.ArgumentUriNullOrEmptyError, nameof(uri));

            return builder.WithUri(new Uri(uri));
        }

        public static TBuilder WithUri<TBuilder>(this TBuilder builder, Uri uri)
            where TBuilder : IConfigurable<IHttpBuilderSettings>
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            builder.WithConfiguration(s =>
            {
                s.UriBuilder.Uri = uri;
            });

            return builder;
        }

        public static TBuilder WithBaseUri<TBuilder>(this TBuilder builder, string uri)
            where TBuilder : IConfigurable<IHttpBuilderSettings>
        {
            return builder.WithUri(uri);
        }

        public static TBuilder WithBaseUri<TBuilder>(this TBuilder builder, Uri uri)
            where TBuilder : IConfigurable<IHttpBuilderSettings>
        {
            return builder.WithUri(uri);
        }

        public static TBuilder WithPath<TBuilder>(this TBuilder builder, string pathAndQuery)
            where TBuilder : IConfigurable<IHttpBuilderSettings>
        {
            builder.WithConfiguration(s =>
            {
                s.UriBuilder.Path = pathAndQuery;
            });

            return builder;
        }

        public static TBuilder WithQueryString<TBuilder>(this TBuilder builder, string name, string value)
            where TBuilder : IConfigurable<IHttpBuilderSettings>
        {
            builder.WithConfiguration(s =>
            {
                s.UriBuilder.WithQueryString(name, value);
            });

            return builder;
        }

        public static TBuilder WithQueryString<TBuilder>(this TBuilder builder, IEnumerable<KeyValuePair<string, string>> values)
            where TBuilder : IConfigurable<IHttpBuilderSettings>
        {
            builder.WithConfiguration(s =>
            {
                s.UriBuilder.WithQueryString(values);
            });

            return builder;
        }

        public static TBuilder WithOptionalQueryString<TBuilder, TValue>(this TBuilder builder, string name, TValue value, Func<TValue, bool> nullCheck = null, Func<TValue, string> toString = null)
             where TBuilder : IConfigurable<IHttpBuilderSettings>
        {
            builder.WithConfiguration(s =>
            {
                if (nullCheck == null)
                    nullCheck = v => value == null;

                if (toString == null)
                    toString = v => v.ToString();

                if (nullCheck(value))
                    return;

                s.UriBuilder.WithQueryString(name, toString(value));
            });

            return builder;
        }

    }
}