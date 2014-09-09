namespace AonWeb.FluentHttp.Handlers
{
    public interface IHttpCallHandlerContextWithResult<in TResult>: IHttpCallHandlerContext
    {
        TResult Result { set; }
    }
}