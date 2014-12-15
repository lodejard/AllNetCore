using Microsoft.AspNet.Http;
using Moq;
using System.Text;
using Xunit;

namespace Microsoft.AspNet.SignalR.Tests.Hosting
{
    public class ResponseExtensionsFacts
    {
        [Fact]
        public void EndAsyncWritesUtf8BufferToResponse()
        {
            // Arrange
            var response = new Mock<HttpResponse>();
            string value = null;

            response.Setup(m => m.Body.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Callback<byte[], int, int>((data, off, count) =>
                    {
                        value = Encoding.UTF8.GetString(data, off, count);
                    });

            // Act
            response.Object.End("Hello World");

            // Assert
            Assert.Equal("Hello World", value);
        }
    }
}