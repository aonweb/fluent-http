using System.Collections.Generic;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.GraphQL.Serialization
{

    public class RelayQueryResult<TViewer> : IRelayQueryResult<TViewer>
        where TViewer : IRelayViewer
    {
        public TViewer Viewer { get; set; }
        public IList<GraphQLError> Errors { get; set; }
    }

    public interface IRelayQueryResult<out TViewer> : IGraphQLQueryResult
        where TViewer : IRelayViewer
    {
        TViewer Viewer { get; }
    }

    public interface IGraphQLQueryResult
    {
        IList<GraphQLError> Errors { get; }
    }

    public class GraphQLData<T>
    {
        public T Data { get; set; }
    }
}