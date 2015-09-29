
using System.Collections.Generic;

namespace AonWeb.FluentHttp
{
    public interface IUriQueryCollection : IReadonlyUriQueryCollection
    {
        IEnumerable<string> this[string name] { get; set; }
        IUriQueryCollection Set(string name, string value);
        IUriQueryCollection Set(string name, IEnumerable<string> values);
        IUriQueryCollection Set(IEnumerable<KeyValuePair<string, string>> values);
        IUriQueryCollection Add(string name, string value);
        IUriQueryCollection Add(IEnumerable<KeyValuePair<string, string>> values);

        IUriQueryCollection Clear();
    }
}