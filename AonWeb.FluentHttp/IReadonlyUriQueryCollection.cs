using System.Collections.Generic;

namespace AonWeb.FluentHttp
{
    public interface IReadonlyUriQueryCollection : IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>
        /// Converts to a string QueryString formatted string i.e. "key1=val1&amp;key2=val2" suitable for use in a Url query string.
        /// </summary>
        /// <returns>A QueryString formatted string i.e. "key1=val1&amp;key2=val2"</returns>
        string ToEncodedString();
        IDictionary<string, ISet<string>> ToDictionary();
    }
}