using System;
using System.Collections.Specialized;
using System.Text;

namespace AonWeb.FluentHttp.Mocks
{
    public interface ILocalRequestContext
    {
        string Body { get;  }
        Encoding ContentEncoding { get;  }
        string ContentType { get;  }
        NameValueCollection Headers { get;  }
        string HttpMethod { get;  }
        Uri Url { get;  }
        bool HasEntityBody { get;  }
        long ContentLength { get;  }
        Uri UrlReferrer { get;  }
        string RawUrl { get;  }
        string[] AcceptTypes { get;  }
        string UserAgent { get;  }
        long RequestCount { get; }
        long RequestCountForThisUrl { get; }
    }
}