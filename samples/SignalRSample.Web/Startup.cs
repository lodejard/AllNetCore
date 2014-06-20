using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.SignalR;
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
            app.UseSignalR();
        }
    }

    public class Chat : Hub
    {
        public override Task OnDisconnected(bool stopCalled)
        {
            Clients.All.send(Context.ConnectionId + " disconnected" + (stopCalled ? "" : " after timeout"));
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnConnected()
        {
            Clients.All.send(Context.ConnectionId + " connected");
            return base.OnConnected();
        }

        public void Send(string message)
        {
            Clients.All.send(message);
        }
    }
}