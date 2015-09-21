namespace AonWeb.FluentHttp
{
    public class TypedBuilderFactory : ITypedBuilderFactory
    {
        private readonly IHttpBuilderFactory _httpBuilderFactory;

        public TypedBuilderFactory()
            :this(new HttpBuilderFactory()) { }

        public TypedBuilderFactory(IHttpBuilderFactory httpBuilderFactory)
        {
            _httpBuilderFactory = httpBuilderFactory;
        }

        public ITypedBuilder Create()
        {
            var child = _httpBuilderFactory.CreateAsChild();
            var settings = new TypedBuilderSettings();

            var builder = new TypedBuilder(settings, child, new Formatter(), Defaults.TypedBuilder.HandlerFactory());

            settings.Builder = builder;

            Defaults.Factory.DefaultTypedBuilderConfiguration?.Invoke(builder);

            return builder;
        }

        public IChildTypedBuilder CreateAsChild()
        {
            var child = _httpBuilderFactory.CreateAsChild();
            var settings = new TypedBuilderSettings();

            var builder = new TypedBuilder(settings, child, new Formatter(), Defaults.TypedBuilder.ChildHandlerFactory());

            settings.Builder = builder;

            return builder;
        }
    }
}