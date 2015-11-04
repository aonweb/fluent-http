using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Autofac;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using Autofac;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests.AutofacTests
{
    public class RegistrationTests
    {
        private readonly ITestOutputHelper _logger;

        public RegistrationTests(ITestOutputHelper logger)
        {
            _logger = logger;
        }

        #region HttpBuilder

        [Fact]
        public async Task HttpBuilderFactory_CanResolveAndCall()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponseOk("Response");
                var factory = container.Resolve<IHttpBuilderFactory>();

                var builder = factory.Create();

                var response = await builder.WithUri(server.ListeningUri).Advanced.WithCaching(false).ResultAsync();

                response.StatusCode.ShouldBe(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task HttpBuilder_CanResolveAndCall()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponseOk("Response");

                var builder = container.Resolve<IHttpBuilder>();

                var response = await builder.WithUri(server.ListeningUri).Advanced.WithCaching(false).ResultAsync();

                response.StatusCode.ShouldBe(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task HttpBuilderFactory_ResolvesCustomConfigurations()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponseOk("Response");
                var factory = container.Resolve<IHttpBuilderFactory>();

                var builder = factory.Create();
                string actual = null;
                var response = await builder.WithUri(server.ListeningUri)
                    .Advanced
                    .WithCaching(false)
                    .OnSending(HandlerPriority.Last, context =>
                    {
                        actual = context.Items["CustomHttpConfiguration"] as string;
                    })
                    .ResultAsync();

                response.StatusCode.ShouldBe(HttpStatusCode.OK);
                actual.ShouldBe("CustomHttpConfiguration: It Works!");
            }
        }

        [Fact]
        public async Task HttpBuilderFactory_ResolvesCustomValidators()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponse(HttpStatusCode.Created);
                var factory = container.Resolve<IHttpBuilderFactory>();

                var builder = factory.Create();

               await Should.ThrowAsync<HttpRequestException>(builder.WithUri(server.ListeningUri).Advanced.WithCaching(false).ResultAsync());
            }
        }

        [Fact]
        public async Task HttpBuilderFactory_ResolvesCustomCacheHandlers()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponse(new MockHttpResponseMessage().WithPrivateCacheHeader());
                var factory = container.Resolve<IHttpBuilderFactory>();

                var builder = factory.Create();
                string actual = null;
                var response = await builder.WithUri(server.ListeningUri)
                    .Advanced.WithCaching(true)
                    .OnCacheMiss(HandlerPriority.Last, context =>
                    {
                        actual = context.Items["CustomHttpCacheHandler"] as string;
                    })
                    .ResultAsync();

                response.StatusCode.ShouldBe(HttpStatusCode.OK);
                actual.ShouldBe("CustomHttpCacheHandler: It Works!");
            }
        }

        [Fact]
        public async Task HttpBuilderFactory_ResolvesCustomHandlers()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponseOk("Response");
                var factory = container.Resolve<IHttpBuilderFactory>();

                var builder = factory.Create();

                string actual = null;
                var response = await builder.WithUri(server.ListeningUri)
                    .Advanced
                    .Advanced.WithCaching(false)
                    .OnSending(HandlerPriority.Last, context =>
                    {
                        actual = context.Items["CustomHttpHandler"] as string;
                    }).ResultAsync();
                response.StatusCode.ShouldBe(HttpStatusCode.OK);
                actual.ShouldBe("CustomHttpHandler: It Works!");
            }
        }

        [Fact]
        public async Task HttpBuilderFactory_TwoBuildersDonNotShareSameSettings()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponseOk("Response1")
                    .WithNextResponseOk("Response2");

                var factory = container.Resolve<IHttpBuilderFactory>();
                int actual1 = 0;
                int actual2 = 0;
                var builder1 = factory.Create().WithUri(server.ListeningUri)
                    .Advanced.WithCaching(false)
                    .WithContextItem("Shared", 1)
                    .OnSending(HandlerPriority.Last, context =>
                    {
                        actual1 = (int) context.Items["Shared"];
                    });
                var builder2 = factory.Create().WithUri(server.ListeningUri)
                    .Advanced.WithCaching(false)
                    .WithContextItem("Shared", 2)
                    .OnSending(HandlerPriority.Last, context =>
                    {
                        actual2 = (int)context.Items["Shared"];
                    });

                
                var response1 = await builder1.ResultAsync();
                var response2 = await builder2.ResultAsync();
                
                actual1.ShouldNotBe(actual2);
            }
        }

        #endregion

        #region TypedBuilder

        [Fact]
        public async Task TypedBuilderFactory_CanResolveAndCall()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponseOk(TestResult.SerializedDefault1);
                var factory = container.Resolve<ITypedBuilderFactory>();

                var builder = factory.Create();

                var result = await builder.WithUri(server.ListeningUri).Advanced.WithCaching(false).ResultAsync<TestResult>();

                result.ShouldBe(TestResult.Default1());
            }
        }

        [Fact]
        public async Task TypedBuilder_CanResolveAndCall()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponseOk("Typed Result");

                var builder = container.Resolve<ITypedBuilder>();

                var response = await builder.WithUri(server.ListeningUri).Advanced.WithCaching(false).ResultAsync<string>();
            
                response.ShouldBe("Typed Result");
            }
        }

        [Fact]
        public async Task TypedBuilderFactory_ResolvesCustomConfigurations()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponseOk("Typed Result");
                var factory = container.Resolve<ITypedBuilderFactory>();

                var builder = factory.Create();
                string actual = null;
                var result = await builder.WithUri(server.ListeningUri)
                    .Advanced
                    .WithCaching(false)
                    .OnSending(HandlerPriority.Last, context =>
                    {
                        actual = context.Items["CustomTypedConfiguration"] as string;
                    })
                    .ResultAsync<string>();

                result.ShouldBe("Typed Result");
                actual.ShouldBe("CustomTypedConfiguration: It Works!");
            }
        }

        [Fact]
        public async Task TypedBuilderFactory_ResolvesCustomValidators()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponse(HttpStatusCode.Created);
                var factory = container.Resolve<ITypedBuilderFactory>();

                var builder = factory.Create();

                await Should.ThrowAsync<HttpRequestException>(builder.WithUri(server.ListeningUri).Advanced.WithCaching(false).ResultAsync<string>());
            }
        }

        [Fact]
        public async Task TypedBuilderFactory_ResolvesCustomCacheHandlers()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponse(new MockHttpResponseMessage().WithContent("Typed Result").WithPrivateCacheHeader());
                var factory = container.Resolve<ITypedBuilderFactory>();

                var builder = factory.Create();
                string actual = null;
                var result = await builder.WithUri(server.ListeningUri)
                    .Advanced.WithCaching(true)
                    .OnCacheMiss(HandlerPriority.Last, context =>
                    {
                        actual = context.Items["CustomTypedCacheHandler"] as string;
                    })
                    .ResultAsync<string>();

                result.ShouldBe("Typed Result");
                actual.ShouldBe("CustomTypedCacheHandler: It Works!");
            }
        }

        [Fact]
        public async Task TypedBuilderFactory_ResolvesCustomHandlers()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponseOk("Typed Result");
                var factory = container.Resolve<ITypedBuilderFactory>();

                var builder = factory.Create();

                string actual = null;
                var result = await builder.WithUri(server.ListeningUri)
                    .Advanced
                    .Advanced.WithCaching(false)
                    .OnSending(HandlerPriority.Last, context =>
                    {
                        actual = context.Items["CustomTypedHandler"] as string;
                    }).ResultAsync<string>();

                result.ShouldBe("Typed Result");
                actual.ShouldBe("CustomTypedHandler: It Works!");
            }
        }

        [Fact]
        public async Task TypedBuilderFactory_TwoBuildersDonNotShareSameSettings()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponseOk("Response1")
                    .WithNextResponseOk("Response2");

                var factory = container.Resolve<ITypedBuilderFactory>();
                var actual1 = 0;
                var actual2 = 0;
                HttpMethod method1 = null;
                HttpMethod method2 = null;
                var builder1 = factory.Create().WithUri(server.ListeningUri)
                    .AsGet()
                    .Advanced.WithCaching(false)
                    .WithContextItem("Shared", 1)
                    .OnSending(HandlerPriority.Last, context =>
                    {
                        actual1 = (int)context.Items["Shared"];
                        method1 = context.Request.Method;
                    });
                var builder2 = factory.Create().WithUri(server.ListeningUri)
                    .AsPut()
                    .Advanced.WithCaching(false)
                    .WithContextItem("Shared", 2)
                    .OnSending(HandlerPriority.Last, context =>
                    {
                        actual2 = (int)context.Items["Shared"];
                        method2 = context.Request.Method;
                    });


                var response1 = await builder1.ResultAsync<string>();
                var response2 = await builder2.ResultAsync<string>();

                method1.ToString().ShouldNotBe(method2.ToString());
                actual1.ShouldNotBe(actual2);
            }
        }


        #endregion

        #region HalBuilder

        [Fact]
        public async Task HalBuilderFactory_CanResolveAndCall()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponseOk(TestResource.SerializedDefault1);
                var factory = container.Resolve<IHalBuilderFactory>();

                var builder = factory.Create();

                var resource = await builder.WithLink(server.ListeningUri).Advanced.WithCaching(false).ResultAsync<TestResource>();

                resource.ShouldBe(TestResource.Default1());
            }
        }

        [Fact]
        public async Task HalBuilder_CanResolveAndCall()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponseOk(TestResource.SerializedDefault1);

                var builder = container.Resolve<IHalBuilder>();

                var resource = await builder.WithLink(server.ListeningUri).Advanced.WithCaching(false).ResultAsync<TestResource>();

                resource.ShouldBe(TestResource.Default1());
            }
        }

        [Fact]
        public async Task HalBuilderFactory_ResolvesCustomConfigurations()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponseOk(TestResource.SerializedDefault1);
                var factory = container.Resolve<IHalBuilderFactory>();

                var builder = factory.Create();
                string actual = null;
                string actualTyped = null;
                var resource = await builder.WithLink(server.ListeningUri)
                    .Advanced
                    .WithCaching(false)
                    .OnSending(HandlerPriority.Last, context =>
                    {
                        actual = context.Items["CustomHalConfiguration"] as string;
                        actualTyped = context.Items["CustomTypedConfiguration"] as string;
                    })
                    .ResultAsync<TestResource>();

                resource.ShouldBe(TestResource.Default1());
                actual.ShouldBe("CustomHalConfiguration: It Works!");
                actualTyped.ShouldBeNull();
            }
        }

        [Fact]
        public async Task HalBuilderFactory_ResolvesCustomValidators()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponse(HttpStatusCode.Created);
                var factory = container.Resolve<IHalBuilderFactory>();

                var builder = factory.Create();

                await Should.ThrowAsync<HttpRequestException>(builder.WithLink(server.ListeningUri).Advanced.WithCaching(false).ResultAsync<TestResource>());
            }
        }

        [Fact]
        public async Task HalBuilderFactory_ResolvesCustomCacheHandlers()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponse(new MockHttpResponseMessage().WithContent(TestResource.SerializedDefault1).WithPrivateCacheHeader());
                var factory = container.Resolve<IHalBuilderFactory>();

                var builder = factory.Create();
                string actual = null;
                var resource = await builder.WithLink(server.ListeningUri)
                    .Advanced.WithCaching(true)
                    .OnCacheMiss(HandlerPriority.Last, context =>
                    {
                        actual = context.Items["CustomTypedCacheHandler"] as string;
                    })
                    .ResultAsync<TestResource>();

                resource.ShouldBe(TestResource.Default1());
                actual.ShouldBe("CustomTypedCacheHandler: It Works!");
            }
        }

        [Fact]
        public async Task HalBuilderFactory_ResolvesCustomHandlers()
        {
            var container = RegistrationHelpers.CreateContainer();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithNextResponseOk(TestResource.SerializedDefault1);
                var factory = container.Resolve<IHalBuilderFactory>();

                var builder = factory.Create();

                string actual = null;
                var resource = await builder.WithLink(server.ListeningUri)
                    .Advanced
                    .Advanced.WithCaching(false)
                    .OnSending(HandlerPriority.Last, context =>
                    {
                        actual = context.Items["CustomTypedHandler"] as string;
                    }).ResultAsync<TestResource>();

                resource.ShouldBe(TestResource.Default1());
                actual.ShouldBe("CustomTypedHandler: It Works!");
            }
        }

        #endregion

        [Fact]
        public void CustomCacheProviderActivated()
        {
            var activated = false;

            CustomCacheProvider.Activated += (sender, args) =>
            {
                activated = true;
            };

            var container = RegistrationHelpers.CreateContainer(false);

            activated.ShouldBeTrue();
        }
    }

    #region Custom Http Classes

    public class CustomHttpConfiguration: IBuilderConfiguration<IHttpBuilder>
    {
        public void Configure(IHttpBuilder builder)
        {
            builder.Advanced.WithContextItem("CustomHttpConfiguration", "CustomHttpConfiguration: It Works!").WithCaching(false);
        }
    }

    public class CustomHttpResponseValidator : IHttpResponseValidator
    {
        public bool IsValid(HttpResponseMessage response)
        {
            return response.StatusCode != HttpStatusCode.Created;
        }
    }

    public class CustomHttpCacheHandler : HttpCacheHandler
    {
        public override HandlerPriority GetPriority(CacheHandlerType type)
        {
            return HandlerPriority.First;
        }

        public override Task OnMiss(CacheMissContext context)
        {
            context.Items["CustomHttpCacheHandler"] = "CustomHttpCacheHandler: It Works!";

            return base.OnMiss(context);
        }
    }

    public class CustomHttpHandler : HttpHandler
    {
        public override HandlerPriority GetPriority(HandlerType type)
        {
            return HandlerPriority.First;
        }

        public override Task OnSending(HttpSendingContext context)
        {
            context.Items["CustomHttpHandler"] = "CustomHttpHandler: It Works!";

            return base.OnSending(context);
        }
    }

    #endregion

    #region Custom Typed Classes

    public class CustomTypedConfiguration : IBuilderConfiguration<ITypedBuilder>
    {
        public void Configure(ITypedBuilder builder)
        {
            builder.Advanced.WithContextItem("CustomTypedConfiguration", "CustomTypedConfiguration: It Works!").WithCaching(false);
        }
    }

    public class CustomTypedResponseValidator : ITypedResponseValidator
    {
        public bool IsValid(HttpResponseMessage response)
        {
            return response.StatusCode != HttpStatusCode.Created;
        }
    }

    public class CustomTypedCacheHandler : TypedCacheHandler
    {
        public override HandlerPriority GetPriority(CacheHandlerType type)
        {
            return HandlerPriority.First;
        }

        public override Task OnMiss(CacheMissContext context)
        {
            context.Items["CustomTypedCacheHandler"] = "CustomTypedCacheHandler: It Works!";

            return base.OnMiss(context);
        }
    }

    public class CustomTypedHandler : TypedHandler
    {
        public override HandlerPriority GetPriority(HandlerType type)
        {
            return HandlerPriority.First;
        }

        public override Task OnSending(TypedSendingContext context)
        {
            context.Items["CustomTypedHandler"] = "CustomTypedHandler: It Works!";

            return base.OnSending(context);
        }
    }

    #endregion

    #region Custom Typed Classes

    public class CustomHalConfiguration : IBuilderConfiguration<IHalBuilder>
    {
        public void Configure(IHalBuilder builder)
        {
            builder.Advanced.WithContextItem("CustomHalConfiguration", "CustomHalConfiguration: It Works!").WithCaching(false);
        }
    }

    #endregion

    public class CustomCacheProvider : InMemoryCacheProvider
    {
        public static EventHandler Activated;

        public CustomCacheProvider(IVaryByProvider varyBy)
            : base(varyBy)
        {
            var handler = Activated;

            if (handler != null)
                Activated(this, EventArgs.Empty);
        }
    }
}