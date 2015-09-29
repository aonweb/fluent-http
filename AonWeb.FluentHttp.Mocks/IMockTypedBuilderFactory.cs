namespace AonWeb.FluentHttp.Mocks
{
    public interface IMockTypedBuilderFactory : ITypedBuilderFactory
    {
        new IMockTypedBuilder Create();
        new IMockTypedBuilder CreateAsChild();
    }
}