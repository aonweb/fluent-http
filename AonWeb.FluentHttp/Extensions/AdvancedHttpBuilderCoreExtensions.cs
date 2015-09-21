using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace AonWeb.FluentHttp
{
    public static class AdvancedHttpBuilderCoreExtensions
    {
        public static TBuilder WithContentEncoding<TBuilder>(this TBuilder builder, Encoding encoding)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>, IAdvancedHttpBuilderCore<TBuilder>
        {
            if (encoding == null)
                return builder;

            builder.WithConfiguration(s => s.ContentEncoding = encoding);
            builder.WithAcceptCharSet(encoding);

            return builder;
        }

        public static TBuilder WithHeadersConfiguration<TBuilder>(this TBuilder builder, Action<HttpRequestHeaders> configuration)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>, IAdvancedHttpBuilderCore<TBuilder>
        {
            return builder.WithClientConfiguration(c => c.WithHeadersConfiguration(configuration));
        }

        public static TBuilder WithHeader<TBuilder>(this TBuilder builder, string name, string value)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>, IAdvancedHttpBuilderCore<TBuilder>
        {
            return builder.WithClientConfiguration(c => c.WithHeader(name, value));
        }

        public static TBuilder WithHeader<TBuilder>(this TBuilder builder, string name, IEnumerable<string> values)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>, IAdvancedHttpBuilderCore<TBuilder>
        {
            return builder.WithClientConfiguration(c => c.WithHeader(name, values));
        }

        public static TBuilder WithAppendHeader<TBuilder>(this TBuilder builder, string name, string value)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>, IAdvancedHttpBuilderCore<TBuilder>
        {
            return builder.WithClientConfiguration(c => c.WithAppendHeader(name, value));
        }

        public static TBuilder WithAppendHeader<TBuilder>(this TBuilder builder, string name, IEnumerable<string> values)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>, IAdvancedHttpBuilderCore<TBuilder>
        {
            return builder.WithClientConfiguration(c => c.WithAppendHeader(name, values));
        }

        public static TBuilder WithMediaType<TBuilder>(this TBuilder builder, string mediaType)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>, IAdvancedHttpBuilderCore<TBuilder>
        {
            if (mediaType == null)
                return builder;

            builder.WithConfiguration(s => s.MediaType = mediaType);
            builder.WithAcceptHeaderValue(mediaType);

            return builder;
        }

        public static TBuilder WithAcceptHeaderValue<TBuilder>(this TBuilder builder, string mediaType)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>, IAdvancedHttpBuilderCore<TBuilder>
        {
            return builder.WithHeadersConfiguration(h => h.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType)));
        }

        public static TBuilder WithAcceptCharSet<TBuilder>(this TBuilder builder, Encoding encoding)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>, IAdvancedHttpBuilderCore<TBuilder>
        {
            return builder.WithAcceptCharSet(encoding.WebName);
        }

        public static TBuilder WithAcceptCharSet<TBuilder>(this TBuilder builder, string charSet)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>, IAdvancedHttpBuilderCore<TBuilder>
        {
            return builder.WithHeadersConfiguration(h => h.AcceptCharset.Add(new StringWithQualityHeaderValue(charSet)));
        }

        public static TBuilder WithAutoDecompression<TBuilder>(this TBuilder builder, bool enabled = true)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>, IAdvancedHttpBuilderCore<TBuilder>
        {
            builder.WithConfiguration(s => s.AutoDecompression = true);

            return builder.WithClientConfiguration(c =>
                c.WithDecompressionMethods(enabled
                    ? Defaults.Client.DecompressionMethods.HasValue && !Defaults.Client.DecompressionMethods.Value.HasFlag(DecompressionMethods.None)
                        ? Defaults.Client.DecompressionMethods.Value
                        : DecompressionMethods.GZip | DecompressionMethods.Deflate
                    : DecompressionMethods.None));
        }

        public static TBuilder WithTimeout<TBuilder>(this TBuilder builder, TimeSpan? timeout)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>, IAdvancedHttpBuilderCore<TBuilder>
        {
            return builder.WithClientConfiguration(c => c.WithTimeout(timeout));
        }

        public static TBuilder WithNoCache<TBuilder>(this TBuilder builder, bool nocache = true)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>, IAdvancedHttpBuilderCore<TBuilder>
        {
            return builder.WithClientConfiguration(c => c.WithNoCache(nocache));
        }

        public static TBuilder WithSuppressCancellationExceptions<TBuilder>(this TBuilder builder, bool suppress = true)
            where TBuilder : IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings>, IAdvancedHttpBuilderCore<TBuilder>
        {
            builder.WithConfiguration(s => s.SuppressCancellationErrors = suppress);

            return builder;
        }
    }
}