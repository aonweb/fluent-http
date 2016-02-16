using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Serialization;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shouldly;
using Xunit;

namespace AonWeb.FluentHttp.Tests.Hal
{
    public class HalSerializationTests
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new HalResourceConverter()
            },
            ContractResolver = new CamelCasePropertyNamesContractResolver()
            {
                IgnoreSerializableInterface = true,
                IgnoreSerializableAttribute = true,
                SerializeCompilerGeneratedMembers = false
            },
            Formatting = Formatting.Indented
        };

        [Fact]
        public void CanSerializeResource()
        {
            var original = TestResource.Default1();

            var json = JsonConvert.SerializeObject(original, _settings);
            const string expected = TestResource.SerializedDefault1;
            var deserialized = JsonConvert.DeserializeObject<TestResource>(json, _settings);

            json.ShouldNotBeNullOrWhiteSpace();
            json.ShouldBe(expected, Case.Insensitive);
            original.ShouldBe(deserialized);
        }

        [Fact]
        public void CanSerializeResourceWithNullLinks()
        {
            var original = TestResource.Default1();
            original.Links = null;

            var json = JsonConvert.SerializeObject(original, _settings);

            const string expected = @"{
  ""stringProperty"": ""TestString"",
  ""intProperty"": 2,
  ""boolProperty"": true,
  ""dateOffsetProperty"": ""2000-01-01T00:00:00-05:00"",
  ""dateProperty"": ""2000-01-01T00:00:00"",
  ""_links"": {}
}";
            var deserialized = JsonConvert.DeserializeObject<TestResource>(json, _settings);

            json.ShouldNotBeNullOrWhiteSpace();
            json.ShouldBe(expected, Case.Insensitive);
            deserialized.Links.ShouldNotBeNull();
            deserialized.Links.Count.ShouldBe(0);
            deserialized.Links = null;
            original.ShouldBe(deserialized);
        }

        [Fact]
        public void CanSerializeResourceWithNullContent()
        {
            var original = new TestResource
            {
                Links = new TestLinks
                {
                    new HyperMediaLink{ Rel = "self", Href="http://link.com/1"}
                },
                StringProperty = null
            };
            var json = JsonConvert.SerializeObject(original, _settings);
            const string expected = @"{
  ""stringProperty"": null,
  ""intProperty"": 0,
  ""boolProperty"": false,
  ""dateOffsetProperty"": ""0001-01-01T00:00:00+00:00"",
  ""dateProperty"": ""0001-01-01T00:00:00"",
  ""_links"": {
    ""self"": {
      ""href"": ""http://link.com/1"",
      ""templated"": false
    }
  }
}";
            var deserialized = JsonConvert.DeserializeObject<TestResource>(json, _settings);

            json.ShouldNotBeNullOrWhiteSpace();
            json.ShouldBe(expected, Case.Insensitive);
            original.ShouldBe(deserialized);
        }

        [Fact]
        public void CanSerializeListWithEmbedded()
        {
            var original = TestListResource.Default1();

            var json = JsonConvert.SerializeObject(original, _settings);
            const string expected = TestListResource.SerializedDefault1;

            var deserialized = JsonConvert.DeserializeObject<TestListResource>(json, _settings);

            json.ShouldNotBeNullOrWhiteSpace();
            json.ShouldBe(expected, Case.Insensitive);
            original.ShouldBe(deserialized);
        }

        [Fact]
        public void CanSerializeListWithEmbeddedWithLinks()
        {
            var original = TestListResourceWithLinks.Default1();

            var json = JsonConvert.SerializeObject(original, _settings);

            const string expected = TestListResourceWithLinks.SerializedDefault1;
            var deserialized = JsonConvert.DeserializeObject<TestListResourceWithLinks>(json, _settings);

            json.ShouldNotBeNullOrWhiteSpace();
            json.ShouldBe(expected, Case.Insensitive);
            original.ShouldBe(deserialized);
        }

        [Fact]
        public void CanSerializeListWithEmbeddedWithLinksWithEmbeddedJsonProperty()
        {
            var original = TestListEmbeddedPropertyParentsResource.Default1();

            var json = JsonConvert.SerializeObject(original, _settings);
            const string expected = TestListEmbeddedPropertyParentsResource.SerializedDefault1;
            var deserialized = JsonConvert.DeserializeObject<TestListEmbeddedPropertyParentsResource>(json, _settings);

            json.ShouldNotBeNullOrWhiteSpace();
            json.ShouldBe(expected, Case.Insensitive);
            original.ShouldBe(deserialized);
        }

        [Fact]
        public void CanSerializeWithEmbedAsArray()
        {
            var original = TestListEmbeddedArrayParentResource.Default1();

            var json = JsonConvert.SerializeObject(original, _settings);

            const string expected = TestListEmbeddedArrayParentResource.SerializedDefault1;
            var deserialized = JsonConvert.DeserializeObject<TestListEmbeddedArrayParentResource>(json, _settings);

            json.ShouldNotBeNullOrWhiteSpace();
            json.ShouldBe(expected, Case.Insensitive);
            original.ShouldBe(deserialized);
        }

        [Fact]
        public void CanSerializeWithEmbedIsNull()
        {
            var original = TestListEmbeddedArrayParentResource.DefaultWithNullEmbed();

            var json = JsonConvert.SerializeObject(original, _settings);

            const string expected = TestListEmbeddedArrayParentResource.SerializedDefaultWithNullEmbed;

            var deserialized = JsonConvert.DeserializeObject<TestListEmbeddedArrayParentResource>(json, _settings);

            json.ShouldNotBeNullOrWhiteSpace();

            json.ShouldBe(expected, Case.Insensitive);
            original.ShouldBe(deserialized);
        }
    }
}