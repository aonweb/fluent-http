using System.Collections;

namespace AonWeb.FluentHttp.Handlers
{
    public interface IHttpCallContext
    {
        IDictionary Items { get; }
    }
}