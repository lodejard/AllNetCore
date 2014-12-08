using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.AspNet.SignalR.Messaging;
//using Microsoft.AspNet.SignalR.Tracing;
using Microsoft.Framework.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.AspNet.SignalR.Tests.Core
{
    public class ConnectionExtensionsFacts
    {
        [Fact]
        public void SendThrowsNullExceptionWhenConnectionIdIsNull()
        {
            var serializer = JsonUtility.CreateDefaultSerializer();
            var counters = new PerformanceCounterManager(new Mock<ILoggerFactory>().Object);

            var connection = new Connection(new Mock<IMessageBus>().Object,
                                serializer,
                                "signal",
                                "connectonid",
                                new[] { "test" },
                                new string[] { },
                                new Mock<ILoggerFactory>().Object,
                                new AckHandler(completeAcksOnTimeout: false,
                                               ackThreshold: TimeSpan.Zero,
                                               ackInterval: TimeSpan.Zero),
                                counters,
                                new Mock<IProtectedData>().Object,
                                new MemoryPool());

            Assert.ThrowsAsync<ArgumentException>(() => connection.Send((string)null, new object())).Wait();
        }

        [Fact]
        public void SendThrowsNullExceptionWhenConnectionIdsAreNull()
        {
            var serializer = JsonUtility.CreateDefaultSerializer();
            var counters = new PerformanceCounterManager(new Mock<ILoggerFactory>().Object);

            var connection = new Connection(new Mock<IMessageBus>().Object,
                                serializer,
                                "signal",
                                "connectonid",
                                new[] { "test" },
                                new string[] { },
                                new Mock<ILoggerFactory>().Object,
                                new AckHandler(completeAcksOnTimeout: false,
                                               ackThreshold: TimeSpan.Zero,
                                               ackInterval: TimeSpan.Zero),
                                counters,
                                new Mock<IProtectedData>().Object,
                                new MemoryPool());

            Assert.ThrowsAsync<ArgumentNullException>(() => connection.Send((IList<string>)null, new object())).Wait();
        }
    }
}
