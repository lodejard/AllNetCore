using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.SignalR.Transports;
using Moq;
using Moq.Protected;
using Xunit;
using System.Threading;

namespace Microsoft.AspNet.SignalR.Tests
{
    public class PersistentConnectionFacts
    {
        public class ProcessRequest
        {
            [Fact]
            public void UnknownTransportFails()
            {
                var connection = new Mock<PersistentConnection>() { CallBase = true };
                var qs = new Dictionary<string, string>();
                var sp = ServiceProviderHelper.CreateServiceProvider();
                var context = new TestContext("/", qs);
                context.MockResponse.SetupProperty(r => r.StatusCode);
                connection.Object.Initialize(sp);

                var task = connection.Object.ProcessRequest(context.MockHttpContext.Object);

                Assert.True(task.IsCompleted);
                Assert.Equal(400, context.MockResponse.Object.StatusCode);
            }

            [Fact]
            public void MissingConnectionTokenFails()
            {
                var connection = new Mock<PersistentConnection>() { CallBase = true };
                var qs = new Dictionary<string, string>();
                var sp = ServiceProviderHelper.CreateServiceProvider();
                var context = new TestContext("/", qs);
                context.MockResponse.SetupProperty(r => r.StatusCode);
                connection.Object.Initialize(sp);

                var task = connection.Object.ProcessRequest(context.MockHttpContext.Object);

                Assert.True(task.IsCompleted);
                Assert.Equal(400, context.MockResponse.Object.StatusCode);
            }

            [Fact]
            public void UncleanDisconnectFiresOnDisconnected()
            {
                // Arrange
                var context = new TestContext("/", new Dictionary<string, string> { { "connectionToken", "1" } });

                var transport = new Mock<ITransport>();
                transport.SetupProperty(m => m.Disconnected);
                transport.SetupProperty(m => m.ConnectionId);
                transport.Setup(m => m.GetGroupsToken()).Returns(TaskAsyncHelper.FromResult(string.Empty));
                transport.Setup(m => m.ProcessRequest(It.IsAny<Connection>())).Returns(TaskAsyncHelper.Empty);

                var transportManager = new Mock<ITransportManager>();
                transportManager.Setup(m => m.GetTransport(context.MockHttpContext.Object)).Returns(transport.Object);

                var protectedData = new Mock<IProtectedData>();
                protectedData.Setup(m => m.Unprotect(It.IsAny<string>(), It.IsAny<string>()))
                             .Returns<string, string>((value, purpose) =>  value);

                var connection = new Mock<PersistentConnection>() { CallBase = true };
                var onDisconnectedCalled = false;
                connection.Protected().Setup("OnDisconnected", context.MockRequest.Object, "1", false).Callback(() =>
                {
                    onDisconnectedCalled = true;
                });

                var sp = ServiceProviderHelper.CreateServiceProvider(services =>
                {
                    services.AddInstance<ITransportManager>(transportManager.Object);
                    services.AddInstance<IProtectedData>(protectedData.Object);
                });

                connection.Object.Initialize(sp);

                // Act
                connection.Object.ProcessRequest(context.MockHttpContext.Object).Wait();
                transport.Object.Disconnected(/* clean: */ false);

                // Assert
                Assert.True(onDisconnectedCalled);
            }
        }

        public class VerifyGroups
        {
            [Fact]
            public void MissingGroupTokenReturnsEmptyList()
            {
                var groups = DoVerifyGroups(groupsToken: null, connectionId: null);

                Assert.Equal(0, groups.Count);
            }

            [Fact]
            public void NullProtectedDataTokenReturnsEmptyList()
            {
                var groups = DoVerifyGroups(groupsToken: "groups", connectionId: null, hasProtectedData: false);

                Assert.Equal(0, groups.Count);
            }

            [Fact]
            public void GroupsTokenWithInvalidConnectionIdReturnsEmptyList()
            {
                var groups = DoVerifyGroups(groupsToken: @"wrong:[""g1"",""g2""]", connectionId: "id");

                Assert.Equal(0, groups.Count);
            }

            [Fact]
            public void GroupsAreParsedFromToken()
            {
                var groups = DoVerifyGroups(groupsToken: @"id:[""g1"",""g2""]", connectionId: "id");

                Assert.Equal(2, groups.Count);
                Assert.Equal("g1", groups[0]);
                Assert.Equal("g2", groups[1]);
            }

            private static IList<string> DoVerifyGroups(string groupsToken, string connectionId, bool hasProtectedData = true)
            {
                var connection = new Mock<PersistentConnection>() { CallBase = true };
                var qs = new Dictionary<string, string>();
                var context = new TestContext("/", qs);
                context.MockResponse.SetupProperty(r => r.StatusCode);
                qs["transport"] = "serverSentEvents";
                qs["connectionToken"] = "1";
                qs["groupsToken"] = groupsToken;

                var protectedData = new Mock<IProtectedData>();
                protectedData.Setup(m => m.Protect(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns<string, string>((value, purpose) => value);

                protectedData.Setup(m => m.Unprotect(It.IsAny<string>(), It.IsAny<string>()))
                             .Returns<string, string>((value, purpose) => hasProtectedData ? value : null);

                var sp = ServiceProviderHelper.CreateServiceProvider(services =>
                {
                    services.AddInstance<IProtectedData>(protectedData.Object);
                });

                connection.Object.Initialize(sp);

                return connection.Object.VerifyGroups(connectionId, groupsToken);
            }
        }

        public class GetConnectionId
        {
            [Fact]
            public void UnprotectedConnectionTokenFails()
            {
                var connection = new Mock<PersistentConnection>() { CallBase = true };
                var context = new TestContext("/");

                var protectedData = new Mock<IProtectedData>();
                protectedData.Setup(m => m.Protect(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns<string, string>((value, purpose) => value);
                protectedData.Setup(m => m.Unprotect(It.IsAny<string>(), It.IsAny<string>()))
                             .Throws<InvalidOperationException>();

                var sp = ServiceProviderHelper.CreateServiceProvider(services =>
                {
                    services.AddInstance<IProtectedData>(protectedData.Object);
                });

                connection.Object.Initialize(sp);

                string connectionId;
                string message;
                int statusCode;

                Assert.Equal(false, connection.Object.TryGetConnectionId(context.MockHttpContext.Object, "1", out connectionId, out message, out statusCode));
                Assert.Equal(null, connectionId);
                Assert.Equal(400, statusCode);
            }

            [Fact]
            public void NullUnprotectedConnectionTokenFails()
            {
                var connection = new Mock<PersistentConnection>() { CallBase = true };
                var context = new TestContext("/");

                var protectedData = new Mock<IProtectedData>();
                protectedData.Setup(m => m.Protect(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns<string, string>((value, purpose) => value);
                protectedData.Setup(m => m.Unprotect(It.IsAny<string>(), It.IsAny<string>())).Returns((string)null);

                var sp = ServiceProviderHelper.CreateServiceProvider(services =>
                {
                    services.AddInstance<IProtectedData>(protectedData.Object);
                });

                connection.Object.Initialize(sp);

                string connectionId;
                string message;
                int statusCode;

                Assert.Equal(false, connection.Object.TryGetConnectionId(context.MockHttpContext.Object, "1", out connectionId, out message, out statusCode));
                Assert.Equal(null, connectionId);
                Assert.Equal(400, statusCode);
            }

            [Fact]
            public void UnauthenticatedUserWithAuthenticatedTokenFails()
            {
                var connection = new Mock<PersistentConnection>() { CallBase = true };
                var context = new TestContext("/");

                var protectedData = new Mock<IProtectedData>();
                protectedData.Setup(m => m.Protect(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns<string, string>((value, purpose) => value);
                protectedData.Setup(m => m.Unprotect(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((value, purpose) => value);

                var sp = ServiceProviderHelper.CreateServiceProvider(services =>
                {
                    services.AddInstance<IProtectedData>(protectedData.Object);
                });

                connection.Object.Initialize(sp);

                string connectionId;
                string message;
                int statusCode;

                Assert.Equal(false, connection.Object.TryGetConnectionId(context.MockHttpContext.Object, "1:::11:::::::1:1", out connectionId, out message, out statusCode));
                Assert.Equal(403, statusCode);
            }

            [Fact]
            public void AuthenticatedUserNameMatches()
            {
                var connection = new Mock<PersistentConnection>() { CallBase = true };
                var context = new TestContext("/");
                context.MockHttpContext.Setup(m => m.User)
                       .Returns(new ClaimsPrincipal(new GenericIdentity("Name")));

                var protectedData = new Mock<IProtectedData>();
                protectedData.Setup(m => m.Protect(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns<string, string>((value, purpose) => value);
                protectedData.Setup(m => m.Unprotect(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((value, purpose) => value);

                var sp = ServiceProviderHelper.CreateServiceProvider(services =>
                {
                    services.AddInstance<IProtectedData>(protectedData.Object);
                });

                connection.Object.Initialize(sp);

                string connectionId;
                string message;
                int statusCode;

                Assert.Equal(true, connection.Object.TryGetConnectionId(context.MockHttpContext.Object, "1:Name", out connectionId, out message, out statusCode));
                Assert.Equal("1", connectionId);
            }

            [Fact]
            public void AuthenticatedUserWithColonsInUserName()
            {
                var connection = new Mock<PersistentConnection>() { CallBase = true };
                var context = new TestContext("/");
                context.MockHttpContext.Setup(m => m.User)
                       .Returns(new ClaimsPrincipal(new GenericIdentity("::11:::::::1:1")));

                string connectionId = Guid.NewGuid().ToString("d");

                var protectedData = new Mock<IProtectedData>();
                protectedData.Setup(m => m.Protect(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns<string, string>((value, purpose) => value);
                protectedData.Setup(m => m.Unprotect(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((value, purpose) => value);

                var sp = ServiceProviderHelper.CreateServiceProvider(services =>
                {
                    services.AddInstance<IProtectedData>(protectedData.Object);
                });

                connection.Object.Initialize(sp);

                string cid;
                string message;
                int statusCode;

                Assert.Equal(true, connection.Object.TryGetConnectionId(context.MockHttpContext.Object, connectionId + ":::11:::::::1:1", out cid, out message, out statusCode));
                Assert.Equal(connectionId, cid);
            }
        }
    }
}
