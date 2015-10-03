using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Microsoft.AspNet.SignalR.Tests
{
    public class PersistentConnectionContextServiceFacts
    {
        [Fact]
        public void HubContextServiceCallsConnectionManagerCorrectly()
        {
            var mockConnectionManager = new Mock<IConnectionManager>(MockBehavior.Strict);
            var testContext = Mock.Of<IPersistentConnectionContext<ContextConnection>>();
            mockConnectionManager.Setup(m => m.GetConnectionContext<ContextConnection>()).Returns(testContext);

            var sp = ServiceProviderHelper.CreateServiceProvider(sc =>
            {
                sc.AddInstance<IConnectionManager>(mockConnectionManager.Object);
            });

            var returnedContext = sp.GetRequiredService<IPersistentConnectionContext<ContextConnection>>();

            Assert.Same(testContext.Connection, returnedContext.Connection);
            Assert.Same(testContext.Groups, returnedContext.Groups);
            mockConnectionManager.VerifyAll();
        }

        public class ContextConnection : PersistentConnection
        {
        }
    }
}
