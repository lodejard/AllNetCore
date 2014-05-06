// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.SignalR.Http
{
    public class ServerRequest : IRequest
    {
        private readonly ClaimsPrincipal _user;
        private readonly HttpRequest _request;

        public ServerRequest(HttpContext context)
        {
            _request = context.Request;
            _user = context.User;
        }

        public string LocalPath
        {
            get
            {
                return (_request.PathBase + _request.Path).Value;
            }
        }

        public IReadableStringCollection QueryString
        {
            get
            {
                return _request.Query;
            }
        }

        public IReadableStringCollection Headers
        {
            get
            {
                return _request.Headers;
            }
        }

        public IReadableStringCollection Cookies
        {
            get
            {
                return _request.Cookies;
            }
        }

        public ClaimsPrincipal User
        {
            get
            {
                return _user;
            }
        }

        public Task<IReadableStringCollection> ReadForm()
        {
            return _request.GetFormAsync();
        }
    }
}

