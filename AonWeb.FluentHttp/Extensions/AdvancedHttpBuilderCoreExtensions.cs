using System;
using System.Net.Http;

namespace AonWeb.FluentHttp
{
    public static class AdvancedHttpBuilderCoreExtensions
    {
        public static TBuilder WithScheme<TBuilder>(this IAdvancedHttpBuilderCore<TBuilder> builder, string scheme)
            where TBuilder : IAdvancedHttpBuilderCore<TBuilder>
        {
            if (string.IsNullOrEmpty(scheme))
                throw new ArgumentNullException(nameof(scheme));

            builder.WithConfiguration(s => s.UriBuilder.Scheme = scheme);

            return (TBuilder)builder;
        }

        public static TBuilder WithHost<TBuilder>(this IAdvancedHttpBuilderCore<TBuilder> builder, string host)
            where TBuilder : IAdvancedHttpBuilderCore<TBuilder>
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException(nameof(host));

            builder.WithConfiguration(s => s.UriBuilder.Host = host);

            return (TBuilder)builder;
        }

        public static TBuilder WithPort<TBuilder>(this IAdvancedHttpBuilderCore<TBuilder> builder, int port)
            where TBuilder : IAdvancedHttpBuilderCore<TBuilder>
        {
            builder.WithConfiguration(s => s.UriBuilder.Port = port);

            return (TBuilder)builder;
        }

        public static TBuilder WithMethod<TBuilder>(this IAdvancedHttpBuilderCore<TBuilder> builder, string method)
            where TBuilder : IAdvancedHttpBuilderCore<TBuilder>
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException(SR.ArgumentMethodNullOrEmptyError, nameof(method));

            return (TBuilder)builder.WithMethod(new HttpMethod(method.ToUpper()));
        }

        public static TBuilder WithMethod<TBuilder>(this IAdvancedHttpBuilderCore<TBuilder> builder, HttpMethod method)
            where TBuilder : IAdvancedHttpBuilderCore<TBuilder>
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            builder.WithConfiguration(s => s.Method = method);

            return (TBuilder)builder;
        }
    }
}