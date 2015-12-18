using System;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Hosting.Server;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
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
            var builder = new WebApplicationBuilder()
                .UseServer(new Server())
                .Configure(_ => { })
                .ConfigureServices(services =>
                {
                    services.AddSignalR();
                    configure(services);
                });
            return builder.Build().Services;
        }

        private class Server : IServer
        {
            IFeatureCollection IServer.Features { get; }
            
            public void Start<TContext>(IHttpApplication<TContext> application)
            {

            }

            public void Dispose()
            {

            }
        }
    }
}