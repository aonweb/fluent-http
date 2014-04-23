using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;

namespace AonWeb.Fluent.Http.Serialization
{
    // TODO: static SerializationProvider to set settings

    public class SerializerFactory<T> : ISerializerFactory<T>
    {
        public ISerializer<T> GetSerializer(HttpResponseMessage response)
        {
            var mediaType = response.Content.Headers.ContentType.MediaType;

            return GetSerializer( mediaType);
        }

        public ISerializer<T> GetSerializer(string mediaType)
        {
            //TODO: more serializers
            //ProtoBuf, Message Pack, XML
            switch (mediaType)
            {
                default:
                    return new JsonSerializer<T>();
            }
        }
    }
}