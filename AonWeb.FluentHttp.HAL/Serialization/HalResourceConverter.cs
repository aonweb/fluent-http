using System;
using System.Linq;
using AonWeb.Fluent.HAL.Representations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace AonWeb.Fluent.HAL.Serialization
{
    public class HalResourceConverter : JsonConverter
    {
        public HalResourceConverter(Type type = null)
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
            var jsonToken = JToken.ReadFrom(reader);

            var deserializedObject = JsonConvert.DeserializeObject(jsonToken.ToString(), ObjectType ?? objectType, new JsonConverter[] { });

            if (jsonToken["_embedded"] != null && jsonToken["_embedded"].HasValues)
            {
                var enumerator = ((JObject)jsonToken["_embedded"]).GetEnumerator();

                while (enumerator.MoveNext())
                {
                    var rel = enumerator.Current.Key;

                    foreach (var property in objectType.GetProperties())
                    {
                        var attribute = property.GetCustomAttributes(true)
                            .FirstOrDefault(attr => attr is HalEmbeddedAttribute && ((HalEmbeddedAttribute)attr).Rel == rel);

                        var halEmbeddedAttribute = attribute as HalEmbeddedAttribute;

                        if (halEmbeddedAttribute == null)
                            continue;

                        var type = halEmbeddedAttribute.Type ?? property.PropertyType;

                        property.SetValue(
                            deserializedObject,
                            JsonConvert.DeserializeObject(
                                enumerator.Current.Value.ToString(),
                                type,
                                new JsonConverter[]
                                    {
                                        new HalResourceConverter(halEmbeddedAttribute.CollectionMemberType)
                                    }),
                            null);
                    }
                }
            }

            if (jsonToken["_links"] != null && jsonToken["_links"].HasValues && typeof(IHalResource).IsAssignableFrom(objectType))
            {
                ((IHalResource)deserializedObject).Links = JsonConvert.DeserializeObject<HyperMediaLinks>(
                                                            jsonToken["_links"].ToString(),
                                                            new JsonConverter[] { new HyperMediaLinksConverter() });
            }

            return deserializedObject;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IHalResource).IsAssignableFrom(objectType);
        }
    }
}