// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using Newtonsoft.Json;

namespace Microsoft.AspNet.SignalR
{
    public static class SignalRServices
    {
        internal static IEnumerable<IServiceDescriptor> GetDefaultServices(IConfiguration configuration = null)
        {
            var serviceDescriber = new ServiceDescriber(configuration);

            // REVIEW: All singletons can't be right but we're just doing this because
            // we had this before

            yield return serviceDescriber.Singleton<IMessageBus, MessageBus>();
            yield return serviceDescriber.Singleton<IMemoryPool, MemoryPool>();
            yield return serviceDescriber.Singleton<IStringMinifier, StringMinifier>();
            yield return serviceDescriber.Singleton<ITransportManager, TransportManager>();
            yield return serviceDescriber.Singleton<ITransportHeartbeat, TransportHeartbeat>();
            yield return serviceDescriber.Singleton<IConnectionManager, ConnectionManager>();
            yield return serviceDescriber.Singleton<IAckHandler, AckHandler>();
            yield return serviceDescriber.Singleton<IAssemblyLocator, DefaultAssemblyLocator>();
            yield return serviceDescriber.Singleton<IHubManager, DefaultHubManager>();
            yield return serviceDescriber.Singleton<IMethodDescriptorProvider, ReflectedMethodDescriptorProvider>();
            yield return serviceDescriber.Singleton<IHubDescriptorProvider, ReflectedHubDescriptorProvider>();
            yield return serviceDescriber.Singleton<IPerformanceCounterManager, PerformanceCounterManager>();
            yield return serviceDescriber.Singleton<JsonSerializer, JsonSerializer>();
            yield return serviceDescriber.Singleton<IUserIdProvider, PrincipalUserIdProvider>();
            yield return serviceDescriber.Singleton<IParameterResolver, DefaultParameterResolver>();
            yield return serviceDescriber.Singleton<IHubActivator, DefaultHubActivator>();
            yield return serviceDescriber.Singleton<IJavaScriptProxyGenerator, DefaultJavaScriptProxyGenerator>();
            yield return serviceDescriber.Singleton<IJavaScriptMinifier, NullJavaScriptMinifier>();
            yield return serviceDescriber.Singleton<IHubRequestParser, HubRequestParser>();

            // REVIEW: This used to be lazy
            var pipeline = new HubPipeline();
            pipeline.AddModule(new AuthorizeModule());

            yield return serviceDescriber.Instance<IHubPipeline>(pipeline);
            yield return serviceDescriber.Instance<IHubPipelineInvoker>(pipeline);

            // TODO: Just use the new IDataProtectionProvider abstraction directly here
            yield return serviceDescriber.Singleton<IProtectedData, DataProtectionProviderProtectedData>();
        }
    }
}
