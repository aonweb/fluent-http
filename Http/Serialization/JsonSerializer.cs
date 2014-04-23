using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AonWeb.Fluent.Http.Serialization
{
    public class JsonSerializer<T> : ISerializer<T>
    {
        public async Task<string> Serialize(object value)
        {
            return await JsonConvert.SerializeObjectAsync(value, Formatting.None, GetSettings());
        }

        public async Task<T> Deserialize(HttpContent content)
        {
            return await Deserialize(await content.ReadAsStringAsync());
        }

        public async Task<T> Deserialize(string content)
        {
            return await JsonConvert.DeserializeObjectAsync<T>(content, GetSettings());
        }

        private JsonSerializerSettings GetSettings()
        {
            // TODO: allow settings to be set globally or at least somewhere
            return new JsonSerializerSettings();
        }
    }
}