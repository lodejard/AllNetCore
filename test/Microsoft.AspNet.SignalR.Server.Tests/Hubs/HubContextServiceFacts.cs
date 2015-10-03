using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Microsoft.AspNet.SignalR.Tests.Hubs
{
    public class HubContextServiceFacts
    {
        [Fact]
        public void HubContextServiceCallsConnectionManagerCorrectly()
        {
            var mockConnectionManager = new Mock<IConnectionManager>(MockBehavior.Strict);
            var testContext = Mock.Of<IHubContext<ContextHub>>();
            mockConnectionManager.Setup(m => m.GetHubContext<ContextHub>()).Returns(testContext);

            var sp = ServiceProviderHelper.CreateServiceProvider(sc =>
            {
                sc.AddInstance<IConnectionManager>(mockConnectionManager.Object);
            });

            var returnedContext = sp.GetRequiredService<IHubContext<ContextHub>>();

            Assert.Same(testContext.Clients, returnedContext.Clients);
            Assert.Same(testContext.Groups, returnedContext.Groups);
            mockConnectionManager.VerifyAll();
        }

        [Fact]
        public void TypedHubContextServiceCallsConnectionManagerCorrectly()
        {
            var mockConnectionManager = new Mock<IConnectionManager>(MockBehavior.Strict);
            var testContext = Mock.Of<IHubContext<TypedContextHub, IClient>>();
            mockConnectionManager.Setup(m => m.GetHubContext<TypedContextHub, IClient>()).Returns(testContext);

            var sp = ServiceProviderHelper.CreateServiceProvider(sc =>
            {
                sc.AddInstance<IConnectionManager>(mockConnectionManager.Object);
            });

            var returnedContext = sp.GetRequiredService<IHubContext<TypedContextHub, IClient>>();

            Assert.Same(testContext.Clients, returnedContext.Clients);
            Assert.Same(testContext.Clients, returnedContext.Clients);
            mockConnectionManager.VerifyAll();
        }

        public interface IClient
        {
        }

        public class ContextHub : Hub
        {
        }

        public class TypedContextHub : Hub<IClient>
        {
        }
    }
}
