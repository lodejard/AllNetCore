using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.SignalR.Configuration;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
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

            // Act
            transport.ProcessRequest(CreateMockTransportConnection());

            // Assert
            Assert.True(transport.ConnectTask.Wait(TimeSpan.FromSeconds(2)), "ConnectTask task not tripped");
            Assert.False(connected, "The Connected event should not be raised");
            Assert.False(reconnected, "The Reconnected event should not be raised");
        }

        [Fact]
        public void SetTheCorrectMIMETypeForJSONSends()
        {
            // Arrange
            var transport = TestLongPollingTransport.Create("/send");

            // Act
            transport.Send(new object());

            // Assert
            Assert.True(transport.TestContentType.Wait(TimeSpan.FromSeconds(2)), "ContentType not set");
            Assert.Equal(JsonUtility.JsonMimeType, transport.TestContentType.Result);
        }

        [Fact]
        public void SetTheCorrectMIMETypeForJSONPSends()
        {
            // Arrange
            // Make the transport think it is responding to a JSONP request
            var queryString = new Dictionary<string, string> { { "callback", "foo" } };
            var transport = TestLongPollingTransport.Create("/send", queryString);

            // Act
            // JSONP send
            transport.Send(new object());

            // Assert
            Assert.True(transport.TestContentType.Wait(TimeSpan.FromSeconds(2)), "ContentType not set");
            Assert.Equal(JsonUtility.JavaScriptMimeType, transport.TestContentType.Result);
        }

        [Fact]
        public void SetTheCorrectMIMETypeForJSONPolls()
        {
            // Arrange
            var transport = TestLongPollingTransport.Create("/poll");

            // Act
            transport.ProcessRequest(CreateMockTransportConnection());

            // Assert
            Assert.True(transport.TestContentType.Wait(TimeSpan.FromSeconds(2)), "ContentType not set");
            Assert.Equal(JsonUtility.JsonMimeType, transport.TestContentType.Result);
        }

        [Fact]
        public void SetTheCorrectMIMETypeForJSONPPolls()
        {
            // Arrange
            // Make the transport think it is responding to a JSONP request
            var queryString = new Dictionary<string, string> { { "callback", "foo" } };
            var transport = TestLongPollingTransport.Create("/poll", queryString);

            // Act
            transport.ProcessRequest(CreateMockTransportConnection());

            // Assert
            Assert.True(transport.TestContentType.Wait(TimeSpan.FromSeconds(2)), "ContentType not set");
            Assert.Equal(JsonUtility.JavaScriptMimeType, transport.TestContentType.Result);
        }

        private static ITransportConnection CreateMockTransportConnection()
        {
            var transportConnection = new Mock<ITransportConnection>();
            transportConnection.Setup(m => m.Receive(It.IsAny<string>(),
                                                     It.IsAny<Func<PersistentResponse, object, Task<bool>>>(),
                                                     It.IsAny<int>(),
                                                     It.IsAny<object>())).Returns(DisposableAction.Empty);
            return transportConnection.Object;
        }

        private class TestLongPollingTransport : LongPollingTransport
        {
            private TaskCompletionSource<string> _contentTypeTcs = new TaskCompletionSource<string>();

            public TestLongPollingTransport(HttpContext context,
                                            JsonSerializer jsonSerializer,
                                            ITransportHeartbeat heartbeat,
                                            IPerformanceCounterManager performanceCounterManager,
                                            IApplicationLifetime applicationLifetime,
                                            ILoggerFactory loggerFactory,
                                            IOptions<SignalROptions> optionsAccessor,
                                            IMemoryPool pool)
                : base(context, jsonSerializer, heartbeat, performanceCounterManager, applicationLifetime, loggerFactory, optionsAccessor, pool)
            {

            }

            public static TestLongPollingTransport Create(
                string requestPath,
                Dictionary<string, string> queryString = null)
            {
                TestLongPollingTransport transport = null;
                var context = new TestContext(requestPath, queryString);

                context.MockResponse.SetupSet(m => m.ContentType = It.IsAny<string>()).Callback<string>(contentType =>
                {
                    transport._contentTypeTcs.SetResult(contentType);
                });

                var json = JsonUtility.CreateDefaultSerializer();
                var heartBeat = new Mock<ITransportHeartbeat>();
                var counters = new Mock<IPerformanceCounterManager>();
                var appLifetime = new Mock<IApplicationLifetime>();
                var loggerFactory = new Mock<ILoggerFactory>();
                var optionsAccessor = new Mock<IOptions<SignalROptions>>();
                optionsAccessor.Setup(m => m.Value).Returns(new SignalROptions());
                var pool = new Mock<IMemoryPool>();

                transport = new TestLongPollingTransport(
                    context.MockHttpContext.Object,
                    json,
                    heartBeat.Object,
                    counters.Object,
                    appLifetime.Object,
                    loggerFactory.Object,
                    optionsAccessor.Object,
                    pool.Object);

                return transport;
            }

            public Task<string> TestContentType
            {
                get { return _contentTypeTcs.Task; }
            }

            public bool TestSuppressReconnect
            {
                get { return SuppressReconnect; }
            }
        }
    }
}
