using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public class HttpCallFormatter<TResult, TContent, TError> : IHttpCallFormatter<TResult, TContent, TError>
    {
        public async Task<HttpContent> CreateContent<T>(T value, HttpCallContext<TResult, TContent, TError> context)
        {
            var type = typeof(T);
            var mediaType = context.MediaType;
            var header = new MediaTypeHeaderValue(mediaType);
            var formatter = context.MediaTypeFormatters.FindWriter(type, header);

            if (formatter == null)
                throw new ArgumentException(String.Format(SR.NoFormatterForMimeTypeErrorFormat, mediaType));

            HttpContent content;
            using (var stream = new MemoryStream())
            {
                await formatter.WriteToStreamAsync(type, value, stream, null, null);

                content = new ByteArrayContent(stream.ToArray());
            }

            formatter.SetDefaultContentHeaders(type, content.Headers, header);

            return content;
        }

        public Task<TResult> DeserializeResult(HttpResponseMessage response, HttpCallContext<TResult, TContent, TError> context)
        {
            return DeserializeResponse<TResult>(response, context);
        }

        public Task<TError> DeserializeError(HttpResponseMessage response, HttpCallContext<TResult, TContent, TError> context)
        {
            return DeserializeResponse<TError>(response, context);
        }

        private static async Task<T> DeserializeResponse<T>(HttpResponseMessage response, HttpCallContext<TResult, TContent, TError> context)
        {
            if (typeof(HttpResponseMessage).IsAssignableFrom(typeof(T))) 
                return (T)(object)response; // ugh :(

            if (response.Content == null) 
                return default(T);

            if (typeof(Stream).IsAssignableFrom(typeof(T))) 
                return (T)(object)await response.Content.ReadAsStreamAsync(); // ugh :(

            if (typeof(byte[]).IsAssignableFrom(typeof(T)))
                return (T)(object)await response.Content.ReadAsByteArrayAsync(); // ugh :(

            return await response.Content.ReadAsAsync<T>(context.MediaTypeFormatters);
        }
    }
}