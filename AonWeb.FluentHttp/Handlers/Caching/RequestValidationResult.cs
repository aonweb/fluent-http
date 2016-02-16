namespace AonWeb.FluentHttp.Handlers.Caching
{
    public enum RequestValidationResult
    {
        OK,
        NoRequestInfo,
        ResultIsEmpty,
        MethodNotCacheable,
        NoStore,
        ErrorOrException
    }
}