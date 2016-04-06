using System.Linq;
using System.Threading.Tasks;
using AonWeb.FluentHttp.GraphQL;
using AonWeb.FluentHttp.GraphQL.Serialization;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests.GraphQL
{
    public class GraphQLBuilderTests : IClassFixture<GraphQLFixture>
    {
        private readonly GraphQLFixture _fixture;
        private readonly ITestOutputHelper _logger;

        public GraphQLBuilderTests(GraphQLFixture fixture, ITestOutputHelper logger)
        {
            _fixture = fixture;
            _logger = logger;
        }

        private static IGraphQLBuilder CreateBuilder()
        {
            var builder = new GraphQLBuilderFactory().Create();

            return builder;
        }

        [Fact]
        public async Task WhenValidQuery_WithValidResponse_ExpectValidResult()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {

                server.WithNextResponseOk(_fixture.MovieResponse);


                var actual = await CreateBuilder()
                    .WithServer(server.ListeningUri)
                    .WithQuery(_fixture.MoviesQuery)
                    .RelayQueryAsync<RelayViewer>();

                actual.ShouldNotBeNull();
            }
        }
    }

    public class GraphQLFixture
    {
        public string MoviesQuery = @"
{
viewer{
    movies(availability: NOW_PLAYING){
      edges{
        node{
          id
          name
        }
      }
    }
  }
}";

        public string MovieResponse = @"{
  ""data"": {
    ""viewer"": {
      ""movies"": {
        ""edges"": [
          {
            ""node"": {
              ""id"": ""TW92aWU6NDIwNDY="",
              ""name"": ""Jurassic World""
            }
},
          {
            ""node"": {
              ""id"": ""TW92aWU6NDA1NTU="",
              ""name"": ""Inside Out""
            }
          },
          {
            ""node"": {
              ""id"": ""TW92aWU6NDM5NTI="",
              ""name"": ""Ted 2""
            }
          },
          {
            ""node"": {
              ""id"": ""TW92aWU6NDcyNzc="",
              ""name"": ""Dope""
            }
          },
          {
            ""node"": {
              ""id"": ""TW92aWU6NDQxOTg="",
              ""name"": ""Spy""
            }
          },
          {
            ""node"": {
              ""id"": ""TW92aWU6NDU1Mjc="",
              ""name"": ""Max""
            }
          },
          {
            ""node"": {
              ""id"": ""TW92aWU6NDQzMjc="",
              ""name"": ""San Andreas""
            }
          },
          {
            ""node"": {
              ""id"": ""TW92aWU6NDQyNDQ="",
              ""name"": ""Insidious Chapter 3""
            }
          },
          {
            ""node"": {
              ""id"": ""TW92aWU6NDEwOTk="",
              ""name"": ""Avengers: Age Of Ultron""
            }
          },
          {
            ""node"": {
              ""id"": ""TW92aWU6NDQyNTc="",
              ""name"": ""Mad Max: Fury Road""
            }
          }
        ]
      }
    }
  }
}
";
    }

    public class RelayViewer : IRelayViewer
    {
        public GraphQLMovies Movies { get; set; }
    }

    public class GraphQLMovies : RelayConnection<GraphQLMovie>
    {

    }

    public class GraphQLMovie : RelayNode
    {
        public string Name { get; set; }
    }
}
