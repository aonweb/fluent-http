using System.Net.Http.Formatting;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Serialization;
using Newtonsoft.Json.Serialization;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHalBuilderFactory : IMockHalBuilderFactory
    {
        private readonly IMockTypedBuilderFactory _innerBuilderFactory;

        public MockHalBuilderFactory()
            :this(new MockTypedBuilderFactory()) { }

        public MockHalBuilderFactory(IMockTypedBuilderFactory innerBuilderFactory)
        {
            _innerBuilderFactory = innerBuilderFactory;
        }

        public IMockHalBuilder Create()
        {
            var child = _innerBuilderFactory.CreateAsChild();

            var builder = new MockHalBuilder(child);

            builder.WithMediaTypeFormatterConfiguration<JsonMediaTypeFormatter>(
                f =>
                {
                    f.SerializerSettings.Converters.Add(new HalResourceConverter());
                    f.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            Defaults.Current.GetHalBuilderDefaults().DefaultBuilderConfiguration?.Invoke(builder);

            return builder;
        }

        IHalBuilder IHalBuilderFactory.Create()
        {
            return Create();
        }
    }
}