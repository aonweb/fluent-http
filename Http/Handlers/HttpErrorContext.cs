using System.Net;

namespace AonWeb.Fluent.Http.Handlers
{
    public class HttpErrorContext<TError>
    {
        public HttpStatusCode StatusCode { get; set; }
        public TError Error { get; set; }
        public bool ErrorHandled { get; set; }
    }
}