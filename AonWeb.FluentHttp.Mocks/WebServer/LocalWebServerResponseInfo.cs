using System.Net;

namespace AonWeb.FluentHttp.Mocks.WebServer
{
    public class LocalWebServerResponseInfo : ResponseInfo<LocalWebServerResponseInfo>
    {
        public LocalWebServerResponseInfo()
            :base( HttpStatusCode.OK) { }

        public LocalWebServerResponseInfo(HttpStatusCode statusCode)
            : base(statusCode) { }
    }
}