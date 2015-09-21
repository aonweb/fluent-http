using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AonWeb.FluentHttp.Mocks.WebServer
{
    public class LocalResponse : ILocalResponse
    {
        public LocalResponse()
            :this( HttpStatusCode.OK) { }

        public LocalResponse(HttpStatusCode statusCode) { 
            Content = string.Empty;
            ContentType = "application/json";
            ContentEncoding = Encoding.UTF8;
            StatusCode = statusCode;
            Headers = new Dictionary<string, string>();
        }

        public string Content { get; set; }
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public bool IsTransient { get; set; }



        public ILocalResponse WithHeader(string name, string value)
        {
            Headers[name] = value;
            return this;
        }

        public HttpResponseMessage ToHttpResponseMessage()
        {
            var response = new HttpResponseMessage(StatusCode)
            {
                ReasonPhrase = StatusDescription ?? StatusCode.ToString()
            };

            foreach (var header in Headers)
                response.Headers.TryAddWithoutValidation(header.Key, header.Value);

            response.Headers.Date = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(Content))
            {
                var buffer = ContentEncoding.GetBytes(Content);
                response.Content = new ByteArrayContent(buffer);

                if (!string.IsNullOrWhiteSpace(ContentType))
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
            }

            return response;
        }
    }
}