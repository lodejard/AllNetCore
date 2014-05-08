// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

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

            return libraries.Select(library => Assembly.Load(new AssemblyName(library.Name)))
                            .ToList();
        }
    }
}
