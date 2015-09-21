namespace AonWeb.FluentHttp
{
    public interface ITypedBuilderFactory
    {
        ITypedBuilder Create();
        IChildTypedBuilder CreateAsChild();
    }
}