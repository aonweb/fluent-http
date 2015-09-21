using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace AonWeb.FluentHttp.Mocks
{
    public interface ILocalResponse
    {
        string Content { get; set; }
        Encoding ContentEncoding { get; set; }
        string ContentType { get; set; }
        HttpStatusCode StatusCode { get; set; }
        string StatusDescription { get; set; }
        IDictionary<string, string> Headers { get; set; }
        bool IsTransient { get; set; }
        HttpResponseMessage ToHttpResponseMessage();
    }
}