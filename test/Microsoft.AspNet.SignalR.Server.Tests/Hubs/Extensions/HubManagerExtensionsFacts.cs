using System;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.AspNet.SignalR.Tests
{
    public class HubManagerExtensionsFacts
    {
        [Fact]
        public void EnsureHubThrowsWhenCantResolve()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);

            Assert.Throws<InvalidOperationException>(() => hubManager.EnsureHub("__ELLO__"));
        }

        [Fact]
        public void EnsureHubSuccessfullyResolves()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);
            var TestHubName = "CoreTestHubWithMethod";

            HubDescriptor hub = null,
                          actualDescriptor = hubManager.GetHub(TestHubName);

            // (Does not throw)
            hub = hubManager.EnsureHub(TestHubName);

            Assert.Equal(hub, actualDescriptor);
        }
    }
}
