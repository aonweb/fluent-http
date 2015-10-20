using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp
{
    public class Formatter : IFormatter
    {
        public async Task<HttpContent> CreateContent(object value, ITypedBuilderContext context)
        {
            var type = context.ContentType;
            var mediaType = context.MediaType;
            var header = new MediaTypeHeaderValue(mediaType);
            var formatter = context.MediaTypeFormatters.FindWriter(type, header);

            if (formatter == null)
                throw new UnsupportedMediaTypeException(string.Format(SR.NoWriteFormatterForMimeTypeErrorFormat, type.FormattedTypeName(), mediaType), header);

            HttpContent content;
            using (var stream = new MemoryStream())
            {
                await formatter.WriteToStreamAsync(type, value, stream, null, null);

                content = new ByteArrayContent(stream.ToArray());
            }

            formatter.SetDefaultContentHeaders(type, content.Headers, header);

            return content;
        }

        public Task<object> DeserializeResult(HttpResponseMessage response, ITypedBuilderContext context)
        {
            return DeserializeResponse(response, context.ResultType, context.MediaTypeFormatters, context.Token);
        }

        public Task<object> DeserializeError(HttpResponseMessage response, ITypedBuilderContext context)
        {
            return DeserializeResponse(response, context.ErrorType, context.MediaTypeFormatters, context.Token);
        }

        private static async Task<object> DeserializeResponse(HttpResponseMessage response, Type type, MediaTypeFormatterCollection formatters, CancellationToken token)
        {
            var typeInfo = type.GetTypeInfo();

            if (typeof(HttpResponseMessage).IsAssignableFrom(typeInfo)) 
                return response;

            var content = response.Content;

            if (content == null) 
                return TypeHelpers.GetDefaultValueForType(type);

            if (typeof(Stream).IsAssignableFrom(typeInfo))
                return await content.ReadAsStreamAsync();

            if (typeof(byte[]).IsAssignableFrom(typeInfo))
                return await content.ReadAsByteArrayAsync();

            if (typeof(string).IsAssignableFrom(typeInfo))
                return await content.ReadAsStringAsync();

            var mediaType = content.Headers.ContentType ?? new MediaTypeHeaderValue("application/octet-stream");

            var formatter = formatters.FindReader(type, mediaType);

            if (formatter == null)
            {
                if (content.Headers.ContentLength == 0)
                    return TypeHelpers.GetDefaultValueForType(type);

                throw new UnsupportedMediaTypeException(string.Format(SR.NoReadFormatterForMimeTypeErrorFormat, typeInfo.FormattedTypeName(), mediaType.MediaType, response.DetailsForException()), mediaType);
            }

            token.ThrowIfCancellationRequested();

            var stream = await content.ReadAsStreamAsync();

            return await formatter.ReadFromStreamAsync(type, stream, content, null, token);
        }
    }
}