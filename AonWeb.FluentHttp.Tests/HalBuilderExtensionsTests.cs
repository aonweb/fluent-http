using AonWeb.FluentHttp.HAL;

namespace AonWeb.FluentHttp.Tests
{
    public class HalBuilderExtensionsTests
    {
        public void CanUseInterface()
        {
            var builder = new HalBuilderFactory().Create();
        }
    }
}