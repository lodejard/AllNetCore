using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Server.Infrastructure
{
    // This is a marker interface that we use to tell if SignalR is configured
    internal interface IInitialized
    {
    }

    internal class Initialized : IInitialized
    { 
    }
}
