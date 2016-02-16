using System;
using System.Collections.Generic;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp
{
    public static class CachingExtensions
    {
        public static TBuilder WithDependentUris<TBuilder>(this IAdvancedCacheConfigurable<TBuilder> handler, IEnumerable<Uri> uris)
            where TBuilder : IAdvancedCacheConfigurable<TBuilder>
        {
            if (uris == null)
                return (TBuilder)handler;

            foreach (var uri in uris)
                handler.WithDependentUri(uri);

            return (TBuilder)handler;
        }

        public static TBuilder WithDependentUri<TBuilder>(this IAdvancedCacheConfigurable<TBuilder> handler, Uri uri)
            where TBuilder : IAdvancedCacheConfigurable<TBuilder>
        {
            if (uri == null)
                return (TBuilder)handler;

            handler.WithConfiguration(s =>
            {

                uri = uri.Normalize();

                if (uri != null && !s.DependentUris.Contains(uri))
                    s.DependentUris.Add(uri);
            });

            return (TBuilder)handler;
        }

        public static TBuilder WithCacheDuration<TBuilder>(this IAdvancedCacheConfigurable<TBuilder> handler, TimeSpan? duration)
            where TBuilder : IAdvancedCacheConfigurable<TBuilder>
        {
            handler.WithConfiguration(s => s.CacheDuration = duration);

            return (TBuilder)handler;
        }

        public static TBuilder WithDefaultDurationForCacheableResults<TBuilder>(this IAdvancedCacheConfigurable<TBuilder> handler, TimeSpan? duration)
            where TBuilder : IAdvancedCacheConfigurable<TBuilder>
        {
            handler.WithConfiguration(s => s.DefaultDurationForCacheableResults = duration);

            return (TBuilder)handler;
        }

        public static TBuilder WithDefaultVaryByHeaders<TBuilder>(this IAdvancedCacheConfigurable<TBuilder> handler, IEnumerable<string> vary)
            where TBuilder : IAdvancedCacheConfigurable<TBuilder>
        {
            handler.WithConfiguration(s =>
            {
                s.DefaultVaryByHeaders.Clear();

                s.DefaultVaryByHeaders.Merge(vary);
            });

            return (TBuilder)handler;
        }

        public static TBuilder WithAdditionalDefaultVaryByHeaders<TBuilder>(this IAdvancedCacheConfigurable<TBuilder> handler, IEnumerable<string> vary)
            where TBuilder : IAdvancedCacheConfigurable<TBuilder>
        {
            handler.WithConfiguration(s => s.DefaultVaryByHeaders.Merge(vary));

            return (TBuilder)handler;
        }

        public static TBuilder WithCacheHandler<TBuilder>(this IAdvancedCacheConfigurable<TBuilder> handler, ICacheHandler cacheHandler)
            where TBuilder : IAdvancedCacheConfigurable<TBuilder>
        {
            handler.WithConfiguration(s => s.HandlerRegister.WithHandler(cacheHandler));

            return (TBuilder)handler;
        }

        public static TBuilder WithCacheHandlerConfiguration<TBuilder, THandler>(this IAdvancedCacheConfigurable<TBuilder> handler, Action<THandler> configure)
            where TBuilder : IAdvancedCacheConfigurable<TBuilder>
            where THandler : class, ICacheHandler
        {
            handler.WithConfiguration(s => s.HandlerRegister.WithHandlerConfiguration(configure));

            return (TBuilder)handler;
        }

        public static TBuilder WithOptionalCacheHandlerConfiguration<TBuilder, THandler>(this IAdvancedCacheConfigurable<TBuilder> handler, Action<THandler> configure)
            where TBuilder : IAdvancedCacheConfigurable<TBuilder>
            where THandler : class, ICacheHandler
        {
            handler.WithConfiguration(s => s.HandlerRegister.WithHandlerConfiguration(configure, false));

            return (TBuilder)handler;
        }
    }
}