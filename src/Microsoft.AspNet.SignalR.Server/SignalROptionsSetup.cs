using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.SignalR
{
    public class SignalROptionsSetup : ConfigureOptions<SignalROptions>
    {
        public SignalROptionsSetup() : base(ConfigureSignalR)
        {
            /// The default order for sorting is -1000. Other framework code
            /// the depends on order should be ordered between -1 to -1999.
            /// User code should order at bigger than 0 or smaller than -2000.
            Order = -1000;
        }

        private static void ConfigureSignalR(SignalROptions options)
        {
            // Add the authorization module by default
            options.Hubs.PipelineModules.Add(new AuthorizeModule());
        }
    }
}