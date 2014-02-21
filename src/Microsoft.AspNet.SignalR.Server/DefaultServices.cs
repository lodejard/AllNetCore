// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.DependencyInjection;
using Microsoft.AspNet.Logging;
using Microsoft.AspNet.SignalR.Configuration;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNet.SignalR.Transports;
using Newtonsoft.Json;

namespace Microsoft.AspNet.SignalR
{
    public class DefaultServices
    {
        private IEnumerable<IServiceDescriptor> GetServices()
        {
            // TODO: Default services
            yield break;
            //var loggerFactory = new Lazy<ILoggerFactory>(() => new DiagnosticsLoggerFactory());
            //Register(typeof(ILogger), () => loggerFactory.Value);

            //var serverIdManager = new ServerIdManager();
            //Register(typeof(IServerIdManager), () => serverIdManager);

            //var serverMessageHandler = new Lazy<IServerCommandHandler>(() => new ServerCommandHandler(this));
            //Register(typeof(IServerCommandHandler), () => serverMessageHandler.Value);

            //var newMessageBus = new Lazy<IMessageBus>(() => new MessageBus(this));
            //Register(typeof(IMessageBus), () => newMessageBus.Value);

            //var stringMinifier = new Lazy<IStringMinifier>(() => new StringMinifier());
            //Register(typeof(IStringMinifier), () => stringMinifier.Value);

            //var jsonSerializer = new Lazy<JsonSerializer>();
            //Register(typeof(JsonSerializer), () => jsonSerializer.Value);

            //var transportManager = new Lazy<TransportManager>(() => new TransportManager(this));
            //Register(typeof(ITransportManager), () => transportManager.Value);

            //var configurationManager = new DefaultConfigurationManager();
            //Register(typeof(IConfigurationManager), () => configurationManager);

            //var transportHeartbeat = new Lazy<TransportHeartbeat>(() => new TransportHeartbeat(this));
            //Register(typeof(ITransportHeartbeat), () => transportHeartbeat.Value);

            //var connectionManager = new Lazy<ConnectionManager>(() => new ConnectionManager(this));
            //Register(typeof(IConnectionManager), () => connectionManager.Value);

            //var ackHandler = new Lazy<AckHandler>();
            //Register(typeof(IAckHandler), () => ackHandler.Value);

            //var userIdProvider = new PrincipalUserIdProvider();
            //Register(typeof(IUserIdProvider), () => userIdProvider);

            //var methodDescriptorProvider = new Lazy<ReflectedMethodDescriptorProvider>();
            //Register(typeof(IMethodDescriptorProvider), () => methodDescriptorProvider.Value);

            //var hubDescriptorProvider = new Lazy<ReflectedHubDescriptorProvider>(() => new ReflectedHubDescriptorProvider(this));
            //Register(typeof(IHubDescriptorProvider), () => hubDescriptorProvider.Value);

            //var parameterBinder = new Lazy<DefaultParameterResolver>();
            //Register(typeof(IParameterResolver), () => parameterBinder.Value);

            //var activator = new Lazy<DefaultHubActivator>(() => new DefaultHubActivator(this));
            //Register(typeof(IHubActivator), () => activator.Value);

            //var hubManager = new Lazy<DefaultHubManager>(() => new DefaultHubManager(this));
            //Register(typeof(IHubManager), () => hubManager.Value);

            //var proxyGenerator = new Lazy<DefaultJavaScriptProxyGenerator>(() => new DefaultJavaScriptProxyGenerator(this));
            //Register(typeof(IJavaScriptProxyGenerator), () => proxyGenerator.Value);

            //var requestParser = new Lazy<HubRequestParser>();
            //Register(typeof(IHubRequestParser), () => requestParser.Value);

            //var assemblyLocator = new Lazy<DefaultAssemblyLocator>(() => new DefaultAssemblyLocator());
            //Register(typeof(IAssemblyLocator), () => assemblyLocator.Value);

            //// Setup the default hub pipeline
            //var dispatcher = new Lazy<IHubPipeline>(() => new HubPipeline().AddModule(new AuthorizeModule()));
            //Register(typeof(IHubPipeline), () => dispatcher.Value);
            //Register(typeof(IHubPipelineInvoker), () => dispatcher.Value);
        }
    }
}
