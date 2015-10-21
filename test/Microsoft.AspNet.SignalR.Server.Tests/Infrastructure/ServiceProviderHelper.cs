using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Hosting.Server;
using Microsoft.AspNet.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNet.SignalR.Tests
{
    /// <summary>
    /// Summary description for ServiceProviderHelper
    /// </summary>
    public class ServiceProviderHelper
    {
        public static IServiceProvider CreateServiceProvider()
        {
            return CreateServiceProvider(_ => { });
        }

        public static IServiceProvider CreateServiceProvider(Action<IServiceCollection> configure)
        {
            var host = new WebHostBuilder()
                .UseServer(new ServerFactory())
                .UseStartup(
                    _ => { },
                    services =>
                    {
                        services.AddSignalR();
                        configure(services);
                        return services.BuildServiceProvider();
                    });
            return host.Build().ApplicationServices;
        }

        private class ServerFactory : IServerFactory
        {
            public IFeatureCollection Initialize(IConfiguration configuration)
            {
                return null;
            }

            public IDisposable Start(IFeatureCollection serverFeatures, Func<IFeatureCollection, Task> application)
            {
                return new StartInstance(application);
            }

            private class StartInstance : IDisposable
            {
                private readonly Func<IFeatureCollection, Task> _application;

                public StartInstance(Func<IFeatureCollection, Task> application)
                {
                    _application = application;
                }

                public void Dispose()
                {
                }
            }
        }
    }
}