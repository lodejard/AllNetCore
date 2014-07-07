using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.SignalR.Configuration;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.AspNet.SignalR.Tests.Transports
{
    public class LongPollingTransportFacts
    {
        [Fact]
        public void SupressReconnectsForRequestsNotEndingInReconnect()
        {
            // Arrange transports while specifying request paths
            var reconnectTransport = TestLongPollingTransport.Create("/reconnect");
            var pollTransport = TestLongPollingTransport.Create("/poll");
            var emptyPathTransport = TestLongPollingTransport.Create("/");

            // Assert
            Assert.False(reconnectTransport.TestSuppressReconnect);
            Assert.True(pollTransport.TestSuppressReconnect);
            Assert.True(emptyPathTransport.TestSuppressReconnect);
        }

        [Fact]
        public void EmptyPathDoesntTriggerReconnects()
        {
            // Arrange
            var transport = TestLongPollingTransport.Create(requestPath: "/");

            var connected = false;
            var reconnected = false;

            transport.Connected = () =>
            {
                connected = true;
                return TaskAsyncHelper.Empty;
            };

            transport.Reconnected = () =>
            {
                reconnected = true;
                return TaskAsyncHelper.Empty;
            };

            var transportConnection = new Mock<ITransportConnection>();
            transportConnection.Setup(m => m.Receive(It.IsAny<string>(),
                                                     It.IsAny<Func<PersistentResponse, object, Task<bool>>>(),
                                                     It.IsAny<int>(),
                                                     It.IsAny<object>())).Returns(DisposableAction.Empty);

            // Act
            transport.ProcessRequest(transportConnection.Object);

            // Assert
            Assert.True(transport.ConnectTask.Wait(TimeSpan.FromSeconds(2)), "ConnectTask task not tripped");
            Assert.False(connected, "The Connected event should not be raised");
            Assert.False(reconnected, "The Reconnected event should not be raised");
        }

        private class TestLongPollingTransport : LongPollingTransport
        {
            public TestLongPollingTransport(HttpContext context,
                                            JsonSerializer jsonSerializer,
                                            ITransportHeartbeat heartbeat,
                                            IPerformanceCounterManager performanceCounterManager,
                                            IApplicationLifetime applicationLifetime,
                                            ILoggerFactory loggerFactory,
                                            IOptionsAccessor<SignalROptions> optionsAccessor,
                                            IMemoryPool pool) : 
                base(context, jsonSerializer, heartbeat, performanceCounterManager, applicationLifetime, loggerFactory, optionsAccessor, pool)
            {

            }

            public static TestLongPollingTransport Create(string requestPath)
            {
                var context = new TestContext(requestPath).MockHttpContext.Object;
                var json = JsonUtility.CreateDefaultSerializer();
                var heartBeat = new Mock<ITransportHeartbeat>();
                var counters = new Mock<IPerformanceCounterManager>();
                var appLifetime = new Mock<IApplicationLifetime>();
                var loggerFactory = new Mock<ILoggerFactory>();
                var optionsAccessor = new Mock<IOptionsAccessor<SignalROptions>>();
                optionsAccessor.Setup(m => m.Options).Returns(new SignalROptions());
                var pool = new Mock<IMemoryPool>();

                return new TestLongPollingTransport(
                    context,
                    json,
                    heartBeat.Object,
                    counters.Object,
                    appLifetime.Object,
                    loggerFactory.Object,
                    optionsAccessor.Object,
                    pool.Object);
            }

            public bool TestSuppressReconnect
            {
                get { return SuppressReconnect; }
            }
        }
    }
}
