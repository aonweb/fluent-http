namespace AonWeb.FluentHttp
{
    public class TypedBuilderFactory : ITypedBuilderFactory
    {
        private readonly IHttpBuilderFactory _httpBuilderFactory;
        private readonly IFormatterFactory _formatterFactory;

        public TypedBuilderFactory()
            :this(new HttpBuilderFactory(), new FormatterFactory()) { }

        public TypedBuilderFactory(IHttpBuilderFactory httpBuilderFactory, IFormatterFactory formatterFactory)
        {
            _httpBuilderFactory = httpBuilderFactory;
            _formatterFactory = formatterFactory;
        }

        public ITypedBuilder Create()
        {
            var child = _httpBuilderFactory.CreateAsChild();
            var formatter = _formatterFactory.Create();
            var settings = new TypedBuilderSettings(formatter);

            var builder = new TypedBuilder(settings, child, Defaults.Current.GetTypedBuilderDefaults().Handlers.GetHandlers(settings));

            settings.Builder = builder;

            Defaults.Current.GetTypedBuilderDefaults().DefaultBuilderConfiguration?.Invoke(builder);

            return builder;
        }

        public IChildTypedBuilder CreateAsChild()
        {
            var child = _httpBuilderFactory.CreateAsChild();
            var formatter = _formatterFactory.Create();
            var settings = new TypedBuilderSettings(formatter);

            var builder = new TypedBuilder(settings, child, Defaults.Current.GetTypedBuilderDefaults().ChildHandlers.GetHandlers(settings));

            settings.Builder = builder;

            return builder;
        }
    }

    public interface IFormatterFactory
    {
        IFormatter Create();
    }

    public class FormatterFactory: IFormatterFactory
    {
        public IFormatter Create()
        {
            return new Formatter();
        }
    }
}