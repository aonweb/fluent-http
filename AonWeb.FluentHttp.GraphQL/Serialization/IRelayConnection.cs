using System.Collections.Generic;

namespace AonWeb.FluentHttp.GraphQL.Serialization
{
    public interface IRelayConnection<TNode, out TPageInfo>
        where TNode : IRelayNode
        where TPageInfo : IRelayPageInfo
    {
        IList<RelayEdge<TNode>> Edges { get; }
        TPageInfo PageInfo { get; }
    }
}