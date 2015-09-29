namespace AonWeb.FluentHttp.Mocks
{
    public interface IMockHttpBuilderFactory : IHttpBuilderFactory
    {
        new IMockHttpBuilder Create();
        new IMockHttpBuilder CreateAsChild();
    }
}