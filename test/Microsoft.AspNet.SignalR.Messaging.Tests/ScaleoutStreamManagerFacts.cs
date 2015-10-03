using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

using SignalRPerformanceCounterManager = Microsoft.AspNet.SignalR.Infrastructure.PerformanceCounterManager;

namespace Microsoft.AspNet.SignalR.Tests
{
    public class ScaleoutStreamManagerFacts
    {
        [Fact]
        public void StreamManagerValidatesScaleoutConfig()
        {
            var loggerFactory = new Mock<ILoggerFactory>();
            var perfCounters = new SignalRPerformanceCounterManager(loggerFactory.Object);
            var config = new ScaleoutOptions();

            config.QueueBehavior = QueuingBehavior.Always;
            config.MaxQueueLength = 0;

            Assert.Throws<InvalidOperationException>(() => new ScaleoutStreamManager((int x, IList<Message> list) => { return TaskAsyncHelper.Empty; },
                (int x, ulong y, ScaleoutMessage msg) => { },
                0,
                new Mock<ILogger>().Object,
                perfCounters,
                config));
        }
    }
}
