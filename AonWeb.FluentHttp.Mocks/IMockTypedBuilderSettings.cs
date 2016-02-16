using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp.Mocks
{
    public interface IMockTypedBuilderSettings: ITypedBuilderSettings, ITypedResultMocker<IMockTypedBuilderSettings>
    {
    }
}