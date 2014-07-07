using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;

namespace SignalRSample.Web
{
    public class Startup
    {
        public void Configure(IBuilder app)
        {
            app.UseServices(services =>
            {
                services
                    .AddSignalR()
                    .SetupOptions(options =>
                    {
                        options.Hubs.EnableDetailedErrors = true;
                    });
            });

            app.UseFileServer();

            app.UseSignalR<RawConnection>("/raw-connection");
            app.UseSignalR();
        }
    }
}