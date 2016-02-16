namespace AonWeb.FluentHttp
{
    public interface IBuilderConfiguration<in T>
    {
        void Configure(T builder);
    }
}