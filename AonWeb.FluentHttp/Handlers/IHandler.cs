namespace AonWeb.FluentHttp.Handlers
{
    public interface IHandler<in THandlerType>
    {
        bool Enabled { get; }
        HandlerPriority GetPriority(THandlerType type);
    }
}