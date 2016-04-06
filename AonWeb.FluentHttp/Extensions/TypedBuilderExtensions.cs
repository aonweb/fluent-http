using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp
{
    public static class TypedBuilderExtensions
    {
        public static  Task<TResult> ResultAsync<TResult>(this ITypedBuilder builder)
        {
            return builder.ResultAsync<TResult>(CancellationToken.None);
        }

        public static Task SendAsync(this ITypedBuilder builder)
        {
            return builder.SendAsync(CancellationToken.None);
        }

        public static ITypedBuilder AsGet(this ITypedBuilder builder)
        {
            return builder.Advanced.WithMethod(HttpMethod.Get);
        }

        public static ITypedBuilder AsPut(this ITypedBuilder builder)
        {
            return builder.Advanced.WithMethod(HttpMethod.Put);
        }

        public static ITypedBuilder AsPost(this ITypedBuilder builder)
        {
            return builder.Advanced.WithMethod(HttpMethod.Post);
        }

        public static ITypedBuilder AsDelete(this ITypedBuilder builder)
        {
            return builder.Advanced.WithMethod(HttpMethod.Delete);
        }

        public static ITypedBuilder AsPatch(this ITypedBuilder builder)
        {
            return builder.Advanced.WithMethod(new HttpMethod("PATCH"));
        }

        public static ITypedBuilder WithContent<TContent>(this ITypedBuilder builder, TContent content)
        {

            return builder.WithContent(content, null, null);
        }

        public static ITypedBuilder WithContent<TContent>(this ITypedBuilder builder, TContent content, Encoding encoding)
        {

            return builder.WithContent(content, encoding, null);
        }

        public static ITypedBuilder WithContent<TContent>(this ITypedBuilder builder, TContent content, Encoding encoding, string mediaType)
        {

            return builder.WithContent(() => content, encoding, mediaType);
        }

        public static ITypedBuilder WithContent<TContent>(this ITypedBuilder builder, Func<TContent> contentFactory)
        {
            return builder.WithContent(contentFactory, null, null);
        }

        public static ITypedBuilder WithContent<TContent>(this ITypedBuilder builder, Func<TContent> contentFactory, Encoding encoding)
        {
            return builder.WithContent(contentFactory, encoding, null);
        }

        public static ITypedBuilder WithContent<TContent>(this ITypedBuilder builder, Func<TContent> contentFactory, Encoding encoding, string mediaType)
        {
            if (contentFactory == null)
                throw new ArgumentNullException(nameof(contentFactory));

            builder.WithConfiguration(s =>
            {
                s.WithDefiniteContentType(typeof (TContent));

                if (!typeof(IEmptyRequest).IsAssignableFrom(typeof(TContent)))
                    s.ContentFactory = () => contentFactory();
            });

            builder.Advanced.WithContentEncoding(encoding);
            builder.Advanced.WithMediaType(mediaType);

            return builder;
        }

        public static ITypedBuilder WithDefaultResult<TResult>(this ITypedBuilder builder, TResult result)
        {
            return builder.WithDefaultResult(() => result);
        }

        public static ITypedBuilder WithDefaultResult<TResult>(this ITypedBuilder builder, Func<TResult> resultFactory)
        {
            return builder.WithConfiguration(
                s => s.WithResultType(typeof (TResult)).DefaultResultFactory = t => resultFactory());
        }

        public static ITypedBuilder WithErrorType<TError>(this ITypedBuilder builder)
        {
            return builder.WithConfiguration(s => s.WithDefiniteErrorType(typeof(TError)));
        }
    }
}