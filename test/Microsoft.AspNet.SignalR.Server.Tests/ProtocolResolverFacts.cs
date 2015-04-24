using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.AspNet.FeatureModel;
using Microsoft.AspNet.SignalR.Infrastructure;
using Moq;
using Xunit;

namespace Microsoft.AspNet.SignalR.Tests
{
    public class ProtocolResolverFacts
    {
        [Theory]
        [InlineData("1.0", "1.5", ".8", "1.0")]
        [InlineData("1.0", "1.5", "1.0", "1.0")]
        [InlineData("1.0", "1.5", "1.2.5", "1.2.5")]
        [InlineData("1.0", "1.5", "1.5", "1.5")]
        [InlineData("1.0", "1.5", "1.9", "1.5")]
        [InlineData("1.0", "1.1", "1.0.5", "1.0.5")]
        [InlineData("1.0", "1.1", "", "1.0")]
        public void ProtocolResolvesCorrectly(string minProtocol, string maxProtocol, string clientProtocol, string expectedProtocol)
        {
            var minProtocolVersion = new Version(minProtocol);
            var maxProtocolVersion = new Version(maxProtocol);
            var protocolResolver = new ProtocolResolver(minProtocolVersion, maxProtocolVersion);


            var queryStrings = new Dictionary<string, string>
            {
                {  "clientProtocol", clientProtocol }
            };

            var context = new TestContext("/negotite", queryStrings);
            
            var version = protocolResolver.Resolve(context.MockRequest.Object);

            Assert.Equal(version, new Version(expectedProtocol));
        }
    }
}
