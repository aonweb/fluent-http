using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.HAL.Serialization;

namespace AonWeb.FluentHttp.HAL
{
    public static class HalBuilderExtensions
    {
        public static IHalBuilder WithLink(this IHalBuilder builder, string link)
        {
            return builder.Advanced.WithConfiguration(b => b.WithUri(link));
        }

        public static IHalBuilder WithLink(this IHalBuilder builder, Uri link)
        {
            return builder.Advanced.WithConfiguration(b => b.WithUri(link));
        }

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

        public static IHalBuilder WithQueryString(this IHalBuilder builder, string name, string value)
        {
            return builder.Advanced.WithConfiguration(b => b.WithQueryString(name, value));
        }

        public static IHalBuilder WithQueryString(this IHalBuilder builder, IEnumerable<KeyValuePair<string, string>> values)
        {
            return builder.Advanced.WithConfiguration(b => b.WithQueryString(values));
        }

        public static IHalBuilder WithAppendQueryString(this IHalBuilder builder, string name, string value)
        {
            return builder.Advanced.WithConfiguration(b => b.WithAppendQueryString(name, value));
        }

        public static IHalBuilder WithAppendQueryString(this IHalBuilder builder, IEnumerable<KeyValuePair<string, string>> values)
        {
            return builder.Advanced.WithConfiguration(b => b.WithAppendQueryString(values));
        }

        public static IHalBuilder WithOptionalQueryString<TValue>(this IHalBuilder builder, string name, TValue value, Func<TValue, bool> nullCheck = null, Func<TValue, string> toString = null)
        {
            return builder.Advanced.WithConfiguration(b => b.WithOptionalQueryString(name, value, nullCheck, toString));
        }

        public static IHalBuilder WithAppendOptionalQueryString<TValue>(this IHalBuilder builder, string name, TValue value, Func<TValue, bool> nullCheck = null, Func<TValue, string> toString = null)
        {
            return builder.Advanced.WithConfiguration(b => b.WithAppendOptionalQueryString(name, value, nullCheck, toString));
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
            return builder.Advanced.WithConfiguration(b => 
            {
                Func<TContent> wrapper = () =>
                {
                    var content = contentFactory();

                    if (!ReferenceEquals(content, null))
                        b.WithDependentUris(content.DependentUris);

                    return content;
                };


                b.WithContent(wrapper, encoding, mediaType);
            });
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