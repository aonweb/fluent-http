using System.Net.Http;
using System.Text;

namespace AonWeb.Fluent.Http.Serialization
{
    public interface ISerializerFactory<T>
    {
        ISerializer<T> GetSerializer(HttpResponseMessage response);
        ISerializer<T> GetSerializer(string mediaType);
    }
}