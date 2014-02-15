// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
#if NET45
using System.Net.WebSockets;
#endif

namespace Microsoft.AspNet.SignalR.WebSockets
{
    internal sealed class WebSocketMessage
    {
#if NET45
        public static readonly WebSocketMessage EmptyTextMessage = new WebSocketMessage(String.Empty, WebSocketMessageType.Text);
        public static readonly WebSocketMessage EmptyBinaryMessage = new WebSocketMessage(new byte[0], WebSocketMessageType.Binary);
        public static readonly WebSocketMessage CloseMessage = new WebSocketMessage(null, WebSocketMessageType.Close);

        public readonly object Data;
        public readonly WebSocketMessageType MessageType;

        public WebSocketMessage(object data, WebSocketMessageType messageType)
        {
            Data = data;
            MessageType = messageType;
        }
#endif
    }
}
