// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using Microsoft.AspNet.ConfigurationModel;
using Microsoft.AspNet.DependencyInjection;

namespace Microsoft.AspNet.SignalR
{
    public static class ServiceCollectionExtensions
    {
        public static ServiceCollection AddSignalR(this ServiceCollection services)
        {
            return services.Add(SignalRServices.GetDefaultServices());
        }

        public static ServiceCollection AddSignalR(this ServiceCollection services, IConfiguration configuration)
        {
            return services.Add(SignalRServices.GetDefaultServices(configuration));
        }
    }
}
