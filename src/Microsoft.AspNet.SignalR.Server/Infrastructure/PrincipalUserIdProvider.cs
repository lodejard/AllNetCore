// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
    public class PrincipalUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (context.User != null && context.User.Identity != null)
            {
                return context.User.Identity.Name;
            }

            return null;
        }
    }
}
