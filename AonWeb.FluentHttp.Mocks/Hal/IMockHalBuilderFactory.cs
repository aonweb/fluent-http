using AonWeb.FluentHttp.HAL;

namespace AonWeb.FluentHttp.Mocks.Hal
{
    public interface IMockHalBuilderFactory: IHalBuilderFactory
    {
        new IMockHalBuilder Create();
    }
}