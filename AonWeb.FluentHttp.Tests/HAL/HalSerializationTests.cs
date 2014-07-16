using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.HAL.Serialization;
using AonWeb.FluentHttp.Tests.Helpers.HAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AonWeb.FluentHttp.Tests.HAL
{
    [TestFixture]
    public class HalSerializationTests
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new HalResourceConverter()
            },
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        [Test]
        public void CanSerializeResource()
        {
            var original = new TestResource
            {
                Result = "Content",
                Links = new HyperMediaLinks()
                {
                    new HyperMediaLink{ Rel = "self", Href="http://link.com/1"},
                    new HyperMediaLink{ Rel = "link2", Href="http://link.com/2"}
                }
            };

            var json = JsonConvert.SerializeObject(original, _settings);
            const string expected = @"{
  ""result"": ""Content"",
  ""_links"": {
    ""self"": {
      ""href"": ""http://link.com/1"",
      ""templated"": false
    },
    ""link2"": {
      ""href"": ""http://link.com/2"",
      ""templated"": false
    }
  }
}";
            var deserialized = JsonConvert.DeserializeObject<TestResource>(json, _settings);

            Assert.IsNotNullOrEmpty(json);
            AssertAreEqualIgnoringWhitespace(expected, json);
            Assert.That(original, Is.EqualTo(deserialized).Using(GenericEqualityComparer.Default));
        }

        [Test]
        public void CanSerializeResourceWithNullLinks()
        {
            var original = new TestResource
            {
                Links = null,
                Result = "Content"
            };

            var json = JsonConvert.SerializeObject(original, _settings);

            const string expected = @"{
  ""result"": ""Content"",
  ""_links"": {
  }
}";
            var deserialized = JsonConvert.DeserializeObject<TestResource>(json, _settings);

            Assert.IsNotNullOrEmpty(json);
            AssertAreEqualIgnoringWhitespace(expected, json);
        }

        [Test]
        public void CanSerializeResourceWithNullContent()
        {
            var original = new TestResource
            {
                Links = new TestLinks
                {
                    new HyperMediaLink{ Rel = "self", Href="http://link.com/1"}
                },
                Result = null
            };
            var json = JsonConvert.SerializeObject(original, _settings);
            const string expected = @"{
  ""result"": null,
  ""_links"": {
    ""self"": {
      ""href"": ""http://link.com/1"",
      ""templated"": false
    }
  }
}";
            var deserialized = JsonConvert.DeserializeObject<TestResource>(json, _settings);

            Assert.IsNotNullOrEmpty(json);
            AssertAreEqualIgnoringWhitespace(expected, json);
            Assert.That(original, Is.EqualTo(deserialized).Using(GenericEqualityComparer.Default));
        }

        [Test]
        public void CanSerializeListWithEmbedded()
        {
            var original = new TestListResource
            {
                Links = new HyperMediaLinks()
                {
                    new HyperMediaLink{ Rel = "self", Href="http://link.com/1"},
                    new HyperMediaLink{ Rel = "link2", Href="http://link.com/2"},
                    new HyperMediaLink{ Rel = "link3", Href="http://link.com/3"}
                },
                Results = new List<TestResource>
                {
                    new TestResource
                    {
                        Result = "result",
                        Links = new HyperMediaLinks
                        {
                            new HyperMediaLink{ Rel = "self", Href="http://link.com/1"},
                            new HyperMediaLink{ Rel = "link2", Href="http://link.com/2"},
                            new HyperMediaLink{ Rel = "link3", Href="http://link.com/3"}
                        }
                    },
                    new TestResource
                    {
                        Result = "result",
                        Links = new HyperMediaLinks
                        {
                            new HyperMediaLink{ Rel = "self", Href="http://link.com/1"},
                            new HyperMediaLink{ Rel = "link2", Href="http://link.com/2"},
                            new HyperMediaLink{ Rel = "link3", Href="http://link.com/3"}
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(original, _settings);
            const string expected = @"{
  ""_links"": {
    ""self"": {
      ""href"": ""http://link.com/1"",
      ""templated"": false
    },
    ""link2"": {
      ""href"": ""http://link.com/2"",
      ""templated"": false
    },
    ""link3"": {
      ""href"": ""http://link.com/3"",
      ""templated"": false
    }
  },
  ""_embedded"": {
    ""results"": [
      {
        ""result"": ""result"",
        ""_links"": {
          ""self"": {
            ""href"": ""http://link.com/1"",
            ""templated"": false
          },
          ""link2"": {
            ""href"": ""http://link.com/2"",
            ""templated"": false
          },
          ""link3"": {
            ""href"": ""http://link.com/3"",
            ""templated"": false
          }
        }
      },
      {
        ""result"": ""result"",
        ""_links"": {
          ""self"": {
            ""href"": ""http://link.com/1"",
            ""templated"": false
          },
          ""link2"": {
            ""href"": ""http://link.com/2"",
            ""templated"": false
          },
          ""link3"": {
            ""href"": ""http://link.com/3"",
            ""templated"": false
          }
        }
      }
    ]
  }
}";
            var deserialized = JsonConvert.DeserializeObject<TestListResource>(json, _settings);

            Assert.IsNotNullOrEmpty(json);
            AssertAreEqualIgnoringWhitespace(expected, json);
            Assert.That(original, Is.EqualTo(deserialized).Using(GenericEqualityComparer.Default));
        }

        [Test]
        public void CanSerializeListWithEmbeddedWithLinks()
        {
            var original = new TestListResourceWithLinks
            {
                Links = new TestLinks
                {
                    new HyperMediaLink{ Rel = "self", Href="http://link.com/1"},
                    new HyperMediaLink{ Rel = "link2", Href="http://link.com/2"},
                    new HyperMediaLink{ Rel = "link3", Href="http://link.com/3"}
                },
                Results = new List<TestResource>
                {
                    new TestResource
                    {
                        Result = "result",
                        Links = new HyperMediaLinks
                        {
                            new HyperMediaLink{ Rel = "self", Href="http://link.com/1"},
                            new HyperMediaLink{ Rel = "link2", Href="http://link.com/2"},
                            new HyperMediaLink{ Rel = "link3", Href="http://link.com/3"}
                        }
                    },
                    new TestResource
                    {
                        Result = "result",
                        Links = new HyperMediaLinks
                        {
                            new HyperMediaLink{ Rel = "self", Href="http://link.com/1"},
                            new HyperMediaLink{ Rel = "link2", Href="http://link.com/2"},
                            new HyperMediaLink{ Rel = "link3", Href="http://link.com/3"}
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(original, _settings);

            const string expected = @"{
  ""_links"": {
    ""self"": {
      ""href"": ""http://link.com/1"",
      ""templated"": false
    },
    ""link2"": {
      ""href"": ""http://link.com/2"",
      ""templated"": false
    },
    ""link3"": {
      ""href"": ""http://link.com/3"",
      ""templated"": false
    }
  },
  ""_embedded"": {
    ""results"": [
      {
        ""result"": ""result"",
        ""_links"": {
          ""self"": {
            ""href"": ""http://link.com/1"",
            ""templated"": false
          },
          ""link2"": {
            ""href"": ""http://link.com/2"",
            ""templated"": false
          },
          ""link3"": {
            ""href"": ""http://link.com/3"",
            ""templated"": false
          }
        }
      },
      {
        ""result"": ""result"",
        ""_links"": {
          ""self"": {
            ""href"": ""http://link.com/1"",
            ""templated"": false
          },
          ""link2"": {
            ""href"": ""http://link.com/2"",
            ""templated"": false
          },
          ""link3"": {
            ""href"": ""http://link.com/3"",
            ""templated"": false
          }
        }
      }
    ]
  }
}";
            var deserialized = JsonConvert.DeserializeObject<TestListResourceWithLinks>(json, _settings);

            Assert.IsNotNullOrEmpty(json);
            AssertAreEqualIgnoringWhitespace(expected, json);
            Assert.That(original, Is.EqualTo(deserialized).Using(GenericEqualityComparer.Default));
        }

        [Test]
        public void CanSerializeListWithEmbeddedWithLinksWithEmbeddedJsonProperty()
        {
            var original = new TestListEmbeddedPropertyParentsResource
            {
                Result = "result",
                Count = 2,
                Results = new List<TestListEmbeddedPropertyParentResource>
                {
                    new TestListEmbeddedPropertyParentResource
                    {
                        Children = new TestListEmbeddedPropertyChildrenResource
                        {
                            Result = "result",
                            Count = 2,
                            Results = new List<TestPoco>
                            {
                                new TestPoco { Result = "child-result1"},
                                new TestPoco { Result = "child-result2"},
                            },
                            Links = new HyperMediaLinks
                            {
                                new HyperMediaLink{ Rel = "self", Href="http://link.com/1"},
                                new HyperMediaLink{ Rel = "link2", Href="http://link.com/2"},
                                new HyperMediaLink{ Rel = "link3", Href="http://link.com/3"}
                            }
                        },
                        Links = new HyperMediaLinks
                        {
                            new HyperMediaLink{ Rel = "self", Href="http://link.com/1"}
                        }
                    },
                    new TestListEmbeddedPropertyParentResource
                    {
                        Children = new TestListEmbeddedPropertyChildrenResource
                        {
                            Result = "result",
                            Count = 2,
                            Results = new List<TestPoco>
                            {
                                new TestPoco { Result = "child-result1"},
                                new TestPoco { Result = "child-result2"},
                            },
                            Links = new HyperMediaLinks
                            {
                                new HyperMediaLink{ Rel = "self", Href="http://link.com/1"},
                                new HyperMediaLink{ Rel = "link2", Href="http://link.com/2"},
                                new HyperMediaLink{ Rel = "link3", Href="http://link.com/3"}
                            }
                        },
                        Links = new HyperMediaLinks
                        {
                            new HyperMediaLink{ Rel = "self", Href="http://link.com/1"}
                        }
                    }
                },
                Links = new HyperMediaLinks
                {
                    new HyperMediaLink{ Rel = "self", Href="http://link.com/1"},
                    new HyperMediaLink{ Rel = "link2", Href="http://link.com/2"},
                    new HyperMediaLink{ Rel = "link3", Href="http://link.com/3"}
                }
            };
            
            var json = JsonConvert.SerializeObject(original, _settings);
            const string expected = @"{
  ""parent"": [
    {
      ""_embedded"": {
        ""children"": [
          {
            ""result"": ""child-result1""
          },
          {
            ""result"": ""child-result2""
          }
        ],
        ""count"": 2,
        ""result"": ""result"",
        ""_links"": {
          ""self"": {
            ""href"": ""http://link.com/1"",
            ""templated"": false
          },
          ""link2"": {
            ""href"": ""http://link.com/2"",
            ""templated"": false
          },
          ""link3"": {
            ""href"": ""http://link.com/3"",
            ""templated"": false
          }
        }
      },
      ""result"": null,
      ""_links"": {
        ""self"": {
          ""href"": ""http://link.com/1"",
          ""templated"": false
        }
      }
    },
    {
      ""_embedded"": {
        ""children"": [
          {
            ""result"": ""child-result1""
          },
          {
            ""result"": ""child-result2""
          }
        ],
        ""count"": 2,
        ""result"": ""result"",
        ""_links"": {
          ""self"": {
            ""href"": ""http://link.com/1"",
            ""templated"": false
          },
          ""link2"": {
            ""href"": ""http://link.com/2"",
            ""templated"": false
          },
          ""link3"": {
            ""href"": ""http://link.com/3"",
            ""templated"": false
          }
        }
      },
      ""result"": null,
      ""_links"": {
        ""self"": {
          ""href"": ""http://link.com/1"",
          ""templated"": false
        }
      }
    }
  ],
  ""count"": 2,
  ""result"": ""result"",
  ""_links"": {
    ""self"": {
      ""href"": ""http://link.com/1"",
      ""templated"": false
    },
    ""link2"": {
      ""href"": ""http://link.com/2"",
      ""templated"": false
    },
    ""link3"": {
      ""href"": ""http://link.com/3"",
      ""templated"": false
    }
  }
}";
            var deserialized = JsonConvert.DeserializeObject<TestListEmbeddedPropertyParentsResource>(json, _settings);

            Assert.IsNotNullOrEmpty(json);
            AssertAreEqualIgnoringWhitespace(expected, json);
            Assert.That(original, Is.EqualTo(deserialized).Using(GenericEqualityComparer.Default));
        }

        [Test]
        public void CanSerializeWithEmbedAsArray()
        {
            var original = new TestListEmbeddedArrayParentResource
            {
                Result = "result",
                Results = new List<TestPoco>
                {
                    new TestPoco { Result = "child-result1"},
                    new TestPoco { Result = "child-result2"},
                },
                Links = new HyperMediaLinks
                {
                    new HyperMediaLink{ Rel = "self", Href="http://link.com/1"},
                    new HyperMediaLink{ Rel = "link2", Href="http://link.com/2"},
                    new HyperMediaLink{ Rel = "link3", Href="http://link.com/3"}
                }
            };

            var json = JsonConvert.SerializeObject(original, _settings);

            const string expected = @"{
  ""result"": ""result"",
  ""_links"": {
    ""self"": {
      ""href"": ""http://link.com/1"",
      ""templated"": false
    },
    ""link2"": {
      ""href"": ""http://link.com/2"",
      ""templated"": false
    },
    ""link3"": {
      ""href"": ""http://link.com/3"",
      ""templated"": false
    }
  },
  ""_embedded"": {
    ""children"": [
      {
        ""result"": ""child-result1""
      },
      {
        ""result"": ""child-result2""
      }
    ]
  }
}";
            var deserialized = JsonConvert.DeserializeObject<TestListEmbeddedArrayParentResource>(json, _settings);

            Assert.IsNotNullOrEmpty(json);
            AssertAreEqualIgnoringWhitespace(expected, json);
            Assert.That(original, Is.EqualTo(deserialized).Using(GenericEqualityComparer.Default));
        }

        [Test]
        public void CanSerializeWithEmbedIsNull()
        {
            var original = new TestListEmbeddedArrayParentResource
            {
                Result = "result",
                Results = null,
                Links = new HyperMediaLinks
                {
                    new HyperMediaLink{ Rel = "self", Href="http://link.com/1"},
                    new HyperMediaLink{ Rel = "link2", Href="http://link.com/2"},
                    new HyperMediaLink{ Rel = "link3", Href="http://link.com/3"}
                }
            };

            var json = JsonConvert.SerializeObject(original, _settings);

            const string expected = @"{
  ""result"": ""result"",
  ""_links"": {
    ""self"": {
      ""href"": ""http://link.com/1"",
      ""templated"": false
    },
    ""link2"": {
      ""href"": ""http://link.com/2"",
      ""templated"": false
    },
    ""link3"": {
      ""href"": ""http://link.com/3"",
      ""templated"": false
    }
  },
  ""_embedded"": {
    ""children"": null
  }
}";

            var deserialized = JsonConvert.DeserializeObject<TestListEmbeddedArrayParentResource>(json, _settings);

            Assert.IsNotNullOrEmpty(json);
            AssertAreEqualIgnoringWhitespace(expected, json);
            Assert.That(original, Is.EqualTo(deserialized).Using(GenericEqualityComparer.Default));
        }

        public static void AssertAreEqualIgnoringWhitespace(string expected, string actual, string message = null)
        {
            var expectedNoWhitespace = Regex.Replace(expected ?? "", "\\s", "", RegexOptions.IgnoreCase);
            var actualNoWhitespace = Regex.Replace(actual ?? "", "\\s", "", RegexOptions.IgnoreCase);

            StringAssert.AreEqualIgnoringCase(expectedNoWhitespace, actualNoWhitespace, message);
        }
    }
}
