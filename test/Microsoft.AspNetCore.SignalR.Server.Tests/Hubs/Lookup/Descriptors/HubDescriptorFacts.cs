using Microsoft.AspNetCore.SignalR.Hubs;
using Xunit;

namespace Microsoft.AspNetCore.SignalR.Tests
{
    public class HubDescriptorFacts
    {
        [Fact]
        public void CorrectQualifiedName()
        {
            string hubName = "MyHubDescriptor",
                   unqualifiedName = "MyUnqualifiedName";

            HubDescriptor hubDescriptor = new HubDescriptor()
            {
                Name = hubName
            };

            Assert.Equal(hubDescriptor.CreateQualifiedName(unqualifiedName), hubName + "." + unqualifiedName);
        }
    }
}
