using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;

namespace SignalRSample.Web
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseServices(services =>
            {
                services.AddSignalR(options =>
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