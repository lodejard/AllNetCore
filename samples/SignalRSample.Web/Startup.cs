using Microsoft.AspNet.Abstractions;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet;

namespace SignalRSample.Web
{
    public class Startup
    {
        public void Configuration(IBuilder app)
        {
            app.UseServices(services =>
            {
                services.AddSignalR();
            });

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