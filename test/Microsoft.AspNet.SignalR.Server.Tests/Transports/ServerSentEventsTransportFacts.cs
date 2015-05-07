using System.IO;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.Framework.DependencyInjection;
using Moq;
using Xunit;

namespace Microsoft.AspNet.SignalR.Tests
{
    public class ServerSentEventsTransportFacts
    {
        [Fact]
        public void ServerSentEventsTransportDisablesRequestBuffering()
        {
            var context = new TestContext("/");
            var sp = ServiceProviderHelper.CreateServiceProvider();

            var ms = new MemoryStream();
            var buffering = new Mock<IHttpBufferingFeature>();

            context.MockHttpContext.Setup(m => m.GetFeature<IHttpBufferingFeature>())
                .Returns(buffering.Object);
            context.MockResponse.SetupAllProperties();
            context.MockResponse.Setup(m => m.Body).Returns(ms);

            var sst = ActivatorUtilities.CreateInstance<ServerSentEventsTransport>(sp, context.MockHttpContext.Object);
            sst.ConnectionId = "1";
            var connection = new Mock<ITransportConnection>();

            sst.InitializeResponse(connection.Object).Wait();

            buffering.Verify(m => m.DisableRequestBuffering(), Times.Once());
        }
    }
}
