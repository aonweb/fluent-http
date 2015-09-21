namespace AonWeb.FluentHttp.Handlers
{
    public interface IHandlerContextWithResult<in TResult> : IHandlerContext, IContextWithResult<TResult>
    {
    }

    public interface IHandlerContextWithResult: IHandlerContext
    {
        object Result { set; }
    }
}