namespace AonWeb.FluentHttp.Mocks
{
    public interface IMockTypedRequestContext : IMockRequestContext
    {
        IMockRequestContext Request { get; }
        ITypedBuilderContext BuilderContext { get; }
    }
}