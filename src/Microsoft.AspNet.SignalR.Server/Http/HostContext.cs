// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.SignalR.Http
{
    public class HostContext
    {
        // Exposed to user code
        public IRequest Request { get; private set; } 

        public IResponse Response { get; private set; }

        public HostContext(IRequest request, IResponse response)
        {
            Request = request;
            Response = response;
        }

        public HostContext(HttpContext context)
        {
            Request = new ServerRequest(context);
            Response = new ServerResponse(context);
        }
    }
}
