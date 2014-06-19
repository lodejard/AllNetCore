// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.SignalR.WebSockets;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
    /// <summary>
    /// A buffering text writer that supports writing binary directly as well
    /// </summary>
    internal class BinaryTextWriter : BufferTextWriter, IBinaryWriter
    {
        public BinaryTextWriter(HttpResponse response) :
            base((data, state) => ((HttpResponse)state).Write(data), response, reuseBuffers: true, bufferSize: 128)
        {

        }

        public BinaryTextWriter(IWebSocket socket) :
            base((data, state) => ((IWebSocket)state).SendChunk(data), socket, reuseBuffers: false, bufferSize: 1024)
        {

        }


        public BinaryTextWriter(Action<ArraySegment<byte>, object> write, object state, bool reuseBuffers, int bufferSize) :
            base(write, state, reuseBuffers, bufferSize)
        {
        }

        public void Write(ArraySegment<byte> data)
        {
            Writer.Write(data);
        }
    }
}
