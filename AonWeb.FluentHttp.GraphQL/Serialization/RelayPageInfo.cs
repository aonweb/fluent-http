namespace AonWeb.FluentHttp.GraphQL.Serialization
{
    public class RelayPageInfo: IRelayPageInfo
    {
        public bool? HasPreviousPage { get; set; }
        public bool? HasNextPage { get; set; }
    }
}