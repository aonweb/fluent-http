using System.Net.Http.Formatting;
using AonWeb.FluentHttp.HAL.Serialization;
using Newtonsoft.Json.Serialization;

namespace AonWeb.FluentHttp.HAL
{
    public class HalBuilderFactory : IHalBuilderFactory
    {
        private readonly ITypedBuilderFactory _innerBuilderFactory;

        public HalBuilderFactory()
            :this(new TypedBuilderFactory()) { }

        public HalBuilderFactory(ITypedBuilderFactory httpBuilderFactory)
        {
            _innerBuilderFactory = httpBuilderFactory;
        }

        public IHalBuilder Create()
        {
            var child = _innerBuilderFactory.CreateAsChild();

            var builder = new HalBuilder(child);

            builder.WithMediaTypeFormatterConfiguration<JsonMediaTypeFormatter>(
                f =>
                {
                    f.SerializerSettings.Converters.Add(new HalResourceConverter());
                    f.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            HalDefaults.Factory.DefaultHalBuilderConfiguration?.Invoke(builder);

            return builder;
        }
    }
}