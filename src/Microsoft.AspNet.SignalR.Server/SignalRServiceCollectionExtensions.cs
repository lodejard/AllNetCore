// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.OptionsModel;
using Newtonsoft.Json;

namespace Microsoft.Framework.DependencyInjection
{
    public static class SignalRServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalR(this IServiceCollection services, Action<SignalROptions> configureOptions = null)
        {
            return services.AddSignalR(configuration: null, configureOptions: configureOptions);
        }

        public static IServiceCollection AddSignalR(this IServiceCollection services, IConfiguration configuration, Action<SignalROptions> configureOptions = null)
        {
            var describe = new ServiceDescriber(configuration);

            // Dependencies
            services.AddOptions(configuration);
            services.AddDataProtection(configuration);

            // SignalR services
            services.TryAdd(describe.Singleton<IMessageBus, MessageBus>());
            services.TryAdd(describe.Singleton<IMemoryPool, MemoryPool>());
            services.TryAdd(describe.Singleton<IStringMinifier, StringMinifier>());
            services.TryAdd(describe.Singleton<ITransportManager, TransportManager>());
            services.TryAdd(describe.Singleton<ITransportHeartbeat, TransportHeartbeat>());
            services.TryAdd(describe.Singleton<IConnectionManager, ConnectionManager>());
            services.TryAdd(describe.Singleton<IAckHandler, AckHandler>());
            services.TryAdd(describe.Singleton<IAssemblyLocator, DefaultAssemblyLocator>());
            services.TryAdd(describe.Singleton<IHubManager, DefaultHubManager>());
            services.TryAdd(describe.Singleton<IMethodDescriptorProvider, ReflectedMethodDescriptorProvider>());
            services.TryAdd(describe.Singleton<IHubDescriptorProvider, ReflectedHubDescriptorProvider>());
            services.TryAdd(describe.Singleton<IPerformanceCounterManager, PerformanceCounterManager>());
            services.TryAdd(describe.Singleton<JsonSerializer, JsonSerializer>());
            services.TryAdd(describe.Singleton<IUserIdProvider, PrincipalUserIdProvider>());
            services.TryAdd(describe.Singleton<IParameterResolver, DefaultParameterResolver>());
            services.TryAdd(describe.Singleton<IHubActivator, DefaultHubActivator>());
            services.TryAdd(describe.Singleton<IJavaScriptProxyGenerator, DefaultJavaScriptProxyGenerator>());
            services.TryAdd(describe.Singleton<IJavaScriptMinifier, NullJavaScriptMinifier>());
            services.TryAdd(describe.Singleton<IHubRequestParser, HubRequestParser>());
            services.TryAdd(describe.Singleton<IHubPipelineInvoker, HubPipeline>());

            // TODO: Just use the new IDataProtectionProvider abstraction directly here
            services.TryAdd(describe.Singleton<IProtectedData, DataProtectionProviderProtectedData>());

            // Setup the default SignalR options
            services.TryAdd(describe.Transient<IConfigureOptions<SignalROptions>, SignalROptionsSetup>());

            if (configuration != null)
            {
                services.Configure<SignalROptions>(configuration);
            }

            if (configureOptions != null)
            {
                services.ConfigureSignalR(configureOptions);
            }

            return services;
        }

        public static IServiceCollection ConfigureSignalR(this IServiceCollection services, Action<SignalROptions> configure)
        {
            return services.Configure(configure);
        }
    }
}
