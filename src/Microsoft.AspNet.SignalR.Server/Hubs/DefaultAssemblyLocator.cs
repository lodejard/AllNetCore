// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Framework.Runtime;

namespace Microsoft.AspNet.SignalR.Hubs
{
    public class DefaultAssemblyLocator : IAssemblyLocator
    {
        private readonly ILibraryManager _libraryManager;

        public DefaultAssemblyLocator(ILibraryManager libraryManager)
        {
            _libraryManager = libraryManager;
        }

        public virtual IList<Assembly> GetAssemblies()
        {
            var libraries = _libraryManager.GetReferencingLibraries(typeof(Hub).GetTypeInfo().Assembly.GetName().Name);

            return libraries.SelectMany(l => l.LoadableAssemblies)
                            .Select(an => Assembly.Load(an))
                            .ToList();
        }
    }
}
