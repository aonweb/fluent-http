using System.Net.Http;

namespace AonWeb.FluentHttp.Serialization
{
    // TODO: static SerializationProvider to set settings

    public class SerializerFactory : ISerializerFactory
    {
        public ISerializer<T> GetSerializer<T>(HttpResponseMessage response)
        {
            var mediaType = response.Content.Headers.ContentType.MediaType;

            return GetSerializer<T>(mediaType);
        }

        public ISerializer<T> GetSerializer<T>(string mediaType)
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