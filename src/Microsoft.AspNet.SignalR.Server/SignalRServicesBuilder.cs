using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Framework.ConfigurationModel;

namespace Microsoft.Framework.DependencyInjection
{
    public class SignalRServicesBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public SignalRServicesBuilder([NotNull] IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public virtual IServiceCollection ServiceCollection
        {
            get { return _serviceCollection; }
        }
    }
}