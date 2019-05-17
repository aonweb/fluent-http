using System;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.GraphQL.Serialization;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp.GraphQL
{
    public class GraphQLBuilder : IAdvancedGraphQLBuilder, IGraphQLQueryBuilder, IGraphQLMutationBuilder
    {
        private readonly IChildTypedBuilder _innerBuilder;

        public GraphQLBuilder(IChildTypedBuilder innerBuilder)
        {
            _innerBuilder = innerBuilder;
        }

        public IAdvancedGraphQLBuilder Advanced => this;

        public IAdvancedGraphQLBuilder WithClientConfiguration(Action<IHttpClientBuilder> configuration)
        {
            _innerBuilder.WithClientConfiguration(configuration);

            return this;
        }

        public async Task<TResult> QueryAsync<TResult>(CancellationToken token) where TResult : IGraphQLQueryResult
        {
            var result = await _innerBuilder.ResultAsync<GraphQLData<TResult>>(token);

            return result.Data;
        }

        public async Task<TResult> MutateAsync<TResult>(CancellationToken token) where TResult : IGraphQLMutationResult
        {
            var result = await _innerBuilder.ResultAsync<GraphQLData<TResult>>(token);

            return result.Data;
        }

        public void CancelRequest()
        {
            _innerBuilder.CancelRequest();
        }

        public IGraphQLBuilder WithConfiguration(Action<ITypedBuilderSettings> configuration)
        {
            _innerBuilder.WithConfiguration(configuration);

            return this;
        }

        public IAdvancedGraphQLBuilder WithConfiguration(Action<IAdvancedTypedBuilder> configuration)
        {
            configuration?.Invoke(_innerBuilder);

            return this;
        }

        public IAdvancedGraphQLBuilder WithConfiguration(Action<ICacheSettings> configuration)
        {
            _innerBuilder.WithConfiguration(configuration);

            return this;
        }

        void IConfigurable<ITypedBuilderSettings>.WithConfiguration(Action<ITypedBuilderSettings> configuration)
        {
            WithConfiguration(configuration);
        }

        void IConfigurable<IAdvancedTypedBuilder>.WithConfiguration(Action<IAdvancedTypedBuilder> configuration)
        {
            WithConfiguration(configuration);
        }

        void IConfigurable<IHttpBuilderSettings>.WithConfiguration(Action<IHttpBuilderSettings> configuration)
        {
            _innerBuilder.WithConfiguration(configuration);
        }

        void IConfigurable<ICacheSettings>.WithConfiguration(Action<ICacheSettings> configuration)
        {
            WithConfiguration(configuration);
        }

        IGraphQLQueryBuilder IFluentConfigurable<IGraphQLQueryBuilder, ITypedBuilderSettings>.WithConfiguration(Action<ITypedBuilderSettings> configuration)
        {
            WithConfiguration(configuration);

            return this;
        }
    }
}
