// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using Microsoft.AspNet.SignalR.Hubs;

namespace Microsoft.AspNet.SignalR
{
    /// <summary>
    /// Provides methods that communicate with SignalR connections that connected to a <see cref="Hub"/>.
    /// </summary>
    public abstract class Hub : HubBase
    {
        /// <summary>
        /// 
        /// </summary>
        public IHubCallerConnectionContext<dynamic> Clients
        {
            get
            {
                return ((IHub)this).Clients;
            }
            set
            {
                ((IHub)this).Clients = value;
            }
        }
    }
}
