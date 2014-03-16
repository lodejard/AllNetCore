// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.AspNet.Abstractions;
using Microsoft.AspNet.SignalR.Hosting;

namespace Microsoft.AspNet.SignalR.Hubs
{
    public class HubCallerContext
    {
        /// <summary>
        /// Gets the connection id of the calling client.
        /// </summary>
        public string ConnectionId { get; private set; }

        /// <summary>
        /// Gets the cookies for the request.
        /// </summary>
        public IReadableStringCollection RequestCookies
        {
            get
            {
                return Request.Cookies;
            }
        }

        /// <summary>
        /// Gets the headers for the request.
        /// </summary>
        public IReadableStringCollection Headers
        {
            get
            {
                return Request.Headers;
            }
        }

        /// <summary>
        /// Gets the querystring for the request.
        /// </summary>
        public IReadableStringCollection QueryString
        {
            get
            {
                return Request.QueryString;
            }
        }

        /// <summary>
        /// Gets the <see cref="IPrincipal"/> for the request.
        /// </summary>
        public IPrincipal User
        {
            get
            {
                return Request.User;
            }
        }

        /// <summary>
        /// Gets the <see cref="IRequest"/> for the current HTTP request.
        /// </summary>
        public IRequest Request { get; private set; }

        public HubCallerContext(IRequest request, string connectionId)
        {
            ConnectionId = connectionId;
            Request = request;
        }
    }
}
