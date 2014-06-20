// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNet.SignalR
{
    [Flags]
    public enum TransportType
    {
        /// <summary>
        /// Every transport
        /// </summary>
        All = Streaming | LongPolling,

        /// <summary>
        /// All transports except for long-polling
        /// </summary>
        Streaming = WebSockets | ServerSentEvents | ForeverFrame,

        WebSockets = 0,
        ServerSentEvents = 1,
        ForeverFrame = 2,
        LongPolling = 4
    }
}