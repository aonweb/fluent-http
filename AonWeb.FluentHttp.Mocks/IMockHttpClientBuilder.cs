using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp.Mocks
{
    public interface IMockHttpClientBuilder : IHttpClientBuilder, IResponseMocker<IMockHttpClientBuilder> { }
}