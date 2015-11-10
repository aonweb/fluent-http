using System.Collections.Generic;

namespace AonWeb.FluentHttp.Serialization
{
    public interface IUriQueryCollection : IReadonlyUriQueryCollection
    {
        IEnumerable<string> this[string name] { get; set; }
        IUriQueryCollection Set(string name, IEnumerable<string> values);
        IUriQueryCollection Add(string name, string value);
        IUriQueryCollection Clear();
    }
}