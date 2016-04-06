using System.Collections.Generic;

namespace AonWeb.FluentHttp.GraphQL.Serialization
{
    public class RelayConnection<TNode> : RelayConnection<TNode, RelayPageInfo>
        where TNode : IRelayNode { }

    public class RelayConnection<TNode, TPageInfo>: IRelayConnection<TNode, TPageInfo>
        where TNode : IRelayNode
        where TPageInfo : IRelayPageInfo
    {
        public IList<RelayEdge<TNode>> Edges { get; set; }
        public TPageInfo PageInfo { get; set; }
    }
}
