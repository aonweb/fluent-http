using System;
using System.Collections.Generic;
using System.Linq;

using AonWeb.FluentHttp.HAL.Representations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AonWeb.FluentHttp.HAL.Serialization
{
    public class HyperMediaLinksConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var links = value as HyperMediaLinks;

            if (links != null)
            {
                writer.WriteStartObject();

                foreach (var link in links)
                {
                    writer.WritePropertyName(link.Rel);
                    writer.WriteStartObject();
                    writer.WritePropertyName("href");
                    writer.WriteValue(link.Href);
                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // hypermedia links require a 
            HyperMediaLinks links;

            try
            {
                links = Activator.CreateInstance(objectType) as HyperMediaLinks;

                if (links == null)
                    throw new NullReferenceException(string.Format("Created instance {0} is not of type HyperMediaLinks", objectType.Name));
            }
            catch (Exception ex)
            {
                throw SerializationErrorHelper.CreateError(reader, string.Format("Could not create HyperMediaLinks object. Type: {0}", objectType.Name), ex);
            }

            var linksJson = JObject.Load(reader);

            if (linksJson != null)
            {
                foreach (var rel in linksJson.OfType<JProperty>())
                {
                    var link = rel.Value.ToObject<HyperMediaLink>();
                    link.Rel = rel.Name;
                    links.Add(link);
                }
            }

            return links;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(HyperMediaLinks).IsAssignableFrom(objectType);
        }
    }
}