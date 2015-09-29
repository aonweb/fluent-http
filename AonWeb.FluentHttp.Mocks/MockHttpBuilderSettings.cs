namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpBuilderSettings : HttpBuilderSettings
    {
        internal void SetBuilder(IRecursiveHttpBuilder builder)
        {
            Builder = builder;
        }
    }
}