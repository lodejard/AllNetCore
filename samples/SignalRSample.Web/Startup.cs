using Microsoft.AspNet.Abstractions;
using Microsoft.AspNet.DependencyInjection;
using Microsoft.AspNet.DependencyInjection.Fallback;
using Microsoft.AspNet.Logging;
using Microsoft.AspNet.SignalR;

namespace SignalRSample.Web
{
    public class Startup
    {
        public void Configuration(IBuilder builder)
        {
            var sc = new ServiceCollection()
                         .Add(SignalRServices.GetServices())
                         .AddSingleton<ILoggerFactory, ConsoleLoggerFactory>()
                         .AddTransient<ITypeActivator, TypeActivator>();

            builder.ServiceProvider = sc.BuildServiceProvider(builder.ServiceProvider);

            builder.MapSignalR();
        }
    }

    public class Chat : Hub
    {
        public void Send(string message)
        {
            Clients.All.send(message);
        }
    }

    public class ConsoleLoggerFactory : ILoggerFactory
    {
        public ILogger Create(string name)
        {
            return new ConsoleLogger(name);
        }

        private class ConsoleLogger : ILogger
        {
            private readonly string _name;

            public ConsoleLogger(string name)
            {
                _name = name;
            }

            public bool WriteCore(TraceType eventType, int eventId, object state, System.Exception exception, System.Func<object, System.Exception, string> formatter)
            {
                System.Console.WriteLine("[{0}]: {1}", _name, formatter(state, exception));
                return true;
            }
        }
    }
}