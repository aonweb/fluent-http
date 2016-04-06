using System.Collections.Generic;

namespace AonWeb.FluentHttp.GraphQL.Serialization
{
    public class GraphQLError
    {
        public string Message { get; set; }
        public IList<GraphQLErrorLocation> Locations { get; set; }
    }
}