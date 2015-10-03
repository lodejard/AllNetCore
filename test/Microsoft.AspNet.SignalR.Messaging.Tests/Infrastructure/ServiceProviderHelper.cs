using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            var collection = new ServiceCollection()
                .AddMessageBus()
                .AddSingleton<ILoggerFactory, LoggerFactory>();

            configure(collection);

            return collection.BuildServiceProvider();
        }
    }
}