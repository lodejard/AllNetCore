using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.AspNet.SignalR.Tests.Core
{
    public class ConnectionExtensionsFacts
    {
        [Fact]
        async public void SendThrowsNullExceptionWhenConnectionIdIsNull()
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

            await Assert.ThrowsAsync<ArgumentException>(() => connection.Send((string)null, new object()));
        }

        [Fact]
        async public void SendThrowsNullExceptionWhenConnectionIdsAreNull()
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

            await Assert.ThrowsAsync<ArgumentNullException>(() => connection.Send((IList<string>)null, new object()));
        }
    }
}
