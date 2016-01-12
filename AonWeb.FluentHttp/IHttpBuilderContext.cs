using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public interface IHttpBuilderContext: IBuilderContext<IRecursiveHttpBuilder>
    {
        Uri Uri { get; }
        HttpMethod Method { get;  }
        string MediaType { get;  }
        Encoding ContentEncoding { get;  }
        HttpCompletionOption CompletionOption { get;  }
        bool AutoDecompression { get; }
        Func<IHttpBuilderContext, Task<HttpContent>> ContentFactory { get; }
        Func<HttpResponseMessage, HttpRequestMessage, Exception> ExceptionFactory { get; }
        HttpHandlerRegister HandlerRegister { get; }
        ResponseValidatorCollection ResponseValidator { get; }
        ICacheMetadata CacheMetadata { get; }
    }
}