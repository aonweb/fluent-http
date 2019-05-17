namespace AonWeb.FluentHttp.GraphQL.Serialization
{
    public interface IRelayMutationResult : IGraphQLMutationResult
    {
        string ClientMutationId { get; }
    }
}