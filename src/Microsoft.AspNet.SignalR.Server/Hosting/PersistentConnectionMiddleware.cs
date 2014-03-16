// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Abstractions;
using Microsoft.AspNet.DependencyInjection;
using Microsoft.AspNet.SignalR.Json;

namespace Microsoft.AspNet.SignalR.Hosting
{
    public class PersistentConnectionMiddleware
    {
        private readonly Type _connectionType;
        private readonly ConnectionConfiguration _configuration;
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public PersistentConnectionMiddleware(RequestDelegate next,
                                              Type connectionType,
                                              ConnectionConfiguration configuration,
                                              IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
            _connectionType = connectionType;
            _configuration = configuration;
        }

        public Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (JsonUtility.TryRejectJSONPRequest(_configuration, context))
            {
                return TaskAsyncHelper.Empty;
            }

            var connection = ActivatorUtilities.CreateInstance(_serviceProvider, _connectionType) as PersistentConnection;

            connection.Initialize(_serviceProvider);

            return connection.ProcessRequest(context);
        }
    }
}
