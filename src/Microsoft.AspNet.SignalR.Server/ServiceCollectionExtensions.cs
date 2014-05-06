// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using Microsoft.Framework.ConfigurationModel;
using Microsoft.AspNet.DependencyInjection;

namespace Microsoft.AspNet.SignalR
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalR(this IServiceCollection services)
        {
            return services.Add(SignalRServices.GetDefaultServices());
        }

        public static IServiceCollection AddSignalR(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Add(SignalRServices.GetDefaultServices(configuration));
        }
    }
}
