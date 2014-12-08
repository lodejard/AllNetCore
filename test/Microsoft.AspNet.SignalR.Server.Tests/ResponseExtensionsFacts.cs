using System;
using System.IO;
using System.Text;
using Moq;
using Xunit;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.Http;
using System.Threading.Tasks;

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

