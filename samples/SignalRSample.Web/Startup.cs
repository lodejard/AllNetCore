using Microsoft.AspNet.Builder;
using Microsoft.AspNet.SignalR;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Serilog;

namespace SignalRSample.Web
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
#if ASPNET50
            string OutputTemplate = "{SourceContext} {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";

            var serilog = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.RollingFile(@".\SignalR-Log-{Date}.txt", outputTemplate: OutputTemplate);

            loggerFactory.AddSerilog(serilog);
#endif

            app.UseServices(services =>
            {
                services.AddSignalR(options =>
                {
                    options.Hubs.EnableDetailedErrors = true;

                    // options.Hubs.RequireAuthentication();
                });
            });

            app.UseFileServer();

            app.UseSignalR<RawConnection>("/raw-connection");
            app.UseSignalR();
        }
    }
}