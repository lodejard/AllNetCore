// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.Framework.DependencyInjection
{
    /// <summary>
    /// A ServiceCollection with a SignalR specific SetupOptions method
    /// </summary>
    public class SignalRServiceCollection
    {
        private readonly IServiceCollection _baseCollection;

        public SignalRServiceCollection(IServiceCollection collection)
        {
            _baseCollection = collection;
        }

        public IServiceCollection Configure(Action<SignalROptions> configure)
        {
            return _baseCollection.ConfigureOptions(configure);
        }
    }
}