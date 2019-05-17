namespace AonWeb.FluentHttp.GraphQL.Serialization
{
    public interface IRelayPageInfo
    {
        bool? HasPreviousPage { get; }
        bool? HasNextPage { get; }
    }
}