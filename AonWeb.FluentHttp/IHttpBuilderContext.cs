using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public interface IHttpBuilderContext: IBuilderContext<IRecursiveHttpBuilder, IHttpBuilderSettings>
    {
        Uri Uri { get; }
        HttpMethod Method { get;  }
        string MediaType { get;  }
        Encoding ContentEncoding { get;  }
        HttpCompletionOption CompletionOption { get;  }
        bool AutoDecompression { get; }
        Func<HttpContent> ContentFactory { get; }
        Func<HttpResponseMessage, Exception> ExceptionFactory { get; }
        HandlerRegister Handler { get; }
        CancellationToken Token { get; }
        void ValidateSettings();
    }
}