using System.Net.Http;

namespace AonWeb.FluentHttp.Serialization
{
    public interface ISerializerFactory
    {
        ISerializer<T> GetSerializer<T>(HttpResponseMessage response);
        ISerializer<T> GetSerializer<T>(string mediaType);
    }
}