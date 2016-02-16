namespace AonWeb.FluentHttp.Mocks
{
    public interface IMockResult: IMaybeTransient
    {
        object Result { get; set; }
    }

    public interface IMockResult<TResult>: IMockResult
    {
        new TResult Result { get; set; }
    }
}