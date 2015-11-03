using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpResponseMessage: HttpResponseMessage, IMockResponse
    {
        private string _contentType = "application/json";

        public MockHttpResponseMessage()
            : this(HttpStatusCode.OK) { }

        public MockHttpResponseMessage(HttpStatusCode statusCode) 
            : base(statusCode) { }

        public MockHttpResponseMessage(HttpResponseMessage response)
        {
            Version = response.Version;
            Content = response.Content;
            StatusCode = response.StatusCode;
            ReasonPhrase = response.ReasonPhrase;
            RequestMessage = response.RequestMessage;

            foreach (var header in response.Headers)
            {
                Headers.Add(header.Key, header.Value);
            }
        }

        public string ContentString
        {
            set
            {
                var oldContent = Content;
                HttpContent newContent;

                if (ContentType != null)
                {
                    newContent = new StringContent(value ?? string.Empty, ContentEncoding, ContentType);
                }
                else
                {
                    newContent = new StringContent(value ?? string.Empty);
                }

                if (oldContent != null)
                {
                    foreach (var header in oldContent.Headers)
                    {
                        newContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                Content = newContent;
            }
        }

        public Encoding ContentEncoding
        {
            get { return Encoding.GetEncoding(Content?.Headers.ContentEncoding.FirstOrDefault() ?? Encoding.UTF8.WebName); }
            set
            {
                Content?.Headers.ContentEncoding.Add(value.WebName);
            }
        }

        public string ContentType
        {
            get { return Content?.Headers.ContentType?.MediaType ?? _contentType; }
            set
            {
                if (Content != null)
                {
                    if (!string.IsNullOrWhiteSpace(value))
                        Content.Headers.ContentType = new MediaTypeHeaderValue(value);
                    else
                        Content.Headers.ContentType = null;
                }

                _contentType = value;
            }
        }

        public HttpMethod Method => RequestMessage?.Method;
        public Uri RequestUri => RequestMessage?.RequestUri;
        public long RequestCount { get; set; }
        public long RequestCountForThisUrl { get; set; }

        public bool IsTransient { get; set; }

        public HttpResponseMessage ToHttpResponseMessage(HttpRequestMessage request)
        {
            RequestMessage = request;

            return this;
        }

        public IMockResponse AsTransient()
        {
            IsTransient = true;

            return this;
        }

        public IMockResponse AsIntransient()
        {
            IsTransient = false;

            return this;
        }
    }
}