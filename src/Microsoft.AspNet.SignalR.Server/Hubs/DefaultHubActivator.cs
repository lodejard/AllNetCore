// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.SignalR.Hubs
{
    public class DefaultHubActivator : IHubActivator
    {
        private readonly ITypeActivator _typeActivator;
        private readonly IServiceProvider _serviceProvider;

        public DefaultHubActivator(ITypeActivator typeActivator, IServiceProvider serviceProvider)
        {
            _typeActivator = typeActivator;
            _serviceProvider = serviceProvider;
        }

        public IHub Create(HubDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor");
            }

            if (descriptor.HubType == null)
            {
                return null;
            }

            return _typeActivator.CreateInstance(_serviceProvider, descriptor.HubType) as IHub;
        }
    }
}
