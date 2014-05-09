// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using Microsoft.AspNet.SignalR.Hubs;

namespace Microsoft.AspNet.SignalR
{
    /// <summary>
    /// Provides methods that communicate with SignalR connections that connected to a <see cref="Hub"/>.
    /// </summary>
    public abstract class Hub<T> : HubBase where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        public IHubCallerConnectionContext<T> Clients
        {
            get
            {
                var clients = ((IHub)this).Clients;
                return new TypedHubCallerConnectionContext<T>(clients);
            }
        }
    }
}
