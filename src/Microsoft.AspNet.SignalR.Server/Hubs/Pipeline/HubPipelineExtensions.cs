// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System;
using Microsoft.AspNet.SignalR.Hubs;

namespace Microsoft.AspNet.SignalR
{
    public static class HubPipelineExtensions
    {
        /// <summary>
        /// Requiring Authentication adds an <see cref="AuthorizeModule"/> to the <see cref="HubOptions" /> with <see cref="IAuthorizeHubConnection"/>
        /// and <see cref="IAuthorizeHubMethodInvocation"/> authorizers that will be applied globally to all hubs and hub methods.
        /// These authorizers require that the <see cref="System.Security.Principal.IPrincipal"/>'s <see cref="System.Security.Principal.IIdentity"/> 
        /// IsAuthenticated for any clients that invoke server-side hub methods or receive client-side hub method invocations. 
        /// </summary>
        /// <param name="options">The <see cref="HubOptions" /> to which the <see cref="AuthorizeModule" /> will be added.</param>
        public static void RequireAuthentication(this HubOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("pipeline");
            }

            var authorizer = new AuthorizeAttribute();
            options.PipelineModules.Push(new AuthorizeModule(globalConnectionAuthorizer: authorizer, globalInvocationAuthorizer: authorizer));
        }
    }
}
