// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Messaging;
using Xunit;

namespace Microsoft.AspNet.SignalR.Server.Tests
{
    public class ScaleoutMessageFacts
    {
        [Fact]
        void FromBytesToBytesProducesCorrectValues()
        {
            var message = new Message("source", "key", "value");
            var message2 = new Message("string", "more", "str");
            var message3 = new Message("s", "l", "n");
            var scaleoutMessage = new ScaleoutMessage(new List<Message>() { message, message2, message3 });

            var bytes = scaleoutMessage.ToBytes();
            var msg = ScaleoutMessage.FromBytes(bytes);

            Assert.True(scaleoutMessage.Messages.Count == 3);
            Assert.True(scaleoutMessage.Messages[0].Source == msg.Messages[0].Source, "Source is not the same");
            Assert.True(scaleoutMessage.Messages[0].Key == msg.Messages[0].Key, "Key is not the same");
            Assert.True(scaleoutMessage.Messages[0].GetString() == msg.Messages[0].GetString(), "Value is not the same");
        }
    }
}