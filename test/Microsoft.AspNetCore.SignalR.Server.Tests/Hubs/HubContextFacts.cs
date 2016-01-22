using System;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

using SignalRPerformanceCounterManager = Microsoft.AspNet.SignalR.Infrastructure.PerformanceCounterManager;

namespace Microsoft.AspNet.SignalR.Tests
{
    public class HubContextFacts
    {
        [Fact]
        public void GroupThrowsNullExceptionWhenGroupNameIsNull()
        {
            var serializer = JsonUtility.CreateDefaultSerializer();
            var counters = new SignalRPerformanceCounterManager(new Mock<ILoggerFactory>().Object);
            var connection = new Mock<IConnection>();
            var invoker = new Mock<IHubPipelineInvoker>();
            var hubContext = new HubContext(connection.Object, invoker.Object, "test");
            
            Assert.Throws<ArgumentException>(() => hubContext.Clients.Group(null));
        }

        [Fact]
        public void ClientThrowsNullExceptionWhenConnectionIdIsNull()
        {
            var serializer = JsonUtility.CreateDefaultSerializer();
            var counters = new SignalRPerformanceCounterManager(new Mock<ILoggerFactory>().Object);
            var connection = new Mock<IConnection>();
            var invoker = new Mock<IHubPipelineInvoker>();

            var hubContext = new HubContext(connection.Object, invoker.Object, "test");

            Assert.Throws<ArgumentException>(() => hubContext.Clients.Client(null));
        }
    }
}
