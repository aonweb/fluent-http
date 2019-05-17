using System;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.GraphQL.Serialization;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp.GraphQL
{
    public interface IGraphQLBuilder :
        IFluentConfigurableWithAdvanced<IGraphQLBuilder, IAdvancedGraphQLBuilder>,
        IFluentConfigurable<IGraphQLBuilder, ITypedBuilderSettings>
    {
        void CancelRequest();
    }

    public interface IGraphQLQueryBuilder : IGraphQLBuilder,
        IFluentConfigurable<IGraphQLQueryBuilder, ITypedBuilderSettings>
    {

        Task<TResult> QueryAsync<TResult>(CancellationToken token)
            where TResult : IGraphQLQueryResult;
    }

    public interface IGraphQLMutationBuilder : IGraphQLBuilder,
        IFluentConfigurable<IGraphQLQueryBuilder, ITypedBuilderSettings>
    {
        Task<TResult> MutateAsync<TResult>(CancellationToken token)
            where TResult : IGraphQLMutationResult;
    }
}