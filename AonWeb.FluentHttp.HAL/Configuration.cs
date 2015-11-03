using System.Net.Http.Formatting;
using AonWeb.FluentHttp.HAL.Serialization;
using Newtonsoft.Json.Serialization;

namespace AonWeb.FluentHttp.HAL
{
    public class HalConfiguration: IBuilderConfiguration<IHalBuilder>
    {
        public void Configure(IHalBuilder builder)
        {
            builder.Advanced.WithMediaTypeFormatterConfiguration<JsonMediaTypeFormatter>(
                f =>
                {
                    f.SerializerSettings.Converters.Add(new HalResourceConverter());
                    f.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
        }
    }
}