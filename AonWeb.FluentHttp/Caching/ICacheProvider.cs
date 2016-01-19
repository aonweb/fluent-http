using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Caching
{
    public interface ICacheProvider
    {
        Task<T> Get<T>(string key);
        Task<bool> Put<T>(string key, T value);
        Task<bool> Delete(string key);
        Task DeleteAll();
    }
}