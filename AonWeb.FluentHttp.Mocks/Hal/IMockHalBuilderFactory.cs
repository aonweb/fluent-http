using AonWeb.FluentHttp.HAL;

namespace AonWeb.FluentHttp.Mocks
{
    public interface IMockHalBuilderFactory: IHalBuilderFactory
    {
        new IMockHalBuilder Create();
    }
}