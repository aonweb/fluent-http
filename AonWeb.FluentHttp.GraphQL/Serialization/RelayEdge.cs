namespace AonWeb.FluentHttp.GraphQL.Serialization
{
    public class RelayEdge<TNode> : IRelayEdge<TNode>
        where TNode: IRelayNode
    {
        public string Cursor { get; set; }
        public TNode Node { get; set; }
    }
}