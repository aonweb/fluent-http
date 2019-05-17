namespace AonWeb.FluentHttp.GraphQL.Serialization
{
    public class GraphQLErrorLocation
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public string Stack { get; set; }
    }
}