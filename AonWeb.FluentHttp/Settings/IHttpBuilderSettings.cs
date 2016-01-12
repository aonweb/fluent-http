using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Settings
{
    public interface IHttpBuilderSettings : IHttpBuilderContext
    {
        HttpUriBuilder UriBuilder { get; }
        new HttpMethod Method { get; set; }
        new string MediaType { get; set; }
        new Encoding ContentEncoding { get; set; }
        new HttpCompletionOption CompletionOption { get; set; }
        new bool SuppressCancellationErrors { get; set; }
        new bool AutoDecompression { get; set; }
        new Func<IHttpBuilderContext, Task<HttpContent>> ContentFactory { get; set; }
        new Func<HttpResponseMessage, HttpRequestMessage, Exception> ExceptionFactory { get; set; }
        void Reset();
        void ValidateSettings();
        new CancellationToken Token { get; set; }
        ICacheSettings CacheSettings { get; }
        new IRecursiveHttpBuilder Builder { get; set; }
    }
}