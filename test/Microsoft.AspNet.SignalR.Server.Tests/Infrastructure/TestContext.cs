using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Collections;
using Moq;

namespace Microsoft.AspNet.SignalR.Tests
{
    public class TestContext
    {
        public TestContext(string path)
            : this(path, query: null)
        {
        }

        public TestContext(string path, Dictionary<string, string> query)
            : this(path, query, form: null)
        {
        }

        public TestContext(string path, Dictionary<string, string> query, Dictionary<string, string> form)
        {
            ResponseContentType = null;
            ResponseBuffer = new List<string>();

            MockRequest = GetRequestForUrl(path, query, form);

            MockResponseHeaders = new Mock<IHeaderDictionary>();
            MockResponseHeaders.Setup(m => m.Set("X-Content-Type-Options", "nosniff"));

            var mockResponseBody = new Mock<Stream>();
            mockResponseBody.Setup(m => m.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback<byte[], int, int>((buffer, offset, count) =>
                    ResponseBuffer.Add(Encoding.UTF8.GetString(buffer, offset, count)));

            MockResponse = new Mock<HttpResponse>();
            MockResponse.SetupGet(m => m.Headers).Returns(MockResponseHeaders.Object);
            MockResponse.SetupGet(m => m.Body).Returns(mockResponseBody.Object);

            MockResponse.SetupSet(m => m.ContentType = It.IsAny<string>())
                .Callback<string>(type => ResponseContentType = type);

            MockHttpContext = new Mock<HttpContext>();
            MockHttpContext.SetupGet(m => m.Response).Returns(MockResponse.Object);
            MockHttpContext.SetupGet(m => m.Request).Returns(MockRequest.Object);
            MockHttpContext.SetupGet(m => m.RequestAborted).Returns(CancellationToken.None);

            MockRequest.SetupGet(m => m.HttpContext).Returns(MockHttpContext.Object);
            MockResponse.SetupGet(m => m.HttpContext).Returns(MockHttpContext.Object);
        }

        public Mock<HttpContext> MockHttpContext { get; set; }
        public Mock<HttpRequest> MockRequest { get; set; }
        public Mock<HttpResponse> MockResponse { get; set; }
        public Mock<IHeaderDictionary> MockResponseHeaders { get; set; }
        public List<string> ResponseBuffer { get; set; }
        public string ResponseContentType { get; set; }

        public static Mock<HttpRequest> GetRequestForUrl(
            string path,
            Dictionary<string, string> query = null,
            Dictionary<string, string> form = null)
        {
            if (query == null)
            {
                query = new Dictionary<string, string>();
            }
            if (form == null)
            {
                form = new Dictionary<string, string>();
            }

            var request = new Mock<HttpRequest>();
            request.Setup(m => m.Path).Returns(new PathString(path));

            var processedQuery = query.ToDictionary(kvp => kvp.Key, kvp => new[] { kvp.Value });
            request.Setup(m => m.Query).Returns(new ReadableStringCollection(processedQuery));

            var mockForm = new Mock<IFormCollection>();

            foreach (var kvp in form)
            {
                mockForm.SetupGet(m => m[kvp.Key]).Returns(kvp.Value);
            }

            request.Setup(m => m.ReadFormAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IFormCollection>(mockForm.Object));

            return request;
        }
    }
}
