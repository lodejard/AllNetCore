using Microsoft.AspNet.Builder;
using Microsoft.AspNet.SignalR;
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

                    // options.Hubs.RequireAuthentication();
                });
            });

            app.UseFileServer();

            app.UseSignalR<RawConnection>("/raw-connection");
            app.UseSignalR();
        }
    }
}