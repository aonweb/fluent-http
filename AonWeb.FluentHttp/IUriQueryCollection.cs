using System.Collections.Generic;

namespace AonWeb.FluentHttp
{
    public interface IUriQueryCollection : IReadonlyUriQueryCollection
    {
        IUriQueryCollection Add(string name, string value);
        IUriQueryCollection Add(IEnumerable<KeyValuePair<string, string>> values);
        IUriQueryCollection Clear();
    }
}