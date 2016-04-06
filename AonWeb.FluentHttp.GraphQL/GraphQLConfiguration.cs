using System.Net.Http.Formatting;
using Newtonsoft.Json.Serialization;

namespace AonWeb.FluentHttp.GraphQL
{
    public class GraphQLConfiguration: IBuilderConfiguration<IGraphQLBuilder>
    {
        public void Configure(IGraphQLBuilder builder)
        {
            builder.Advanced.WithConfiguration(innerBuilder => 
                innerBuilder
                .AsPost()
                .Advanced
                    .WithMediaTypeFormatterConfiguration<JsonMediaTypeFormatter>(f =>
                    {
                        f.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    }));
        }
    }
}