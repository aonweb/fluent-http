namespace AonWeb.FluentHttp.Mocks
{
    public interface IMockFormatterFactory : IFormatterFactory
    {
        new MockFormatter Create();
    }
}