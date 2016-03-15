using System;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;

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
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseServer(new Server())
                .Configure(_ => { })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IApplicationEnvironment, TestApplicationEnvironment>();
                    services.AddSignalR();
                    configure(services);
                });
            return builder.Build().Services;
        }

        private class TestApplicationEnvironment : IApplicationEnvironment
        {
            public string ApplicationBasePath => typeof(ServiceProviderHelper).GetTypeInfo().Assembly.Location;

            public string ApplicationName => typeof(ServiceProviderHelper).GetTypeInfo().Assembly.GetName().Name;

            public string ApplicationVersion
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public FrameworkName RuntimeFramework
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
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