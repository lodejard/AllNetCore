using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.AspNet.SignalR.Tests
{
    public class MethodExtensionsFacts
    {
        [Fact]
        public void MatchSuccessful()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);

            // Should be AddNumbers
            MethodDescriptor methodDescriptor = hubManager.GetHubMethod("CoreTestHubWithMethod", "AddNumbers", new IJsonValue[] { null, null });

            // We should find our method descriptor
            Assert.NotNull(methodDescriptor);

            // Value does not matter, hence the null;
            Assert.True(methodDescriptor.Matches(new IJsonValue[] { null, null }));
        }
    }
}
