using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace Microsoft.AspNet.SignalR.Tests
{
    public class MethodExtensionsFacts
    {
        [Fact]
        public void MatchSuccessful()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var ta = new TypeActivator();
            var hubManager = ta.CreateInstance<DefaultHubManager>(sp);

            // Should be AddNumbers
            MethodDescriptor methodDescriptor = hubManager.GetHubMethod("CoreTestHubWithMethod", "AddNumbers", new IJsonValue[] { null, null });

            // We should find our method descriptor
            Assert.NotNull(methodDescriptor);

            // Value does not matter, hence the null;
            Assert.True(methodDescriptor.Matches(new IJsonValue[] { null, null }));
        }
    }
}
