using System.Collections.Generic;

namespace AonWeb.FluentHttp.GraphQL.Serialization
{
    public class GraphQLPayload
    {
        public string Query { get; set; }
        public IDictionary<string, object> Variables { get; set; }
    }
}