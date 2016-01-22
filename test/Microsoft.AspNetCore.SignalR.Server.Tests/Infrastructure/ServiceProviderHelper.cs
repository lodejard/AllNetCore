using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.SignalR.Tests
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
            var builder = new WebHostBuilder()
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