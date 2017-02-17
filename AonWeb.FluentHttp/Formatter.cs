using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp
{
    public class Formatter : IFormatter
    {
        public Formatter()
        {
            MediaTypeFormatters = new MediaTypeFormatterCollection().FluentAdd(new StringMediaFormatter());
        }

        public MediaTypeFormatterCollection MediaTypeFormatters { get; }

        public async Task<HttpContent> CreateContent(object value, ITypedBuilderContext context)
        {
            var content = value as HttpContent;

            if (content != null)
                return content;

            var type = context.ContentType;
            var mediaType = context.MediaType;
            var header = new MediaTypeHeaderValue(mediaType);
            var formatter = MediaTypeFormatters.FindWriter(type, header);

            if (formatter == null)
                throw new UnsupportedMediaTypeException(string.Format(SR.NoWriteFormatterForMimeTypeErrorFormat, type.FormattedTypeName(), mediaType), header);

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
            return ObjectHelpers.Deserialize(response, context.ResultType, MediaTypeFormatters, context.Token);
        }

        public Task<object> DeserializeError(HttpResponseMessage response, ITypedBuilderContext context)
        {
            return ObjectHelpers.Deserialize(response, context.ErrorType, MediaTypeFormatters, context.Token);
        }
    }
}