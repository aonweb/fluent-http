using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.HAL.Serialization;

using Newtonsoft.Json;

namespace AonWeb.FluentHttp.Tests.Helpers.HAL
{
    public class HalMother
    {
        public static string TestResourceJson = "{\"result\":\"Response1\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}}";
        public static string TestResourceJson2 = "{\"result\":\"Response2\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}}";
        public static string TestResourceWithLinksJson = "{\"result\":\"Response1\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false},\"link1\":{\"href\":\"http://localhost:8889/link1/1\",\"templated\":false}}}";
        public static string TestResourceWithLinksJson2 = "{\"result\":\"Response2\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false},\"link1\":{\"href\":\"http://localhost:8889/link1/1\",\"templated\":false}}}";
        public static string TestListJson = "{\"_embedded\":{\"results\":[{\"result\":\"Response1ForItem1\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}},{\"result\":\"Response1ForItem2\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/2\",\"templated\":false}}},{\"result\":\"Response1ForItem3\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}}]},\"_links\":{\"self\":{\"href\":\"http://localhost:8889/list/1\",\"templated\":false}}}";
        public static string TestListJson2 = "{\"_embedded\":{\"results\":[{\"result\":\"Response2ForItem1\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}},{\"result\":\"Response2ForItem2\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/2\",\"templated\":false}}},{\"result\":\"Response2ForItem3\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}}]},\"_links\":{\"self\":{\"href\":\"http://localhost:8889/list/1\",\"templated\":false}}}";

        public static string TestListEmbeddedArrayJson = @"{
    ""result"": ""parent"",
    ""_embedded"": [{
        ""children"": [{
            ""result"":""child1""
        },
        {
            ""result"":""child1""
        }]
    }],
    ""_links"": {
        ""self"": {
            ""href"": ""http://link.com"",
            ""templated"": false
        }
    }
}";
        public static string TestListEmbeddedPropertyJson = @"{
  ""parent"": [
    {
      ""result"": ""parent1"",
      ""_embedded"": {
        ""children"": [
          {
            ""result"": ""child1""
          },
          {
            ""result"": ""child2""
          }
        ],
        ""links"": {
          ""self"": {
            ""href"": ""http://localhost:8889/canonical/parent1/children"",
            ""templated"": false
          }
        },
        ""count"": 2
      }
    },
    {
      ""result"": ""parent2"",
      ""_embedded"": {
        ""children"": [
          {
            ""result"": ""child1""
          },
          {
            ""result"": ""child2""
          }
        ],
        ""links"": {
          ""self"": {
            ""href"": ""http://localhost:8889/canonical/parent2/children"",
            ""templated"": false
          }
        },
        ""count"": 2
      }
    }
  ],
  ""links"": {
    ""self"": {
      ""href"": ""http://localhost:8889/canonical/parent2"",
      ""templated"": false
    }
  },
  ""count"": 2
}";
    }

    public class TestResource : HalResource
    {
        public string Result { get; set; }
    }

    public class TestError
    {
        public string Result { get; set; }
    }

    public class TestResourceWithLinks : HalResource<TestLinks>
    {
        public string Result { get; set; }
    }

    public class TestLinks : HyperMediaLinks
    {
        public Uri Link1()
        {
            return this.GetLink("link1");
        }
    }

    public class TestListResource : HalResource
    {
        [HalEmbedded("results")]
        public IList<TestResource> Results { get; set; }
    }

    public class TestListResourceWithLinks : HalResource<TestLinks>
    {
        [HalEmbedded("results")]
        public IList<TestResource> Results { get; set; }
    }

    public class TestListEmbeddedPropertyParentsResource : TestResource
    {
        [JsonProperty("parent")]
        public IList<TestListEmbeddedPropertyParentResource> Results { get; set; }
        public int Count { get; set; }
    }

    public class TestListEmbeddedPropertyParentResource : TestResource
    {
        [JsonProperty("_embedded")]
        public TestListEmbeddedPropertyChildrenResource Children { get; set; }
    }

    public class TestListEmbeddedPropertyChildrenResource : TestResource
    {
        [JsonProperty("children")]
        public IList<TestPoco> Results { get; set; }
        public int Count { get; set; }
    }

    public class TestPoco
    {
        public string Result { get; set; }
    }

    public class TestListEmbeddedArrayParentResource : TestResource
    {
        [HalEmbedded("children")]
        public IList<TestPoco> Results { get; set; }
    }

    public class GenericEqualityComparer : IEqualityComparer
    {
        private static Lazy<GenericEqualityComparer> _default = new Lazy<GenericEqualityComparer>(() => new GenericEqualityComparer());
        public static GenericEqualityComparer Default { get { return _default.Value; } }

        public bool Equals(object x, object y)
        {
            return Equals(x, y, string.Empty);
        }

        public bool Equals(object x, object y, string parentName)
        {
            if (ReferenceEquals(null, x) && ReferenceEquals(null, y)) 
                return true;

            if (ReferenceEquals(null, x))
            {
                Console.WriteLine("{0} was null for x but not y", parentName.TrimEnd('.'));
                return false;
            }   

            if (ReferenceEquals(null, y))
            {
                Console.WriteLine("{0} was null for y but not x", parentName.TrimEnd('.'));
                return false;
            } 

            if (ReferenceEquals(x, y))
                return true;

            if (typeof(IEquatable<>).IsInstanceOfType(x))
            {
                var e = x.Equals(y);

                if (!e)
                    Console.WriteLine("{0} was IEquatable and not equal. x value: {1}, y value {2}", parentName.TrimEnd('.'), x, y);

                return e;
            }

            if (typeof(IEnumerable).IsInstanceOfType(x))
            {
                var e = EnumerableComparer.Default.Equals((IEnumerable)x, (IEnumerable)y, parentName);

                if (!e)
                    Console.WriteLine("{0} was IEnumerable and not equal.", parentName.TrimEnd('.'));

                return e;
            }

            Type type;
            var xtype = x.GetType();
            var ytype = y.GetType();

            if (ytype.IsAssignableFrom(xtype)) 
                type = xtype;
            else if (xtype.IsAssignableFrom(ytype)) 
                type = ytype;
            else
            {
                return false;
            }


            return type.GetProperties().All(property => PropertyValueEquals(property, x, y, parentName));
        }

        private bool PropertyValueEquals(PropertyInfo property, object x, object y, string parentName)
        {
            var propx = property.GetValue(x);
            var propy = property.GetValue(y);

            return Equals(propx, propy, parentName + property.Name + ".");

        }

        public int GetHashCode(object obj)
        {
            return obj == null ? 0 : obj.GetHashCode();
        }
    }

    public class EnumerableComparer : IEqualityComparer<IEnumerable>
    {
        private EnumerableComparer() { }

        private static Lazy<EnumerableComparer> _default = new Lazy<EnumerableComparer>(() => new EnumerableComparer());
        public static EnumerableComparer Default { get { return _default.Value; } }

        public bool Equals(IEnumerable x, IEnumerable y)
        {
            return Equals(x, y, string.Empty);
        }

        public bool Equals(IEnumerable x, IEnumerable y, string parentName)
        {
            if (ReferenceEquals(null, x) && ReferenceEquals(null, y))
                return true;

            if (ReferenceEquals(null, x))
            {
                Console.WriteLine("Value was null for x but not y");
                return false;
            }

            if (ReferenceEquals(null, y))
            {
                Console.WriteLine("Value was null for y but not x");
                return false;
            } 


            if (ReferenceEquals(x, y))
                return true;

            var xEnum = x.GetEnumerator();
            var yEnum = y.GetEnumerator();

            var i = 0;

            while (xEnum.MoveNext())
            {
                if (!yEnum.MoveNext())
                {
                    Console.WriteLine("x is longer than y");
                    return false;
                }

                var xi = xEnum.Current;
                var yi = yEnum.Current;

                if (!GenericEqualityComparer.Default.Equals(xi, yi, parentName + "[" + i + "]")) 
                    return false;

                i++;
            }

            if (yEnum.MoveNext())
            {
                Console.WriteLine("x is longer than y");
                return false;
            }

            return true;
        }

        public int GetHashCode(IEnumerable obj)
        {
            if (obj == null)
                return 0;

            return obj.Cast<object>().Aggregate(0, (current, item) => current ^ (item == null ? 0 : item.GetHashCode()) * 397);
        }
    }
}