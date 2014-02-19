// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.Abstractions;

namespace Microsoft.AspNet.SignalR.Owin
{
    public class ServerRequest : IRequest
    {

        private INameValueCollection _queryString;
        private INameValueCollection _headers;
        private IDictionary<string, Cookie> _cookies;
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

        public INameValueCollection QueryString
        {
            get
            {
                return LazyInitializer.EnsureInitialized(
                    ref _queryString, () =>
                    {
                        return new ReadableStringCollectionWrapper(_request.Query);
                    });
            }
        }

        public INameValueCollection Headers
        {
            get
            {
                return LazyInitializer.EnsureInitialized(
                    ref _headers, () =>
                    {
                        return new ReadableStringCollectionWrapper(_request.Headers);
                    });
            }
        }

        public IDictionary<string, Cookie> Cookies
        {
            get
            {
                return LazyInitializer.EnsureInitialized(
                    ref _cookies, () =>
                    {
                        var cookies = new Dictionary<string, Cookie>(StringComparer.OrdinalIgnoreCase);
                        foreach (var kv in _request.Cookies)
                        {
                            if (!cookies.ContainsKey(kv.Key))
                            {
                                // ??? Value is an array instead of a string
                                cookies.Add(kv.Key, new Cookie(kv.Key, kv.Value[0]));
                            }
                        }
                        return cookies;
                    });
            }
        }

        public IPrincipal User
        {
            get
            {
                return _user;
            }
        }


        // TODO
        public Task<INameValueCollection> ReadForm()
        {
            // TODO
            //IFormCollection form = await _request.ReadFormAsync();
            //return new ReadableStringCollectionWrapper(form);
            return TaskAsyncHelper.FromResult<INameValueCollection>(null);
        }
    }
}

