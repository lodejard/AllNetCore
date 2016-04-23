using System;
using Microsoft.AspNetCore.SignalR.Hubs;
using Xunit;

namespace Microsoft.AspNetCore.SignalR.Tests
{
    public class HubConnectionContextFacts
    {
        [Fact]
        public void GroupThrowsNullExceptionWhenGroupNameIsNull()
        {
            var hubConContext = new HubConnectionContext();
            Assert.Throws<ArgumentException>(() => hubConContext.Group(null));
        }

        [Fact]
        public void ClientThrowsNullExceptionWhenClientIdIsNull()
        {
            var hubConContext = new HubConnectionContext();
            Assert.Throws<ArgumentException>(() => hubConContext.Client(null));
        }
    }
}
