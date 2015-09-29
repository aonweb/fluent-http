using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace AonWeb.FluentHttp
{
    public interface IHttpBuilderSettings: IHttpBuilderContext
    {
        HttpUriBuilder UriBuilder { get; }
        new HttpMethod Method { get; set; }
        new string MediaType { get; set; }
        new Encoding ContentEncoding { get; set; }
        new HttpCompletionOption CompletionOption { get; set; }
        new bool SuppressCancellationErrors { get; set; }
        new bool AutoDecompression { get; set; }
        new Func<IHttpBuilderContext, HttpContent> ContentFactory { get; set; }
        new Func<HttpResponseMessage, Exception> ExceptionFactory { get; set; }
        void Reset();
        IList<Func<HttpResponseMessage, bool>> SuccessfulResponseValidators { get; }

       new CancellationToken Token { get; set; }
    }
}