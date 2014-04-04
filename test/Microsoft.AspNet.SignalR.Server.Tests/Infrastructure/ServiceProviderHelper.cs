using System;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
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
            var collection = new ServiceCollection()
                .Add(OptionsServices.GetDefaultServices())
                .Add(HostingServices.GetDefaultServices())
                .Add(SignalRServices.GetDefaultServices())
                .AddInstance<ILoggerFactory>(new TestLoggerFactory());

            configure(collection);

            return collection.BuildServiceProvider(CallContextServiceLocator.Locator.ServiceProvider);
        }

        private class TestLoggerFactory : ILoggerFactory
        {
            public ILogger Create(string name)
            {
                return new TestLogger(name);
            }

            private class TestLogger : ILogger
            {
                private readonly string _name;

                public TestLogger(string name)
                {
                    _name = name;
                }

                public bool WriteCore(TraceType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
                {
                    Console.WriteLine("[" + _name + "]: " + formatter(state, exception));
                    return true;
                }
            }
        }
    }
}