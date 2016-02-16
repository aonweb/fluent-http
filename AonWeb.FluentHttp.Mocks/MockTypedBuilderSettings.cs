using System;
using System.Collections.Generic;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockTypedBuilderSettings : TypedBuilderSettings, IMockTypedBuilderSettings
    {
        private readonly IMockFormatter _formatter;

        public MockTypedBuilderSettings(
            IMockFormatter formatter,
            ICacheSettings cacheSettings,
            IEnumerable<ITypedHandler> handlers,
            IEnumerable<ITypedResponseValidator> responseValidators) 
                : base(formatter, cacheSettings, handlers, responseValidators)
        {
            _formatter = formatter;
        }

        public IMockTypedBuilderSettings WithResult<TResult>(Predicate<IMockTypedRequestContext> predicate, Func<IMockTypedRequestContext, IMockResult<TResult>> resultFactory)
        {
            _formatter.WithResult(predicate, resultFactory);

            return this;
        }

        public IMockTypedBuilderSettings WithError<TError>(Predicate<IMockTypedRequestContext> predicate, Func<IMockTypedRequestContext, IMockResult<TError>> errorFactory)
        {
            _formatter.WithError(predicate, errorFactory);

            return this;
        }
    }
}