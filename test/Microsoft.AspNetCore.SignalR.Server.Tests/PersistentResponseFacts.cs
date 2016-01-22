using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.SignalR.Json;
using Microsoft.AspNetCore.SignalR.Messaging;
using Microsoft.AspNetCore.SignalR.Transports;
using Xunit;

namespace Microsoft.AspNetCore.SignalR.Tests
{
    public class PersistentResponseFacts
    {
        [Fact]
        public void WriteJsonWritesMessagesThatAreNotExcluded()
        {
            // Arrange
            var writer = new StringWriter();
            var response = new PersistentResponse(m => m.Key == "key2", w =>
            {
                w.Write("Baz");
            });
            response.Messages = new List<ArraySegment<Message>>();
            response.Messages.Add(new ArraySegment<Message>(new[] { new Message("1", "key", "value1"), 
                                                                    new Message("1", "key2", "value2") }, 0, 2));

            // Act
            ((IJsonWritable)response).WriteJson(writer);

            // Assert
            Assert.Equal(@"{""C"":""Baz"",""M"":[value1]}", writer.ToString());
        }

        [Fact]
        public void WriteJsonWritesSkipsCommands()
        {
            // Arrange
            var writer = new StringWriter();
            var response = new PersistentResponse(m => false, w =>
            {
                w.Write("Baz");
            });
            response.Messages = new List<ArraySegment<Message>>();
            response.Messages.Add(new ArraySegment<Message>(new[] { new Message("1", "key", "value1") { CommandId = "something"}, 
                                                                    new Message("1", "key2", "value2") }, 0, 2));

            // Act
            ((IJsonWritable)response).WriteJson(writer);

            // Assert
            Assert.Equal(@"{""C"":""Baz"",""M"":[value2]}", writer.ToString());
        }

        [Fact]
        public void WriteJsonSkipsNullGroups()
        {
            // Arrange
            var writer = new StringWriter();
            var response = new PersistentResponse(m => m.Key == "key2", w =>
            {
                w.Write("Baz");
            });
            response.Messages = new List<ArraySegment<Message>>();
            response.Messages.Add(new ArraySegment<Message>(new[] { new Message("1", "key", "value1"), 
                                                                    new Message("1", "key2", "value2") }, 0, 2));

            // Act
            ((IJsonWritable)response).WriteJson(writer);

            // Assert
            Assert.Equal(@"{""C"":""Baz"",""M"":[value1]}", writer.ToString());
        }

        [Fact]
        public void WriteJsonSkipsNullTransportDaa()
        {
            // Arrange
            var writer = new StringWriter();
            var response = new PersistentResponse(m => m.Key == "key2", w =>
            {
                w.Write("Baz");
            });
            response.Messages = new List<ArraySegment<Message>>();
            response.Messages.Add(new ArraySegment<Message>(new[] { new Message("1", "key", "value1"), 
                                                                    new Message("1", "key2", "value2") }, 0, 2));

            // Act
            ((IJsonWritable)response).WriteJson(writer);

            // Assert
            Assert.Equal(@"{""C"":""Baz"",""M"":[value1]}", writer.ToString());
        }
    }
}
