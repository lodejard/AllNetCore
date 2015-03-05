using System;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.DependencyInjection;

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
            var collection = HostingServices.Create()
                .AddOptions()
                .AddDataProtection()
                .AddSignalR();

            configure(collection.ServiceCollection);

            return collection.ServiceCollection.BuildServiceProvider();
        }
    }
}