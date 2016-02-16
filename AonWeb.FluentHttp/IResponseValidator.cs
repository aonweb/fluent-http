using System.Net.Http;

namespace AonWeb.FluentHttp
{
    public interface ITypedResponseValidator: IResponseValidator
    {
        
    }

    public interface IHttpResponseValidator : IResponseValidator
    {

    }

    public interface IResponseValidator
    {
        bool IsValid(HttpResponseMessage response);
    }
}