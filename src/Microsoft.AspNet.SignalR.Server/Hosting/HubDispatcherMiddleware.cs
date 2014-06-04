// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.SignalR.Hosting
{
    public class HubDispatcherMiddleware
    {
        private readonly IOptionsAccessor<SignalROptions> _optionsAccessor;
        private readonly IServiceProvider _serviceProvider;

        public HubDispatcherMiddleware(RequestDelegate next,
                                       IOptionsAccessor<SignalROptions> optionsAccessor,
                                       IServiceProvider serviceProvider)
        {
            _optionsAccessor = optionsAccessor;
            _serviceProvider = serviceProvider;
        }

        public Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (JsonUtility.TryRejectJSONPRequest(_optionsAccessor.Options, context))
            {
                return TaskAsyncHelper.Empty;
            }

            var dispatcher = new HubDispatcher(_optionsAccessor);

            dispatcher.Initialize(_serviceProvider);

            return dispatcher.ProcessRequest(context);
        }
    }
}
