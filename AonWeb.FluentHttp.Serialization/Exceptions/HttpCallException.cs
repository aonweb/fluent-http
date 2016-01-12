using System;
using System.Net;
using System.Net.Http;
using AonWeb.FluentHttp.Exceptions.Helpers;

namespace AonWeb.FluentHttp.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an unhandled error in the http call.
    /// </summary>
    public class HttpCallException : Exception, IWriteableExceptionResponseMetadata
    {
        private readonly string _message;

        public HttpCallException(HttpResponseMessage response, HttpRequestMessage request)
            : this(response, request, null, null)
        { }

        public HttpCallException(HttpResponseMessage response, HttpRequestMessage request, string message)
            : this(response, request, message, null)
        { }

        public HttpCallException(HttpResponseMessage response, HttpRequestMessage request, string message, Exception exception)
            : base(message, exception)
        {
            _message = message;

            this.Apply(request);
            this.Apply(response);
        }

        public HttpCallException(HttpStatusCode statusCode)
            : this(statusCode, null)
        { }

        public HttpCallException(HttpStatusCode statusCode, string message)
            : this(statusCode, message, null)
        { }

        public HttpCallException(HttpStatusCode statusCode, string message, Exception exception) :
            base(message, exception)
        {
            _message = message;

            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }
        public string ReasonPhrase { get; private set; }
        public long? ResponseContentLength { get; private set; }
        public string ResponseContentType { get; private set; }
        public Uri RequestUri { get; private set; }
        public HttpMethod RequestMethod { get; private set; }
        public long? RequestContentLength { get; private set; }
        public string RequestContentType { get; private set; }

        public override string Message
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_message))
                    return _message;

                return this.GetExceptionMessage(MessagePrefix, MessageReason, InnerException?.Message);
            }
        }

        protected virtual string MessagePrefix => null;
        protected virtual string MessageReason => null;

        #region IWriteableExceptionResponseMetadata

        string IWriteableExceptionResponseMetadata.ReasonPhrase
        {
            get { return ReasonPhrase; }
            set { ReasonPhrase = value; }
        }

        long? IWriteableExceptionResponseMetadata.ResponseContentLength
        {
            get { return ResponseContentLength; }
            set { ResponseContentLength = value; }
        }

        string IWriteableExceptionResponseMetadata.ResponseContentType
        {
            get { return ResponseContentType; }
            set { ResponseContentType = value; }
        }

        Uri IWriteableExceptionResponseMetadata.RequestUri
        {
            get { return RequestUri; }
            set { RequestUri = value; }
        }

        HttpMethod IWriteableExceptionResponseMetadata.RequestMethod
        {
            get { return RequestMethod; }
            set { RequestMethod = value; }
        }

        long? IWriteableExceptionResponseMetadata.RequestContentLength
        {
            get { return RequestContentLength; }
            set { RequestContentLength = value; }
        }

        string IWriteableExceptionResponseMetadata.RequestContentType
        {
            get { return RequestContentType; }
            set { RequestContentType = value; }
        }

        HttpStatusCode IWriteableExceptionResponseMetadata.StatusCode
        {
            get { return StatusCode; }
            set { StatusCode = value; }
        }

        #endregion
    }
}