// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using Microsoft.AspNet.SignalR;
using Microsoft.Framework.ConfigurationModel;

namespace Microsoft.Framework.DependencyInjection
{
    public static class SignalRServiceCollectionExtensions
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
