using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public class HttpCallFormatter : IHttpCallFormatter
    {
        public async Task<HttpContent> CreateContent(object value, TypedHttpCallContext context)
        {
            var type = context.ContentType;
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

        public Task<object> DeserializeResult(HttpResponseMessage response, TypedHttpCallContext context)
        {
            return DeserializeResponse(response, context.ResultType, context.MediaTypeFormatters, context.TokenSource.Token);
        }

        public Task<object> DeserializeError(HttpResponseMessage response, TypedHttpCallContext context)
        {
            return DeserializeResponse(response, context.ErrorType, context.MediaTypeFormatters, context.TokenSource.Token);
        }

        private static async Task<object> DeserializeResponse(HttpResponseMessage response, Type type, MediaTypeFormatterCollection formatters, CancellationToken token)
        {
            if (typeof(HttpResponseMessage).IsAssignableFrom(type)) 
                return response; // ugh :(

            var content = response.Content;

            if (content == null) 
                return Helper.GetDefaultValueForType(type);

            if (typeof(Stream).IsAssignableFrom(type))
                return await content.ReadAsStreamAsync();

            if (typeof(byte[]).IsAssignableFrom(type))
                return await content.ReadAsByteArrayAsync();

            var mediaType = content.Headers.ContentType ?? new MediaTypeHeaderValue("application/octet-stream");

            var formatter = formatters.FindReader(type, mediaType);

            if (formatter == null)
            {
                if (content.Headers.ContentLength == 0)
                    return Helper.GetDefaultValueForType(type);

                throw new UnsupportedMediaTypeException(string.Format("No MediaTypeFormatter is available to read an object of type '{0}' from content with media type '{1}'", type.Name, mediaType.MediaType), mediaType);
            }

            token.ThrowIfCancellationRequested();

            var stream = await content.ReadAsStreamAsync();

            return await formatter.ReadFromStreamAsync(type, stream, content, null, token);
        }
    }
}