namespace AonWeb.FluentHttp.Mocks
{
    public class MockTypedBuilderSettings : TypedBuilderSettings
    {
        internal void SetBuilder(IChildTypedBuilder builder)
        {
            Builder = builder;
        }
    }
}