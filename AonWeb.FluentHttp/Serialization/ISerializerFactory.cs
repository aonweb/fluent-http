using System.Net.Http;
using System.Text;

namespace AonWeb.FluentHttp.Serialization
{
    public interface ISerializerFactory
    {
        ISerializer<T> GetSerializer<T>(HttpResponseMessage response);
        ISerializer<T> GetSerializer<T>(string mediaType);
    }
}