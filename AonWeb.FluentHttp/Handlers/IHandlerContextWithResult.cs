namespace AonWeb.FluentHttp.Handlers
{
    public interface IHandlerContextWithResult<in TResult> : IHandlerContext
    {
        TResult Result { set; }
    }

    public interface IHandlerContextWithResult: IHandlerContext
    {
        object Result { set; }
    }
}