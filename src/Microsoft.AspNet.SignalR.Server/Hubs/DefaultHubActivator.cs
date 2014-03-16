// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using Microsoft.AspNet.DependencyInjection;

namespace Microsoft.AspNet.SignalR.Hubs
{
    public class DefaultHubActivator : IHubActivator
    {
        private readonly ITypeActivator _typeActivator;

        public DefaultHubActivator(ITypeActivator typeActivator)
        {
            _typeActivator = typeActivator;
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

            return _typeActivator.CreateInstance(descriptor.HubType) as IHub;
        }
    }
}
