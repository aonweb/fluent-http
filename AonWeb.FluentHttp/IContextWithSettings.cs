namespace AonWeb.FluentHttp
{
    public interface IContextWithSettings<out TSettings> : IContext
    {
        TSettings GetSettings();
    }
}