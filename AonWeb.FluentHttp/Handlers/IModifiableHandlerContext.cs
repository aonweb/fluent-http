namespace AonWeb.FluentHttp.Handlers
{
    public interface IModifiableHandlerContext : IContext
    {
        Modifiable GetHandlerResult();
    }
}