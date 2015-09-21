using System;
using System.Net.Http;

namespace AonWeb.FluentHttp
{
    public static class AdvancedBuilderExtensions
    {
        public static TBuilder WithScheme<TBuilder>(this TBuilder builder, string scheme)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>
        {
            if (string.IsNullOrEmpty(scheme))
                throw new ArgumentNullException(nameof(scheme));

            builder.WithConfiguration(s => s.UriBuilder.Scheme = scheme);

            return builder;
        }

        public static TBuilder WithHost<TBuilder>(this TBuilder builder, string host)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException(nameof(host));

            builder.WithConfiguration(s => s.UriBuilder.Host = host);

            return builder;
        }

        public static TBuilder WithPort<TBuilder>(this TBuilder builder, int port)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>
        {
            builder.WithConfiguration(s => s.UriBuilder.Port = port);

            return builder;
        }

        public static TBuilder WithMethod<TBuilder>(this TBuilder builder, string method)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentException(SR.ArgumentMethodNullOrEmptyError, nameof(method));

            return builder.WithMethod(new HttpMethod(method.ToUpper()));
        }

        public static TBuilder WithMethod<TBuilder>(this TBuilder builder, HttpMethod method)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            builder.WithConfiguration(s => s.Method = method);

            return builder;
        }
    }
}