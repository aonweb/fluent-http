using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AonWeb.FluentHttp.Mocks
{
    public interface IMockRequestContext
    {
        HttpContent Content { get; }
        Encoding ContentEncoding { get;  }
        string ContentType { get;  }
        HttpMethod Method { get;  }
        Uri RequestUri { get; }
        HttpRequestHeaders Headers { get; }
        long RequestCount { get; }
        long RequestCountForThisUrl { get; }
    }
}