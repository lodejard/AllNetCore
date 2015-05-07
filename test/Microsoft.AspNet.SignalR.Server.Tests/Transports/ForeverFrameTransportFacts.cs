using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.Framework.DependencyInjection;
using Moq;
using Xunit;

namespace Microsoft.AspNet.SignalR.Tests
{
    public class ForeverFrameTransportFacts
    {
        [InlineData("</sCRiPT>", "</sCRiPT>")]
        [InlineData("</SCRIPT dosomething='false'>", "</SCRIPT dosomething='false'>")]
        [InlineData("<p>ELLO</p>", "<p>ELLO</p>")]
        public void ForeverFrameTransportEscapesTags(string data, string expected)
        {
            var context = new TestContext("/");
            var sp = ServiceProviderHelper.CreateServiceProvider();

            var ms = new MemoryStream();
            context.MockResponse.Setup(m => m.Body).Returns(ms);

            var fft = ActivatorUtilities.CreateInstance<ForeverFrameTransport>(sp, context.MockHttpContext.Object);

            AssertEscaped(fft, ms, data, expected);
        }

        [Theory]
        [InlineData("<script type=\"\"></script>", "\\u003cscript type=\"\"\\u003e\\u003c/script\\u003e")]
        [InlineData("<script type=''></script>", "\\u003cscript type=''\\u003e\\u003c/script\\u003e")]
        public void ForeverFrameTransportEscapesTagsWithPersistentResponse(string data, string expected)
        {
            var context = new TestContext("/");
            var sp = ServiceProviderHelper.CreateServiceProvider();

            var ms = new MemoryStream();
            context.MockResponse.Setup(m => m.Body).Returns(ms);

            var fft = ActivatorUtilities.CreateInstance<ForeverFrameTransport>(sp, context.MockHttpContext.Object);
            fft.ConnectionId = "1";

            AssertEscaped(fft, ms, GetWrappedResponse(data), expected);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("-100")]
        [InlineData("1,000")]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void ForeverFrameTransportThrowsOnInvalidFrameId(string frameId)
        {
            var qs = new Dictionary<string, string> { { "frameId", frameId } };
            var context = new TestContext("/", qs);
            var sp = ServiceProviderHelper.CreateServiceProvider();

            var ms = new MemoryStream();
            context.MockResponse.Setup(m => m.Body).Returns(ms);

            var fft = ActivatorUtilities.CreateInstance<ForeverFrameTransport>(sp, context.MockHttpContext.Object);
            var connection = new Mock<ITransportConnection>();

            Assert.Throws(typeof(InvalidOperationException), () => fft.InitializeResponse(connection.Object));
        }

        [Fact]
        public void ForeverFrameTransportSetsCorrectContentType()
        {
            var qs = new Dictionary<string, string> {
                { "frameId", "1" }
            };
            var context = new TestContext("/", qs);
            var sp = ServiceProviderHelper.CreateServiceProvider();

            var ms = new MemoryStream();
            context.MockResponse.SetupAllProperties();
            context.MockResponse.Setup(m => m.Body).Returns(ms);

            var fft = ActivatorUtilities.CreateInstance<ForeverFrameTransport>(sp, context.MockHttpContext.Object);
            fft.ConnectionId = "1";
            var connection = new Mock<ITransportConnection>();

            fft.InitializeResponse(connection.Object).Wait();

            Assert.Equal("text/html; charset=UTF-8", context.MockResponse.Object.ContentType);
        }

        [Fact]
        public void ForeverFrameTransportDisablesRequestBuffering()
        {
            var qs = new Dictionary<string, string> {
                { "frameId", "1" }
            };
            var context = new TestContext("/", qs);
            var sp = ServiceProviderHelper.CreateServiceProvider();

            var ms = new MemoryStream();
            var buffering = new Mock<IHttpBufferingFeature>();

            context.MockHttpContext.Setup(m => m.GetFeature<IHttpBufferingFeature>())
                .Returns(buffering.Object);
            context.MockResponse.SetupAllProperties();
            context.MockResponse.Setup(m => m.Body).Returns(ms);

            var fft = ActivatorUtilities.CreateInstance<ForeverFrameTransport>(sp, context.MockHttpContext.Object);
            fft.ConnectionId = "1";
            var connection = new Mock<ITransportConnection>();

            fft.InitializeResponse(connection.Object).Wait();

            buffering.Verify(m => m.DisableRequestBuffering(), Times.Once());
        }

        private static void AssertEscaped(ForeverFrameTransport fft, MemoryStream ms, object input, string expectedOutput)
        {
            fft.Send(input).Wait();

            string rawResponse = Encoding.UTF8.GetString(ms.ToArray());

            // Doing contains due to all the stuff that gets sent through the buffer
            Assert.True(rawResponse.Contains(expectedOutput));
        }

        private static void AssertEscaped(ForeverFrameTransport fft, MemoryStream ms, PersistentResponse input, string expectedOutput)
        {
            fft.Send(input).Wait();

            string rawResponse = Encoding.UTF8.GetString(ms.ToArray());

            // Doing contains due to all the stuff that gets sent through the buffer
            Assert.True(rawResponse.Contains(expectedOutput));
        }

        private static PersistentResponse GetWrappedResponse(string raw)
        {
            var data = Encoding.Default.GetBytes(raw);
            var message = new Message("foo", "key", new ArraySegment<byte>(data));

            var response = new PersistentResponse
            {
                Messages = new List<ArraySegment<Message>>
                {
                    new ArraySegment<Message>(new Message[] { message })
                }
            };

            return response;
        }
    }
}
