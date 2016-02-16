namespace AonWeb.FluentHttp
{
    public interface IChildIBuilderFactory<out TBuilder>
    {
        TBuilder CreateAsChild();
    }
}