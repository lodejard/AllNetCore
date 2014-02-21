// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Abstractions;

namespace Microsoft.AspNet.SignalR.Http
{
    public class ServerRequest : IRequest
    {
        private IPrincipal _user;

        private readonly HttpRequest _request;

        public ServerRequest(HttpRequest request)
        {
            _request = request;

            // Cache user because AspNetWebSocket.CloseOutputAsync clears it. We need it during Hub.OnDisconnected
            // TODO: user
            // _user = _request.User;
        }

        // TODO
        public Uri Url
        {
            get
            {
                return null;
                // TODO
                //return _request.Uri;
            }
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

        public IPrincipal User
        {
            get
            {
                return _user;
            }
        }

        public Task<IReadableStringCollection> ReadForm()
        {
            // TODO: Form
            //IFormCollection form = await _request.ReadFormAsync();
            //return new ReadableStringCollectionWrapper(form);
            return TaskAsyncHelper.FromResult<IReadableStringCollection>(null);
        }
    }
}

