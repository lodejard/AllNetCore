using System.Linq;
using Microsoft.AspNet.Abstractions;
using Microsoft.AspNet.DependencyInjection;
using Microsoft.AspNet.DependencyInjection.Fallback;
using Microsoft.AspNet.Logging;
using Microsoft.AspNet.Security.DataProtection;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace SignalRSample.Web
{
    public class Startup
    {
        public void Configuration(IBuilder builder)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.Add(SignalRServices.GetServices());

            // Workaround for ITypeActivator coming from the host and using the wrong IServiceProvider
            serviceCollection.AddTransient<ITypeActivator, TypeActivator>();
            serviceCollection.AddSingleton<ILoggerFactory, DiagnosticsLoggerFactory>();

            // The host will add one of these soon
            serviceCollection.AddInstance<IDataProtectionProvider>(DataProtectionProvider.CreateFromDpapi());

            serviceCollection.FallbackServices = builder.ServiceProvider;
            var sp = serviceCollection.BuildServiceProvider();
            builder.ServiceProvider = sp;

            builder.RunSignalR();
        }
    }
}