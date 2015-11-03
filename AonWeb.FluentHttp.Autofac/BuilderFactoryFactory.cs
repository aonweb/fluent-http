using AonWeb.FluentHttp.HAL;
using Autofac;

namespace AonWeb.FluentHttp.Autofac
{
    public class BuilderFactoryFactory
    {
        private readonly ILifetimeScope _scope;

        public BuilderFactoryFactory(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public IHttpBuilderFactory CreateHttpBuilderFactory()
        {
            var builderScope = _scope.Tag.ToString() == Constants.BuilderScopeTag ? _scope : _scope.BeginLifetimeScope(Constants.BuilderScopeTag);
            return builderScope.Resolve<ProxyHttpBuilderFactory>();
        }
        public ITypedBuilderFactory CreateTypedBuilderFactory()
        {
            var builderScope = _scope.Tag.ToString() == Constants.BuilderScopeTag ? _scope : _scope.BeginLifetimeScope(Constants.BuilderScopeTag);
            return builderScope.Resolve<ProxyTypedBuilderFactory>();
        }
        public IHalBuilderFactory CreateHalBuilderFactory()
        {
            var builderScope = _scope.Tag.ToString() == Constants.BuilderScopeTag ? _scope : _scope.BeginLifetimeScope(Constants.BuilderScopeTag);
            return builderScope.Resolve<ProxyHalBuilderFactory>();
        }
    }
}