using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpRequestMessage: HttpRequestMessage, IMockRequestContext, IFluentConfigurable<MockHttpRequestMessage, MockHttpRequestMessage>
    {
        private string _contentType = "application/json";

        public MockHttpRequestMessage()
        { }

        public MockHttpRequestMessage(HttpRequestMessage request)
        {
            Version = request.Version;
            Content = request.Content;
            Method = request.Method;
            RequestUri = request.RequestUri;
            foreach (var header in request.Headers)
            {
                Headers.Add(header.Key, header.Value);
            }
            foreach (var props in request.Properties)
            {
                Properties.Add(props.Key, props.Value);
            }
        }

        public MockHttpRequestMessage(HttpListenerRequest request)
        {
            Content = new StreamContent(request.InputStream);
            ContentEncoding = request.ContentEncoding;
            ContentType = request.ContentType;

            foreach (var headerKey in request.Headers.AllKeys)
            {
                var values = request.Headers.GetValues(headerKey);
                if (!Headers.TryAddWithoutValidation(headerKey, values))
                    Content.Headers.TryAddWithoutValidation(headerKey, values);
            }

            Method = new HttpMethod(request.HttpMethod);
            RequestUri = request.Url;

            if (request.UrlReferrer != null)
                Headers.TryAddWithoutValidation("Referer", request.UrlReferrer.OriginalString);

            if (request.AcceptTypes != null)
            {
                Headers.TryAddWithoutValidation("Accept", string.Join(",", request.AcceptTypes));
            }

            if (!string.IsNullOrWhiteSpace(request.UserAgent))
                Headers.TryAddWithoutValidation("User-Agent", request.UserAgent);
        }


        public string ContentString
        {
            set
            {
                if (ContentType != null)
                {
                    Content = new StringContent(value, ContentEncoding, ContentType);
                }
                else
                {
                    Content = new StringContent(value);
                }
            }
        }

        public Encoding ContentEncoding {
            get { return Encoding.GetEncoding(Content?.Headers.ContentEncoding.FirstOrDefault() ?? Encoding.UTF8.WebName); }
            set
            {
                if (value != null)
                    Content?.Headers.TryAddWithoutValidation("Content-Encoding" ,value.WebName);
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
                        Content.Headers.TryAddWithoutValidation("Content-Type", value);
                    else
                        Content.Headers.ContentType = null;
                }

                _contentType = value;
            }
        }

        
        public long RequestCount { get; set; }
        public long RequestCountForThisUrl { get; set; }

        public MockHttpRequestMessage WithConfiguration(Action<MockHttpRequestMessage> configuration)
        {
            configuration?.Invoke(this);

            return this;
        }

        void IConfigurable<MockHttpRequestMessage>.WithConfiguration(Action<MockHttpRequestMessage> configuration)
        {
            WithConfiguration(configuration);
        }
    }
}