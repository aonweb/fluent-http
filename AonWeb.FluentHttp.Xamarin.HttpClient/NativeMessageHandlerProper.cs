using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ModernHttpClient;

namespace AonWeb.FluentHttp.Xamarin.HttpClient
{
    public class NativeMessageHandlerProper : NativeMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //This is required since OKHttp on the Android side will throw an error if it encounters a POST without a body.
            if (request.Method == HttpMethod.Post && request.Content == null)
            {
                request.Content = new ByteArrayContent(new byte[0]);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}