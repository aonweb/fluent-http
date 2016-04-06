using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.GraphQL.Serialization;
using AonWeb.FluentHttp.Settings;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.GraphQL
{
    public static class GraphQLBuilderExtensions
    {
        const string RelayVariablesKey = "RelayVariables";

        internal enum PayloadType
        {
            Query,
            Mutation
        }

        public static IGraphQLBuilder WithServer(this IGraphQLBuilder builder, string serverUri)
        {
            return builder.Advanced.WithConfiguration(b => b.WithUri(serverUri));
        }

        public static IGraphQLBuilder WithServer(this IGraphQLBuilder builder, Uri serverUri)
        {
            return builder.Advanced.WithConfiguration(b => b.WithUri(serverUri));
        }

        public static IGraphQLQueryBuilder WithQuery(this IGraphQLBuilder builder, string query)
        {
            return builder.WithQuery(() => query);
        }

        public static IGraphQLQueryBuilder WithQuery(this IGraphQLBuilder builder, Func<string> query)
        {
            builder.WithPayload(query, PayloadType.Query);

            return (IGraphQLQueryBuilder)builder;
        }

        public static IGraphQLMutationBuilder WithMutation(this IGraphQLBuilder builder, string mutation)
        {
            return builder.WithMutation(() => mutation);
        }

        public static IGraphQLMutationBuilder WithMutation(this IGraphQLBuilder builder, Func<string> mutation)
        {
            builder.WithPayload(mutation, PayloadType.Mutation);

            return (IGraphQLMutationBuilder)builder;
        }

        public static Task<TResult> QueryAsync<TResult>(this IGraphQLQueryBuilder builder) where TResult : IGraphQLQueryResult
        {
            return builder.QueryAsync<TResult>(CancellationToken.None);
        }


        public static Task<RelayQueryResult<TViewer>> RelayQueryAsync<TViewer>(this IGraphQLQueryBuilder builder)
            where TViewer : IRelayViewer
        {
            return builder.RelayQueryAsync<TViewer>(CancellationToken.None);
        }

        public static Task<RelayQueryResult<TViewer>> RelayQueryAsync<TViewer>(this IGraphQLQueryBuilder builder, CancellationToken token)
            where TViewer : IRelayViewer
        {
            return builder.QueryAsync<RelayQueryResult<TViewer>>(token);
        }

        public static Task<TResult> MutateAsync<TResult>(this IGraphQLMutationBuilder builder) where TResult : IGraphQLMutationResult
        {
            return builder.MutateAsync<TResult>(CancellationToken.None);
        }

        public static TBuilder WithVariable<TBuilder>(this IGraphQLBuilder builder, string name, object value)
            where TBuilder : IGraphQLBuilder
        {
            builder.CreateAndApplyVariables(variables =>
            {

                variables[name] = value;
            });

            return (TBuilder)builder;
        }

        public static TBuilder WithVariables<TBuilder>(this IGraphQLBuilder builder, IEnumerable<KeyValuePair<string, string>> values)
            where TBuilder : IGraphQLBuilder
        {
            builder.CreateAndApplyVariables(variables =>
            {
                foreach (var value in values)
                {
                    variables[value.Key] = value.Value;
                }
            });

            return (TBuilder)builder;
        }

        public static TBuilder WithOptionalVariable<TBuilder>(this IGraphQLBuilder builder, string name, object value)
            where TBuilder : IGraphQLBuilder
        {
            return builder.WithOptionalVariable<TBuilder, object>(name, value);
        }

        public static TBuilder WithOptionalVariable<TBuilder, TValue>(this IGraphQLBuilder builder, string name, TValue value, Func<TValue, bool> nullCheck = null)
             where TBuilder : IGraphQLBuilder
        {

            if (nullCheck == null)
                nullCheck = v => value == null;

            if (nullCheck(value))
                return (TBuilder)builder;

            return builder.WithVariable<TBuilder>(name, value);
        }

        private static void CreateAndApplyVariables(this IGraphQLBuilder builder, Action<IDictionary<string, object>> configuration)
        {
            builder.Advanced.WithConfiguration((ITypedBuilderSettings settings) =>
            {
                var variables = settings.Items[RelayVariablesKey] as IDictionary<string, object>;

                if (variables == null)
                {
                    variables = new Dictionary<string, object>();
                    settings.Items[RelayVariablesKey] = variables;
                }

                configuration?.Invoke(variables);
            });
        }


        private static void WithPayload(this IGraphQLBuilder builder, Func<string> payloadFactory, PayloadType payloadType)
        {
            builder.Advanced.WithConfiguration(innerBuilder =>
            {
                if (payloadFactory == null)
                    throw new ArgumentNullException(nameof(payloadFactory));

                innerBuilder.WithContentEncoding(Encoding.UTF8);
                innerBuilder.WithMediaType("application/json");

                innerBuilder.WithConfiguration(s =>
                {
                    s.WithDefiniteContentType(typeof(GraphQLPayload));

                    s.ContentFactory = () =>
                    {
                        var payload = (payloadFactory() ?? string.Empty).Trim().Replace(Environment.NewLine, "\n").Replace("\t", " ");

                        return new GraphQLPayload
                        {
                            Query = payload,
                            Variables = s.Items[RelayVariablesKey] as IDictionary<string, object>
                        };

                    };
                });
            });
        }
    }
}