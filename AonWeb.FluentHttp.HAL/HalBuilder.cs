using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.HAL.Representations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.HAL
{
    public class HalBuilder : IAdvancedHalBuilder
    {
        private readonly IChildTypedBuilder _innerInnerBuilder;

        public HalBuilder(IChildTypedBuilder innerBuilder)
        {
            _innerInnerBuilder = innerBuilder;
        }

        public IAdvancedHalBuilder Advanced => this;

        

        

        

        //public IAdvancedHalBuilder WithDependentResources(params IHalResource[] resources)
        //{
        //    if (resources == null) 
        //        return this;

        //    var uris = resources.Select(r => r.Links.Self());

        //    _innerBuilder.WithDependentUris(uris);

        //    return this;
        //}

        //public IAdvancedHalBuilder WithDependentLink(Uri link)
        //{
        //    _innerBuilder.WithDependentUri(link);

        //    return this;
        //}

        //public IAdvancedHalBuilder WithDependentLink(Func<Uri> linkFactory)
        //{
        //    if (linkFactory == null)
        //        throw new ArgumentNullException(nameof(linkFactory));

        //    _innerBuilder.WithDependentUri(linkFactory());

        //    return this;
        //}


        public IAdvancedHalBuilder WithClientConfiguration(Action<IHttpClientBuilder> configuration)
        {
            _innerInnerBuilder.WithClientConfiguration(configuration);

            return this;
        }

        public void CancelRequest()
        {
            _innerInnerBuilder.CancelRequest();
        }

        public async Task<TResult> ResultAsync<TResult>()
            where TResult : IHalResource
        {
            return await _innerInnerBuilder.ResultAsync<TResult>().ConfigureAwait(false);
        }

        public async Task<TResult> ResultAsync<TResult>(CancellationToken token)
            where TResult : IHalResource
        {
            return await _innerInnerBuilder.ResultAsync<TResult>(token).ConfigureAwait(false);
        }

        public async Task SendAsync()
        {
            await _innerInnerBuilder.SendAsync().ConfigureAwait(false);
        }

        public async Task SendAsync(CancellationToken token)
        {
            await _innerInnerBuilder.SendAsync(token).ConfigureAwait(false);
        }

        public IHalBuilder WithConfiguration(Action<ITypedBuilderSettings> configuration)
        {
            _innerInnerBuilder.WithConfiguration(configuration);

            return this;
        }

        public IAdvancedHalBuilder WithConfiguration(Action<IAdvancedTypedBuilder> configuration)
        {
            configuration?.Invoke(_innerInnerBuilder);

            return this;
        }

        void IConfigurable<ITypedBuilderSettings>.WithConfiguration(Action<ITypedBuilderSettings> configuration)
        {
            WithConfiguration(configuration);
        }

        void IConfigurable<IAdvancedTypedBuilder>.WithConfiguration(Action<IAdvancedTypedBuilder> configuration)
        {
            WithConfiguration(configuration);
        }
    }
}
