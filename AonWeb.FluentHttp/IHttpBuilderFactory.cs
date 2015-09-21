namespace AonWeb.FluentHttp
{
    public interface IHttpBuilderFactory
    {
        IHttpBuilder Create();
        IChildHttpBuilder CreateAsChild();
    }
}