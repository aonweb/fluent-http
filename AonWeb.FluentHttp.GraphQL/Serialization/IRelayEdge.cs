namespace AonWeb.FluentHttp.GraphQL.Serialization
{
    public interface IRelayEdge<out TNode>
        where TNode: IRelayNode
    {
        string Cursor { get; }
        TNode Node { get; }
    }
}