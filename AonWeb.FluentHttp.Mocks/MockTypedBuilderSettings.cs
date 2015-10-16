using System;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockTypedBuilderSettings : TypedBuilderSettings, IMockTypedBuilderSettings
    {
        private readonly MockFormatter _formatter;

        public MockTypedBuilderSettings(MockFormatter formatter): base(formatter)
        {
            _formatter = formatter;
        }

        internal void SetBuilder(IChildTypedBuilder builder)
        {
            Builder = builder;
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