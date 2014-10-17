// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.Framework.Logging;
using Newtonsoft.Json;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
    /// <summary>
    /// Default <see cref="IConnectionManager"/> implementation.
    /// </summary>
    public class ConnectionManager : IConnectionManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPerformanceCounterManager _counters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionManager"/> class.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IDependencyResolver"/>.</param>
        public ConnectionManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _counters = _serviceProvider.GetRequiredService<IPerformanceCounterManager>();
        }

        /// <summary>
        /// Returns a <see cref="IPersistentConnectionContext"/> for the <see cref="PersistentConnection"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="PersistentConnection"/></typeparam>
        /// <returns>A <see cref="IPersistentConnectionContext"/> for the <see cref="PersistentConnection"/>.</returns>
        public IPersistentConnectionContext GetConnectionContext<T>() where T : PersistentConnection
        {
            return GetConnection(typeof(T));
        }

        /// <summary>
        /// Returns a <see cref="IPersistentConnectionContext"/> for the <see cref="PersistentConnection"/>.
        /// </summary>
        /// <param name="type">Type of the <see cref="PersistentConnection"/></param>
        /// <returns>A <see cref="IPersistentConnectionContext"/> for the <see cref="PersistentConnection"/>.</returns>
        public IPersistentConnectionContext GetConnection(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            string rawConnectionName = type.FullName;
            string connectionName = PrefixHelper.GetPersistentConnectionName(rawConnectionName);
            IConnection connection = GetConnectionCore(connectionName);

            return new PersistentConnectionContext(connection, new GroupManager(connection, PrefixHelper.GetPersistentConnectionGroupName(rawConnectionName)));
        }

        /// <summary>
        /// Returns a <see cref="IHubContext"/> for the specified <see cref="IHub"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="IHub"/></typeparam>
        /// <returns>a <see cref="IHubContext"/> for the specified <see cref="IHub"/></returns>
        public IHubContext GetHubContext<T>() where T : IHub
        {
            return GetHubContext(typeof(T).GetHubName());
        }

        /// <summary>
        /// Returns a <see cref="IHubContext"/>for the specified hub.
        /// </summary>
        /// <param name="hubName">Name of the hub</param>
        /// <returns>a <see cref="IHubContext"/> for the specified hub</returns>
        public IHubContext GetHubContext(string hubName)
        {
            var connection = GetConnectionCore(connectionName: null);
            var hubManager = _serviceProvider.GetRequiredService<IHubManager>();
            var pipelineInvoker = _serviceProvider.GetRequiredService<IHubPipelineInvoker>();

            hubManager.EnsureHub(hubName,
                _counters.ErrorsHubResolutionTotal,
                _counters.ErrorsHubResolutionPerSec,
                _counters.ErrorsAllTotal,
                _counters.ErrorsAllPerSec);

            return new HubContext(connection, pipelineInvoker, hubName);
        }

        /// <summary>
        /// Returns a <see cref="IHubContext{TClient}"/> for the specified <see cref="IHub"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="IHub"/></typeparam>
        /// <typeparam name="TClient">Interface implemented by the client proxy</typeparam>
        /// <returns>a <see cref="IHubContext{TClient}"/> for the specified <see cref="IHub"/></returns>
        public IHubContext<TClient> GetHubContext<T, TClient>()
            where T : IHub
            where TClient : class
        {
            return GetHubContext<TClient>(typeof(T).GetHubName());
        }

        /// <summary>
        /// Returns a <see cref="IHubContext{TClient}"/>for the specified hub.
        /// </summary>
        /// <param name="hubName">Name of the hub</param>
        /// <typeparam name="TClient">Interface implemented by the client proxy</typeparam>
        /// <returns>a <see cref="IHubContext{TClient}"/> for the specified hub</returns>
        public IHubContext<TClient> GetHubContext<TClient>(string hubName) where TClient : class
        {
            var dynamicContext = GetHubContext(hubName);
            return new HubContext<TClient>(dynamicContext);
        }

        internal Connection GetConnectionCore(string connectionName)
        {
            IList<string> signals = connectionName == null ? ListHelper<string>.Empty : new[] { connectionName };

            // Give this a unique id
            var connectionId = Guid.NewGuid().ToString();
            return new Connection(_serviceProvider.GetRequiredService<IMessageBus>(),
                                  _serviceProvider.GetRequiredService<JsonSerializer>(),
                                  connectionName,
                                  connectionId,
                                  signals,
                                  ListHelper<string>.Empty,
                                  _serviceProvider.GetRequiredService<ILoggerFactory>(),
                                  _serviceProvider.GetRequiredService<IAckHandler>(),
                                  _serviceProvider.GetRequiredService<IPerformanceCounterManager>(),
                                  _serviceProvider.GetRequiredService<IProtectedData>(),
                                  _serviceProvider.GetRequiredService<IMemoryPool>());
        }
    }
}
