using System;
using System.Linq;
using AonWeb.FluentHttp.HAL.Representations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace AonWeb.FluentHttp.HAL.Serialization
{
    public class HalResourceConverter : JsonConverter
    {
        public HalResourceConverter() : this(null) { }

        public HalResourceConverter(Type type)
        {
            ObjectType = type;
        }

        protected Type ObjectType { get; set; }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value as IHalResource == null)
                return;

            var resolver = serializer.ContractResolver as CamelCasePropertyNamesContractResolver;

            writer.WriteStartObject();

            foreach (var property in value.GetType().GetProperties())
            {
                // TODO: Handle _embedded serialization
                if (!property.CanRead || property.GetCustomAttributes(true).Any(a => a is HalEmbeddedAttribute))
                    continue;

                if (property.Name == "Links")
                {
                    writer.WritePropertyName("_links");

                    var links = ((IHalResource)value).Links;

                    if (links != null)
                    {
                        writer.WriteStartObject();

                        foreach (var link in links)
                        {
                            writer.WritePropertyName(link.Rel);
                            writer.WriteStartObject();
                            writer.WritePropertyName("href");
                            writer.WriteValue(link.Href);
                            writer.WritePropertyName("templated");
                            writer.WriteValue(link.IsTemplated);
                            writer.WriteEndObject();
                        }

                        writer.WriteEndObject();
                    }
                    else
                    {
                        writer.WriteNull();
                    }
                }
                else
                {
                    writer.WritePropertyName((resolver == null ? property.Name : resolver.GetResolvedPropertyName(property.Name)));
                    writer.WriteValue(property.GetValue(value, null));
                }
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var json = JObject.Load(reader);

            JToken embedded;
            if (json.TryGetValue("_embedded", out embedded))
                json.Remove("_embedded");

            serializer.Converters.Remove(this);

            var resource = json.ToObject(ObjectType ?? objectType, serializer);

            serializer.Converters.Add(this);

            if (embedded == null) 
                return resource;
            
            var enumerator = ((JObject)embedded).GetEnumerator();

            while (enumerator.MoveNext())
            {
                var rel = enumerator.Current.Key;

                foreach (var property in objectType.GetProperties())
                {
                    var attribute = property.GetCustomAttributes(true).OfType<HalEmbeddedAttribute>().FirstOrDefault(attr => attr.Rel == rel);

                    if (attribute == null)
                        continue;

                    var type = attribute.Type ?? property.PropertyType;

                    var propValue = enumerator.Current.Value.ToObject(type, serializer);

                    property.SetValue(resource, propValue, null);
                }
            }
            

            return resource;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IHalResource).IsAssignableFrom(objectType);
        }
    }
}