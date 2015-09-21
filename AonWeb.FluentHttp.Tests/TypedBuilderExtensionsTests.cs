namespace AonWeb.FluentHttp.Tests
{
    public class TypedBuilderExtensionsTests
    {
        public void CanUseInterface()
        {
            var builder = new TypedBuilderFactory().Create();
        }
    }
}