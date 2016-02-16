namespace AonWeb.FluentHttp
{
    public interface IHttpBuilderFactory : IBuilderFactory<IHttpBuilder>, IChildIBuilderFactory<IChildHttpBuilder>
    {
    }
}