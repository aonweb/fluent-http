using System;
using System.Net.Http;
using System.Text;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.HAL
{
    public static class HalBuilderExtensions
    {
        public static IHalBuilder AsGet(this IHalBuilder builder)
        {
            return builder.Advanced.WithMethod(HttpMethod.Get);
        }

        public static IHalBuilder AsPut(this IHalBuilder builder)
        {
            return builder.Advanced.WithMethod(HttpMethod.Put);
        }

        public static IHalBuilder AsPost(this IHalBuilder builder)
        {
            return builder.Advanced.WithMethod(HttpMethod.Post);
        }

        public static IHalBuilder AsDelete(this IHalBuilder builder)
        {
            return builder.Advanced.WithMethod(HttpMethod.Delete);
        }

        public static IHalBuilder AsPatch(this IHalBuilder builder)
        {
            return builder.Advanced.WithMethod(new HttpMethod("PATCH"));
        }

        public static IHalBuilder WithOptionalQueryString<TValue>(this IHalBuilder builder, string name, TValue value, Func<TValue, bool> nullCheck = null, Func<TValue, string> toString = null)
        {
            return builder.Advanced.WithConfiguration(b => b.WithOptionalQueryString(name, value, nullCheck, toString));
        }

        public static IHalBuilder WithContent<TContent>(this IHalBuilder builder, TContent content)
             where TContent : IHalRequest
        {
            return builder.WithContent(content, null, null);
        }

        public static IHalBuilder WithContent<TContent>(this IHalBuilder builder, TContent content, Encoding encoding)
            where TContent : IHalRequest
        {
            return builder.WithContent(content, encoding, null);
        }

        public static IHalBuilder WithContent<TContent>(this IHalBuilder builder, TContent content, Encoding encoding, string mediaType)
            where TContent : IHalRequest
        {
            return builder.WithContent(() => content, encoding, mediaType);
        }

        public static IHalBuilder WithContent<TContent>(this IHalBuilder builder, Func<TContent> contentFactory)
             where TContent : IHalRequest
        {
            return builder.WithContent(contentFactory, null, null);
        }

        public static IHalBuilder WithContent<TContent>(this IHalBuilder builder, Func<TContent> contentFactory, Encoding encoding)
            where TContent : IHalRequest
        {
            return builder.WithContent(contentFactory, encoding, null);
        }

        public static IHalBuilder WithContent<TContent>(this IHalBuilder builder, Func<TContent> contentFactory, Encoding encoding, string mediaType)
            where TContent : IHalRequest
        {
            builder.Advanced.WithConfiguration(b => b.WithContent(contentFactory, encoding, mediaType));

            return builder;
        }

        public static IHalBuilder WithDefaultResult<TResult>(this IHalBuilder builder, TResult result)
            where TResult : IHalResource
        {
            builder.Advanced.WithConfiguration(b => b.WithDefaultResult(() => result));

            return builder;
        }

        public static IHalBuilder WithDefaultResult<TResult>(this IHalBuilder builder, Func<TResult> resultFactory)
             where TResult : IHalResource
        {
            builder.Advanced.WithConfiguration(b => b.WithDefaultResult(resultFactory));

            return builder;
        }

        public static IHalBuilder WithErrorType<TError>(this IHalBuilder builder)
        {
            return builder.WithConfiguration(s => s.WithDefiniteErrorType(typeof(TError)));
        }
    }
}