using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SignalRSample.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseIISPlatformHandler();
            app.UseFileServer();

            app.UseSignalR<RawConnection>("/raw-connection");
            app.UseSignalR();
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseDefaultConfiguration(args)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
