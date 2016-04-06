namespace AonWeb.FluentHttp.GraphQL
{
    public interface IAdvancedGraphQLBuilder : IGraphQLBuilder,
        IAdvancedHttpBuilderCore<IAdvancedGraphQLBuilder>,
        IFluentConfigurable<IAdvancedGraphQLBuilder, IAdvancedTypedBuilder>,
        IAdvancedCacheConfigurable<IAdvancedGraphQLBuilder>
    {

    }
}