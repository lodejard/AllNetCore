// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Framework.ConfigurationModel;

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
            services.AddOptions(configuration);
            services.TryAdd(SignalRServices.GetDefaultServices(configuration));
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
