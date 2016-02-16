using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Caching
{
    public class ResponseSerializer
    {
        public async Task<HttpResponseMessage> Deserialize(byte[] buffer, CancellationToken token)
        {
            using (var stream = new MemoryStream(buffer))
            {
                var response = new HttpResponseMessage
                {
                    Content = new StreamContent(stream)
                };
                response.Content.Headers.Add("Content-Type", "application/http;msgtype=response");
                token.ThrowIfCancellationRequested();
                await response.Content.LoadIntoBufferAsync();
                return await response.Content.ReadAsHttpResponseMessageAsync(token);
            }
        }

        public async Task<byte[]> Serialize(HttpResponseMessage response, CancellationToken token)
        {
            var request = response.RequestMessage;

            response.RequestMessage = null;

            if (response.Content != null)
                await response.Content.LoadIntoBufferAsync();

            var httpMessageContent = new HttpMessageContent(response);

            token.ThrowIfCancellationRequested();

            var buffer = await httpMessageContent.ReadAsByteArrayAsync();

            response.RequestMessage = request;

            using (var stream = new MemoryStream())
            {
                await stream.WriteAsync(buffer, 0, buffer.Length, token);

                return stream.ToArray();
            }
        }
    }
}