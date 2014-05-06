// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using Microsoft.AspNet.SignalR;
using Microsoft.Framework.ConfigurationModel;

namespace Microsoft.Framework.DependencyInjection
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
