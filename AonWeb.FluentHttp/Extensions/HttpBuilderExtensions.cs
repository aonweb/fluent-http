using System;
using System.Net.Http;
using System.Text;

namespace AonWeb.FluentHttp
{
    public static class HttpBuilderExtensions
    {
        public static IHttpBuilder AsGet(this IHttpBuilder builder)
        {
            builder.Advanced.WithMethod(HttpMethod.Get);

            return builder;
        }

        public static IHttpBuilder AsPut(this IHttpBuilder builder)
        {
            return builder.Advanced.WithMethod(HttpMethod.Put);
        }

        public static IHttpBuilder AsPost(this IHttpBuilder builder)
        {
            return builder.Advanced.WithMethod(HttpMethod.Post);
        }

        public static IHttpBuilder AsDelete(this IHttpBuilder builder)
        {
            return builder.Advanced.WithMethod(HttpMethod.Delete);
        }

        public static IHttpBuilder AsPatch(this IHttpBuilder builder)
        {
            return builder.Advanced.WithMethod(new HttpMethod("PATCH"));
        }

        public static IHttpBuilder AsHead(this IHttpBuilder builder)
        {
            return builder.Advanced.WithMethod(HttpMethod.Head);
        }

        public static IHttpBuilder WithContent(this IHttpBuilder builder, string content)
        {
            return builder.WithContent(content, null, null);
        }

        public static IHttpBuilder WithContent(this IHttpBuilder builder, string content, Encoding encoding)
        {
            return builder.WithContent(content, encoding, null);
        }

        public static IHttpBuilder WithContent(this IHttpBuilder builder, string content, Encoding encoding,
            string mediaType)
        {
            return builder.WithContent(() => content, encoding, mediaType);
        }

        public static IHttpBuilder WithContent(this IHttpBuilder builder, Func<string> contentFactory)
        {
            return builder.WithContent(contentFactory, null, null);
        }

        public static IHttpBuilder WithContent(this IHttpBuilder builder, Func<string> contentFactory, Encoding encoding)
        {
            return builder.WithContent(contentFactory, encoding, null);
        }

        public static IHttpBuilder WithContent(this IHttpBuilder builder, Func<string> contentFactory, Encoding encoding,
            string mediaType)

        {
            if (contentFactory == null)
                throw new ArgumentNullException(nameof(contentFactory));

            if (encoding != null)
                builder.Advanced.WithContentEncoding(encoding);

            if (string.IsNullOrWhiteSpace(mediaType))
                builder.Advanced.WithMediaType(mediaType);

            return builder.WithContent(() =>
            {
                var content = contentFactory() ?? string.Empty;

                if (!string.IsNullOrEmpty(content))
                    return new StringContent(content, encoding, mediaType);

                return null;
            });
        }

        public static IHttpBuilder WithContent(this IHttpBuilder builder, Func<HttpContent> contentFactory)
        {
            if (contentFactory == null)
                throw new ArgumentNullException(nameof(contentFactory));

            return builder.WithConfiguration(s => s.ContentFactory = contentFactory);
        }
    }
}