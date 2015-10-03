using System;
using System.Collections.Generic;

namespace AonWeb.FluentHttp
{
    public class NormalizedUriQueryCollection : UriQueryCollection
    {
        public NormalizedUriQueryCollection()
            :base(new SortedDictionary<string, ISet<string>>(StringComparer.OrdinalIgnoreCase)) { }

        public NormalizedUriQueryCollection(IReadonlyUriQueryCollection collection)
            : base(new SortedDictionary<string, ISet<string>>(collection.ToDictionary(), StringComparer.OrdinalIgnoreCase)) { }

        protected override ISet<string> CreateItem(IEnumerable<string> values)
        {
            return new SortedSet< string > (values, StringComparer.Ordinal);
        }

        //implemented from of System.Web.HttpValueCollection.FillFromString
        public new static NormalizedUriQueryCollection FromQueryString(string queryString)
        {
            var collection = new NormalizedUriQueryCollection();

            collection.FillFromString(queryString);

            return collection;
        }
    }
}