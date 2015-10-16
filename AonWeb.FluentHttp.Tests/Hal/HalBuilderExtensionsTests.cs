using AonWeb.FluentHttp.HAL;

namespace AonWeb.FluentHttp.Tests.Hal
{
    public class HalBuilderExtensionsTests
    {
        public void CanUseInterface()
        {
            var builder = new HalBuilderFactory().Create();
        }
    }
}