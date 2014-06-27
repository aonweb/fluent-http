using AonWeb.FluentHttp.HAL.Representations;

namespace AonWeb.FluentHttp.HAL
{
    public class HalCallBuilderSettings : TypedHttpCallBuilderSettings
    {
        public HalCallBuilderSettings()
            : base(typeof(EmptyHalResult), typeof(EmptyHalRequest), typeof(string)) { }
    }
}