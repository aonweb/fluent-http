using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            var resource = value as IHalResource;

            if (resource == null)
                return;

            var resolver = serializer.ContractResolver as CamelCasePropertyNamesContractResolver;

            writer.WriteStartObject();

            var embeds = new List<Tuple<HalEmbeddedAttribute, PropertyInfo>>();

            foreach (var property in value.GetType().GetProperties())
            {
                if (!property.CanRead)
                    continue;

                var embeddedAttribute = property.GetCustomAttributes(true).OfType<HalEmbeddedAttribute>().FirstOrDefault();

                if (embeddedAttribute != null)
                {
                    embeds.Add(new Tuple<HalEmbeddedAttribute, PropertyInfo>(embeddedAttribute, property));
                }
                else if (property.Name == "Links")
                {
                    WriteLinks(writer, resource.Links);
                }
                else
                {
                    writer.WritePropertyName((resolver == null ? property.Name : resolver.GetResolvedPropertyName(property.Name)));
                    writer.WriteValue(property.GetValue(value, null));
                }
            }


            WriteEmbeddeds(writer, embeds, value);

            writer.WriteEndObject();
        }

        private static void WriteEmbeddeds(JsonWriter writer, IList<Tuple<HalEmbeddedAttribute, PropertyInfo>> attributesAndProperties, object parentValue)
        {
            writer.WritePropertyName("_embedded");

            if (attributesAndProperties.Count > 1)
                writer.WriteStartArray();

            foreach (var attributesAndProperty in attributesAndProperties)
            {
                var embeddedAttribute = attributesAndProperty.Item1;
                var embedValue = attributesAndProperty.Item2.GetValue(parentValue);
                writer.WriteStartObject();
                writer.WritePropertyName(embeddedAttribute.Rel);
                writer.WriteValue(embedValue);
                writer.WriteEndObject();
            }

            if (attributesAndProperties.Count > 1)
                writer.WriteEndArray();
        }

        private static void WriteLinks(JsonWriter writer, IEnumerable<HyperMediaLink> links)
        {
            writer.WritePropertyName("_links");

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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            objectType = ObjectType ?? objectType;
            var json = JObject.Load(reader);

            var hasJsonPropEmbedded =
                objectType.GetProperties()
                    .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>())
                    .Any(a => a != null && a.PropertyName == "_embedded");

            JToken embedded = null;

            if (!hasJsonPropEmbedded)
            {
                if (json.TryGetValue("_embedded", out embedded))
                    json.Remove("_embedded");
            }

            JToken links;
            if (json.TryGetValue("_links", out links))
                json.Remove("_links");

            object resource;

            try
            {
                resource = Activator.CreateInstance(objectType);
            }
            catch (Exception ex)
            {
                throw SerializationErrorHelper.CreateError(reader, string.Format("Could not create HalResource object. Type: {0}", objectType.Name), ex);
            }

            serializer.Populate(json.CreateReader(), resource);

            TryPopulateLinks(reader, objectType, serializer, links, resource);

            TryPopulateEmbedded(embedded, objectType, serializer, resource);
            

            return resource;
        }

        private static void TryPopulateLinks(
            JsonReader reader,
            Type objectType,
            JsonSerializer serializer,
            JToken links,
            object resource)
        {
            if (links == null)
                return;

            var linkProperty = objectType.GetProperty("Links");

            if (linkProperty == null)
                throw SerializationErrorHelper.CreateError(reader, string.Format("Could not create HyperMediaLinks object. Could not find property 'Links' on object of type {0}", objectType.Name));

            var linkListType = linkProperty.PropertyType;

            if (!typeof(IList<HyperMediaLink>).IsAssignableFrom(linkListType))
                throw SerializationErrorHelper.CreateError(reader, string.Format("Could not create HyperMediaLinks object. Links property type '{0}' on type '{1}' is not assignable to IList<HyperMediaLink>", linkListType.Name, objectType.Name));

            IList<HyperMediaLink> list;
            
            try
            {
                list = (IList<HyperMediaLink>)Activator.CreateInstance(linkListType);
            }
            catch (Exception ex)
            {
                throw SerializationErrorHelper.CreateError(reader, string.Format("Could not create HyperMediaLinks object. Type: {0}", linkListType.Name), ex);
            }

            var enumerator = ((JObject)links).GetEnumerator();

            while (enumerator.MoveNext())
            {
                var link = new HyperMediaLink{ Rel = enumerator.Current.Key };
                serializer.Populate(enumerator.Current.Value.CreateReader(), link);
                list.Add(link);
            }

            linkProperty.SetValue(resource, list);
        }

        private static void TryPopulateEmbedded(JToken embedded, Type objectType, JsonSerializer serializer, object resource)
        {
            if (embedded == null) 
                return;

            if (embedded.Type == JTokenType.Array)
            {
                foreach (var item in ((JArray)embedded))
                    TryPopulateEmbedded(item, objectType, serializer, resource);
            }
            else
            {
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
            }

            
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IHalResource).IsAssignableFrom(objectType);
        }
    }
}