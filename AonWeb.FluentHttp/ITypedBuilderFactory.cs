namespace AonWeb.FluentHttp
{
    public interface ITypedBuilderFactory: IBuilderFactory<ITypedBuilder>, IChildIBuilderFactory<IChildTypedBuilder>
    {
    }
}