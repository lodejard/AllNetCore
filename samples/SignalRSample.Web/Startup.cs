using Microsoft.AspNet.Abstractions;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.RequestContainer;

namespace SignalRSample.Web
{
    public class Startup
    {
        public void Configuration(IBuilder app)
        {
            app.UseContainer(services =>
            {
                services.AddSignalR();
            });

            app.MapSignalR();
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