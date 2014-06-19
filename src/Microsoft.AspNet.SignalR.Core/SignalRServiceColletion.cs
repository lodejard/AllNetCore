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

        public IServiceCollection SetupOptions(Action<SignalROptions> configure)
        {
            // The generic SetupOptions extension method is not found without a reference to "this"
            return _baseCollection.SetupOptions<SignalROptions>(configure);
        }
    }
}