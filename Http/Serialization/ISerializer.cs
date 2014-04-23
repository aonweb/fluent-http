using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AonWeb.Fluent.Http.Serialization
{
    public interface ISerializer<T>
    {
        Task<string> Serialize(object value);
        Task<T> Deserialize(HttpContent content);
        Task<T> Deserialize(string content);
    }
}