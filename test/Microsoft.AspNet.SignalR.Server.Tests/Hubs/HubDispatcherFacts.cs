using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.Runtime.Infrastructure;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.AspNet.SignalR.Tests.Hubs
{
    public class HubDispatcherFacts
    {
        public static IEnumerable<string[]> JSProxyUrls
        {
            get
            {
                return new List<string[]>()
                {
                    new []{"/signalr/hubs"},
                    new []{"/signalr/js"}
                };
            }
        }

        [Theory]
        [MemberData("JSProxyUrls")]
        public void RequestingSignalrHubsUrlReturnsProxy(string proxyUrl)
        {
            // Arrange
            var serviceProvider = CreateServiceProvider();
            var dispatcher = new HubDispatcher(serviceProvider.GetService<IOptions<SignalROptions>>());
            var testContext = new TestContext(proxyUrl);

            // Act
            dispatcher.Initialize(serviceProvider);
            dispatcher.ProcessRequest(testContext.MockHttpContext.Object).Wait();

            // Assert
            Assert.Equal("application/javascript; charset=UTF-8", testContext.ResponseContentType);
            Assert.Equal(1, testContext.ResponseBuffer.Count);
            Assert.NotNull(testContext.ResponseBuffer[0]);
            Assert.False(testContext.ResponseBuffer[0].StartsWith("throw new Error("));
        }

        [Theory]
        [MemberData("JSProxyUrls")]
        public void RequestingSignalrHubsUrlWithTrailingSlashReturnsProxy(string proxyUrl)
        {
            // Arrange
            var serviceProvider = CreateServiceProvider();
            var dispatcher = new HubDispatcher(serviceProvider.GetService<IOptions<SignalROptions>>());
            var testContext = new TestContext(proxyUrl + "/");

            // Act
            dispatcher.Initialize(serviceProvider);
            dispatcher.ProcessRequest(testContext.MockHttpContext.Object).Wait();

            // Assert
            Assert.Equal("application/javascript; charset=UTF-8", testContext.ResponseContentType);
            Assert.Equal(1, testContext.ResponseBuffer.Count);
            Assert.NotNull(testContext.ResponseBuffer[0]);
            Assert.False(testContext.ResponseBuffer[0].StartsWith("throw new Error("));
        }

        [Theory]
        [MemberData("JSProxyUrls")]
        public void RequestingSignalrHubsUrlWithJavaScriptProxiesDesabledDoesNotReturnProxy(string proxyUrl)
        {
            // Arrange
            var serviceProvider = CreateServiceProvider();
            var optionsAccessor = serviceProvider.GetService<IOptions<SignalROptions>>();
            optionsAccessor.Options.Hubs.EnableJavaScriptProxies = false;
            var dispatcher = new HubDispatcher(optionsAccessor);
            var testContext = new TestContext(proxyUrl);

            // Act
            dispatcher.Initialize(serviceProvider);
            dispatcher.ProcessRequest(testContext.MockHttpContext.Object).Wait();

            // Assert
            Assert.Equal("application/javascript; charset=UTF-8", testContext.ResponseContentType);
            Assert.Equal(1, testContext.ResponseBuffer.Count);
            Assert.True(testContext.ResponseBuffer[0].StartsWith("throw new Error("));
        }

        [Fact]
        public void DetailedErrorsAreDisabledByDefault()
        {
            // Arrange
            var serviceProvider = CreateServiceProvider();
            var dispatcher = new HubDispatcher(serviceProvider.GetService<IOptions<SignalROptions>>());
            var testContext = new TestContext("/signalr/send", new Dictionary<string, string>
            {
                {"transport", "longPolling"},
                {"connectionToken", "0"},
                {"data", "{\"H\":\"ErrorHub\",\"M\":\"Error\",\"A\":[],\"I\":0}"}
            });

            // Act
            dispatcher.Initialize(serviceProvider);
            dispatcher.ProcessRequest(testContext.MockHttpContext.Object).Wait();

            var json = JsonSerializer.Create(new JsonSerializerSettings());

            // Assert
            Assert.Equal("application/json; charset=UTF-8", testContext.ResponseContentType);
            Assert.True(testContext.ResponseBuffer.Count > 0);

            using (var reader = new StringReader(String.Join(String.Empty, testContext.ResponseBuffer)))
            {
                var hubResponse = (HubResponse)json.Deserialize(reader, typeof(HubResponse));
                Assert.Contains("ErrorHub.Error", hubResponse.Error);
                Assert.DoesNotContain("Custom", hubResponse.Error);
            }
        }

        [Fact]
        public void DetailedErrorsFromFaultedTasksAreDisabledByDefault()
        {
            // Arrange
            var serviceProvider = CreateServiceProvider();
            var dispatcher = new HubDispatcher(serviceProvider.GetService<IOptions<SignalROptions>>());
            var testContext = new TestContext("/signalr/send", new Dictionary<string, string>
            {
                { "transport", "longPolling"},
                {"connectionToken", "0"},
                {"data", "{\"H\":\"ErrorHub\",\"M\":\"ErrorTask\",\"A\":[],\"I\":0}"}
            });

            // Act
            dispatcher.Initialize(serviceProvider);
            dispatcher.ProcessRequest(testContext.MockHttpContext.Object).Wait();

            // Assert
            Assert.Equal("application/json; charset=UTF-8", testContext.ResponseContentType);
            Assert.True(testContext.ResponseBuffer.Count > 0);

            using (var reader = new StringReader(String.Join(String.Empty, testContext.ResponseBuffer)))
            {
                var json = JsonSerializer.Create(new JsonSerializerSettings());
                var hubResponse = (HubResponse)json.Deserialize(reader, typeof(HubResponse));
                Assert.Contains("ErrorHub.ErrorTask", hubResponse.Error);
                Assert.DoesNotContain("Custom", hubResponse.Error);
            }
        }

        [Fact]
        public void DetailedErrorsCanBeEnabled()
        {
            // Arrange
            var serviceProvider = CreateServiceProvider();
            var optionsAccessor = serviceProvider.GetService<IOptions<SignalROptions>>();
            optionsAccessor.Options.Hubs.EnableDetailedErrors = true;
            var dispatcher = new HubDispatcher(optionsAccessor);
            var testContext = new TestContext("/signalr/send", new Dictionary<string, string>
            {
                { "transport", "longPolling"},
                { "connectionToken", "0"},
                { "data", "{\"H\":\"ErrorHub\",\"M\":\"Error\",\"A\":[],\"I\":0}"}
            });

            // Act
            dispatcher.Initialize(serviceProvider);
            dispatcher.ProcessRequest(testContext.MockHttpContext.Object).Wait();

            // Assert
            Assert.Equal("application/json; charset=UTF-8", testContext.ResponseContentType);
            Assert.True(testContext.ResponseBuffer.Count > 0);

            using (var reader = new StringReader(String.Join(String.Empty, testContext.ResponseBuffer)))
            {
                var json = JsonSerializer.Create(new JsonSerializerSettings());
                var hubResponse = (HubResponse)json.Deserialize(reader, typeof(HubResponse));
                Assert.Equal("Custom Error.", hubResponse.Error);
            }
        }

        [Fact]
        public void DetailedErrorsFromFaultedTasksCanBeEnabled()
        {
            // Arrange
            var serviceProvider = CreateServiceProvider();
            var optionsAccessor = serviceProvider.GetService<IOptions<SignalROptions>>();
            optionsAccessor.Options.Hubs.EnableDetailedErrors = true;
            var dispatcher = new HubDispatcher(optionsAccessor);
            var testContext = new TestContext("/signalr/send", new Dictionary<string, string>
            {
                {"transport", "longPolling"},
                {"connectionToken", "0"},
                {"data", "{\"H\":\"ErrorHub\",\"M\":\"ErrorTask\",\"A\":[],\"I\":0}"}
            });

            // Act
            dispatcher.Initialize(serviceProvider);
            dispatcher.ProcessRequest(testContext.MockHttpContext.Object).Wait();

            // Assert
            Assert.Equal("application/json; charset=UTF-8", testContext.ResponseContentType);
            Assert.True(testContext.ResponseBuffer.Count > 0);

            using (var reader = new StringReader(String.Join(String.Empty, testContext.ResponseBuffer)))
            {
                var json = JsonSerializer.Create(new JsonSerializerSettings());
                var hubResponse = (HubResponse)json.Deserialize(reader, typeof(HubResponse));
                Assert.Equal("Custom Error from task.", hubResponse.Error);
            }
        }

        [Fact]
        public void DuplicateHubNamesThrows()
        {
            // Arrange
            var mockHub = new Mock<IHub>();
            var mockHubManager = new Mock<IHubManager>();
            mockHubManager.Setup(m => m.GetHub("foo")).Returns(new HubDescriptor { Name = "foo", HubType = mockHub.Object.GetType() });

            var serviceProvider = new ServiceCollection()
                .Add(OptionsServices.GetDefaultServices())
                .Add(HostingServices.GetDefaultServices())
                .Add(SignalRServices.GetDefaultServices().Where(descriptor => descriptor.ServiceType != typeof(IHubManager)))
                .AddInstance<IHubManager>(mockHubManager.Object)
                .BuildServiceProvider();

            var dispatcher = new HubDispatcher(serviceProvider.GetService<IOptions<SignalROptions>>());
            var testContext = new TestContext("/ignorePath", new Dictionary<string, string>
            {
                {"connectionData", @"[{name: ""foo""}, {name: ""Foo""}]"},
            });

            // Act & Assert
            dispatcher.Initialize(serviceProvider);
            Assert.Throws<InvalidOperationException>(() => dispatcher.Authorize(testContext.MockRequest.Object));
        }

        private static IServiceProvider CreateServiceProvider()
        {
            return ServiceProviderHelper.CreateServiceProvider(services =>
            {
                services.AddTransient<IProtectedData, EmptyProtectedData>();
                services.AddTransient<ErrorHub>();
            });
        }

        private class ErrorHub : Hub
        {
            public void Error()
            {
                throw new Exception("Custom Error.");
            }

            public async Task ErrorTask()
            {
                await TaskAsyncHelper.Delay(TimeSpan.FromMilliseconds(1));
                throw new Exception("Custom Error from task.");
            }
        }

        private class EmptyProtectedData : IProtectedData
        {
            public string Protect(string data, string purpose)
            {
                return data;
            }

            public string Unprotect(string protectedValue, string purpose)
            {
                return protectedValue;
            }
        }
    }
}
