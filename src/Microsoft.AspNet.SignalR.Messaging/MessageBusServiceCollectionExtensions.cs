// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.Framework.ConfigurationModel;

namespace Microsoft.Framework.DependencyInjection
{
    public static class MessageBusServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageBus(this IServiceCollection services)
        {
            return services.AddMessageBus(configuration: null);
        }

        public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
        {
            var describe = new ServiceDescriber(configuration);

            // Dependencies
            services.AddOptions(configuration);

            // SignalR services
            services.TryAdd(describe.Singleton<IMessageBus, MessageBus>());
            services.TryAdd(describe.Singleton<IStringMinifier, StringMinifier>());
            services.TryAdd(describe.Singleton<IPerformanceCounterManager, PerformanceCounterManager>());

            return services;
        }
    }
}
