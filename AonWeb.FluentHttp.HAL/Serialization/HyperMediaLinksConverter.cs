using System;
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
            var links = new HyperMediaLinks();
            var json = JObject.Load(reader);
            var enumerator = json.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var link = JsonConvert.DeserializeObject<HyperMediaLink>(enumerator.Current.Value.ToString());
                link.Rel = enumerator.Current.Key;
                links.Add(link);
            }

            return links;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}