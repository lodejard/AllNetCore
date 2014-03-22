using Microsoft.AspNet.Abstractions;
using Microsoft.AspNet.SignalR;

namespace SignalRSample.Web
{
    public class Startup
    {
        public void Configuration(IBuilder app)
        {
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