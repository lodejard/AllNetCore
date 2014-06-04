// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using Microsoft.AspNet.SignalR;
using Microsoft.Framework.ConfigurationModel;

namespace Microsoft.Framework.DependencyInjection
{
    public static class SignalRServiceCollectionExtensions
    {
        public static SignalRServiceCollection AddSignalR(this IServiceCollection services)
        {
            services.Add(SignalRServices.GetDefaultServices());
            return new SignalRServiceCollection(services);
        }

        public static SignalRServiceCollection AddSignalR(this IServiceCollection services, IConfiguration configuration)
        {
            services.Add(SignalRServices.GetDefaultServices(configuration));
            return new SignalRServiceCollection(services);
        }
    }
}
