using System.Collections.Generic;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpBuilderSettings : HttpBuilderSettings
    {
        public MockHttpBuilderSettings(ICacheSettings cacheSettings, IEnumerable<IHttpHandler> handlers, IEnumerable<IHttpResponseValidator> responseValidators) 
            : base(cacheSettings, handlers, responseValidators)
        {

        }
    }
}