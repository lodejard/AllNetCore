using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.FeatureModel;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Hosting.Server;
using Microsoft.AspNet.Hosting.Startup;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime.Infrastructure;

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
            var collection = new ServiceCollection();

            var host = new WebHostBuilder(collection.BuildServiceProvider())
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
            public IServerInformation Initialize(IConfiguration configuration)
            {
                return null;
            }

            public IDisposable Start(IServerInformation serverInformation, Func<IFeatureCollection, Task> application)
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