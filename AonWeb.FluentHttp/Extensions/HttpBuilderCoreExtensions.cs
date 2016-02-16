using System;
using System.Collections.Generic;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp
{
    public static class HttpBuilderCoreExtensions
    {
        public static TBuilder WithUri<TBuilder>(this IHttpBuilderCore<TBuilder> builder, string uri)
            where TBuilder : IHttpBuilderCore<TBuilder>
        {
            builder.WithConfiguration(s =>
            {
                s.UriBuilder.WithUri(uri);
            });

            return (TBuilder)builder;
        }

        public static TBuilder WithUri<TBuilder>(this IHttpBuilderCore<TBuilder> builder, Uri uri)
            where TBuilder : IHttpBuilderCore<TBuilder>
        {
            builder.WithConfiguration(s =>
            {
                s.UriBuilder.WithUri(uri);
            });

            return (TBuilder)builder;
        }

        public static TBuilder WithBaseUri<TBuilder>(this IHttpBuilderCore<TBuilder> builder, string uri)
            where TBuilder : IHttpBuilderCore<TBuilder>
        {
            return builder.WithUri(uri);
        }

        public static TBuilder WithBaseUri<TBuilder>(this IHttpBuilderCore<TBuilder> builder, Uri uri)
            where TBuilder : IHttpBuilderCore<TBuilder>
        {
            return builder.WithUri(uri);
        }

        public static TBuilder WithPath<TBuilder>(this IHttpBuilderCore<TBuilder> builder, string pathAndQuery)
            where TBuilder : IHttpBuilderCore<TBuilder>
        {
            builder.WithConfiguration(s =>
            {
                s.UriBuilder.WithPath(pathAndQuery);
            });

            return (TBuilder)builder;
        }

        public static TBuilder WithQueryString<TBuilder>(this IHttpBuilderCore<TBuilder> builder, string name, string value)
            where TBuilder : IHttpBuilderCore<TBuilder>
        {
            builder.WithConfiguration(s =>
            {
                s.UriBuilder.WithQueryString(name, value);
            });

            return (TBuilder)builder;
        }

        public static TBuilder WithQueryString<TBuilder>(this IHttpBuilderCore<TBuilder> builder, IEnumerable<KeyValuePair<string, string>> values)
            where TBuilder : IHttpBuilderCore<TBuilder>
        {
            builder.WithConfiguration(s =>
            {
                s.UriBuilder.WithQueryString(values);
            });

            return (TBuilder)builder;
        }

        public static TBuilder WithAppendQueryString<TBuilder>(this IHttpBuilderCore<TBuilder> builder, string name, string value)
            where TBuilder : IHttpBuilderCore<TBuilder>
        {
            builder.WithConfiguration(s =>
            {
                s.UriBuilder.WithAppendQueryString(name, value);
            });

            return (TBuilder)builder;
        }

        public static TBuilder WithAppendQueryString<TBuilder>(this IHttpBuilderCore<TBuilder> builder, IEnumerable<KeyValuePair<string, string>> values)
            where TBuilder : IHttpBuilderCore<TBuilder>
        {
            builder.WithConfiguration(s =>
            {
                s.UriBuilder.WithAppendQueryString(values);
            });

            return (TBuilder)builder;
        }

        public static TBuilder WithOptionalQueryString<TBuilder, TValue>(this IHttpBuilderCore<TBuilder> builder, string name, TValue value, Func<TValue, bool> nullCheck = null, Func<TValue, string> toString = null)
             where TBuilder : IHttpBuilderCore<TBuilder>
        {
            builder.WithConfiguration(s =>
            {
                s.UriBuilder.WithOptionalQueryString(name, value, nullCheck, toString);
            });

            return (TBuilder)builder;
        }

        public static TBuilder WithAppendOptionalQueryString<TBuilder, TValue>(this IHttpBuilderCore<TBuilder> builder, string name, TValue value, Func<TValue, bool> nullCheck = null, Func<TValue, string> toString = null)
             where TBuilder : IHttpBuilderCore<TBuilder>
        {
            builder.WithConfiguration(s =>
            {
                s.UriBuilder.WithAppendOptionalQueryString(name, value, nullCheck, toString);
            });

            return (TBuilder)builder;
        }

    }
}