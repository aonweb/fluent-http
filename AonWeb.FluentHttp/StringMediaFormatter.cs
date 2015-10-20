using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp
{
    public class StringMediaFormatter : MediaTypeFormatter
    {
        public StringMediaFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }
        public override bool CanReadType(Type type)
        {
            return (typeof(string).IsAssignableFrom(type));
        }

        public override bool CanWriteType(Type type)
        {
            return (typeof(string).IsAssignableFrom(type));
        }

        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            return await content.ReadAsStringAsync();
        }

        public override Task WriteToStreamAsync(
            Type type,
            object value,
            Stream writeStream,
            HttpContent content,
            TransportContext transportContext)
        {
            var stringContent = content as StringContent;

            if (stringContent == null)
            {
                var valueString = string.Empty;
                string charSet = null;
                string mediaType = null;
                Encoding encoding = null;

                if (value != null) 
                    valueString = value.ToString();

                if (content != null)
                {
                    mediaType = content.Headers.ContentType.MediaType;

                    if (content.Headers.ContentType != null)
                        charSet = content.Headers.ContentType.CharSet;
                    else
                        charSet = content.Headers.ContentEncoding.FirstOrDefault();
                    
                }

                if (string.IsNullOrWhiteSpace(charSet))
                {
                    encoding = Encoding.UTF8;
                }

                if (string.IsNullOrWhiteSpace(mediaType))
                    mediaType = "text/plain";

                if (encoding == null)
                {
                    try
                    {
                        encoding = Encoding.GetEncoding(charSet);
                    }
                    catch (Exception)
                    {
                        encoding = Encoding.UTF8;
                    }  
                }

                stringContent = new StringContent(valueString, encoding, mediaType);
            }

            return stringContent.CopyToAsync(writeStream, transportContext);
        }
    }
}