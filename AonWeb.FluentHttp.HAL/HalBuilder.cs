using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.HAL.Representations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.HAL
{
    public class HalBuilder : IAdvancedHalBuilder
    {
        private readonly IChildTypedBuilder _innerBuilder;

        public HalBuilder(IChildTypedBuilder builder)
        {
            _innerBuilder = builder;
        }

        public IAdvancedHalBuilder Advanced => this;

        //public IHalBuilder WithLink(string link)
        //{
        //    _innerBuilder.WithUri(link);

        //    return this;
        //}

        //public IHalBuilder WithLink(Uri link)
        //{
        //    _innerBuilder.WithUri(link);

        //    return this;
        //}

        //public IHalBuilder WithLink(Func<string> linkFactory)
        //{
        //    if (linkFactory == null)
        //        throw new ArgumentNullException("linkFactory");

        //    return WithLink(linkFactory());
        //}

        //public IHalBuilder WithLink(Func<Uri> linkFactory)
        //{
        //    if (linkFactory == null)
        //        throw new ArgumentNullException("linkFactory");

        //    return WithLink(linkFactory());
        //}

        //public IHalBuilder WithLink(IHalResource resource, string key, string tokenKey, object tokenValue)
        //{
        //    return WithLink(resource.GetLink(key, tokenKey, tokenValue));
        //}

        //public IHalBuilder WithLink(IHalResource resource, string key, IDictionary<string, object> tokens)
        //{
        //    return WithLink(resource.GetLink(key, tokens));
        //}

        //public IHalBuilder WithContent<TContent>(TContent content)
        //    where TContent : IHalRequest
        //{
        //    _innerBuilder.WithContent(CreateContentFactoryWrapper(content));

        //    return this;
        //}

        //public IHalBuilder WithContent<TContent>(TContent content, Encoding encoding)
        //    where TContent : IHalRequest
        //{
        //    _innerBuilder.WithContent(CreateContentFactoryWrapper(content), encoding);

        //    return this;
        //}

        //public IHalBuilder WithContent<TContent>(TContent content, Encoding encoding, string mediaType)
        //    where TContent : IHalRequest
        //{
        //    _innerBuilder.WithContent(CreateContentFactoryWrapper(content), encoding, mediaType);

        //    return this;
        //}

        //public IHalBuilder WithContent<TContent>(Func<TContent> contentFactory)
        //    where TContent : IHalRequest
        //{
        //    _innerBuilder.WithContent(CreateContentFactoryWrapper(contentFactory));

        //    return this;
        //}

        //public IHalBuilder WithContent<TContent>(Func<TContent> contentFactory, Encoding encoding)
        //    where TContent : IHalRequest
        //{
        //    _innerBuilder.WithContent(CreateContentFactoryWrapper(contentFactory), encoding);

        //    return this;
        //}

        //public IHalBuilder WithContent<TContent>(Func<TContent> contentFactory, Encoding encoding, string mediaType)
        //    where TContent : IHalRequest
        //{
        //    _innerBuilder.WithContent(CreateContentFactoryWrapper(contentFactory), encoding, mediaType);

        //    return this;
        //}

        //public IHalBuilder WithDefaultResult<TResult>(TResult result)
        //    where TResult : IHalResource
        //{
        //    _innerBuilder.WithDefaultResult(result);

        //    return this;
        //}

        //public IHalBuilder WithDefaultResult<TResult>(Func<TResult> resultFactory)
        //    where TResult : IHalResource
        //{
        //    _innerBuilder.WithDefaultResult(resultFactory);

        //    return this;
        //}

        //public IHalBuilder WithErrorType<TError>()
        //{
        //    _innerBuilder.WithErrorType<TError>();

        //    return this;
        //}

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
            _innerBuilder.WithClientConfiguration(configuration);

            return this;
        }

        

        
        public void CancelRequest()
        {
            _innerBuilder.CancelRequest();
        }

        public async Task<TResult> ResultAsync<TResult>()
            where TResult : IHalResource
        {
            return await _innerBuilder.ResultAsync<TResult>().ConfigureAwait(false);
        }

        public async Task<TResult> ResultAsync<TResult>(CancellationToken token)
            where TResult : IHalResource
        {
            return await _innerBuilder.ResultAsync<TResult>(token).ConfigureAwait(false);
        }

        public async Task SendAsync()
        {
            await _innerBuilder.SendAsync().ConfigureAwait(false);
        }

        public async Task SendAsync(CancellationToken token)
        {
            await _innerBuilder.SendAsync(token).ConfigureAwait(false);
        }

        private Func<TContent> CreateContentFactoryWrapper<TContent>(TContent content)
            where TContent : IHalRequest
        {
            return () =>
            {
                if (!ReferenceEquals(content, null))
                    _innerBuilder.WithDependentUris(content.DependentUris);

                return content;
            };
        }

        private Func<TContent> CreateContentFactoryWrapper<TContent>(Func<TContent> contentFactory)
            where TContent : IHalRequest
        {
            if (contentFactory == null)
                throw new ArgumentNullException(nameof(contentFactory));

            return () =>
            {
                var content = contentFactory();

                _innerBuilder.WithDependentUris(content.DependentUris);

                return content;
            };
        }



        public IHalBuilder WithConfiguration(Action<ITypedBuilderSettings> configuration)
        {
            _innerBuilder.WithConfiguration(configuration);

            return this;
        }

        public IAdvancedHalBuilder WithConfiguration(Action<IAdvancedTypedBuilder> configuration)
        {
            configuration?.Invoke(_innerBuilder);

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
