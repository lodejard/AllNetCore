using Microsoft.AspNet.Builder;
using Microsoft.AspNet.SignalR;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;

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

            app.UseStaticFiles();
            app.UseSignalR();
        }
    }

    public class Chat : Hub
    {
        public void Send(string message)
        {
            Clients.All.send(message);
        }
    }
}