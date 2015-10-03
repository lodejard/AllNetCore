using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.AspNet.SignalR.Tests
{
    public class DefaultHubManagerFacts
    {
        [Fact]
        public void GetValidHub()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);
            var hubDescriptor = hubManager.GetHub("CoreTestHub");

            Assert.NotNull(hubDescriptor);
            Assert.False(hubDescriptor.NameSpecified);
        }

        [Fact]
        public void GetInValidHub()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);
            var hubDescriptor = hubManager.GetHub("__ELLO__");

            Assert.Null(hubDescriptor);
        }

        [Fact]
        public void GetValidHubsWithoutPredicate()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);
            var hubDescriptors = hubManager.GetHubs(predicate: null);

            Assert.NotNull(hubDescriptors);

            HubDescriptor coreTestHub = hubDescriptors.First(descriptor => descriptor.Name == "CoreTestHub");

            Assert.NotNull(coreTestHub);
        }

        [Fact]
        public void GetValidHubsWithValidPredicate()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);
            var hubDescriptors = hubManager.GetHubs(descriptor => descriptor.Name == "CoreTestHub");

            Assert.NotNull(hubDescriptors);
            Assert.Equal(hubDescriptors.First().Name, "CoreTestHub");
        }

        [Fact]
        public void GetValidHubsWithInvalidPredicate()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);
            var hubDescriptors = hubManager.GetHubs(descriptor => descriptor.Name == "CoreTestHub_INVALIDHUB_____");

            // Still have an ienumerable sequence
            Assert.NotNull(hubDescriptors);
            // But there's nothing in the ienumerable
            Assert.Empty(hubDescriptors);
        }

        [Fact]
        public void GetValidHubMethod()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);
            var methodDescriptor = hubManager.GetHubMethod("CoreTestHubWithMethod", "AddNumbers", new IJsonValue[] { null, null });

            Assert.NotNull(methodDescriptor);
            Assert.Equal(methodDescriptor.Name, "AddNumbers");
            Assert.Equal(methodDescriptor.ReturnType, typeof(int));

            IList<ParameterDescriptor> parameters = methodDescriptor.Parameters;
            Assert.Equal(parameters.Count, 2);
            Assert.Equal(parameters[0].Name, "first");
            Assert.Equal(parameters[0].ParameterType, typeof(int));
            Assert.Equal(parameters[1].Name, "second");
            Assert.Equal(parameters[1].ParameterType, typeof(int));
        }

        [Fact]
        public void GetInvalidHubMethod()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);
            // The AddNumbers method has 2 parameters, so should not find the method
            var methodDescriptor = hubManager.GetHubMethod("CoreTestHubWithMethod", "AddNumbers", null);

            Assert.Null(methodDescriptor);
        }

        [Fact]
        public void GetHubMethodFromInvalidHub()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);
            // There is no ________________CoreTestHubWithMethod________________ name
            var methodDescriptor = hubManager.GetHubMethod("________________CoreTestHubWithMethod________________", "AddNumbers", new IJsonValue[] { null, null });

            Assert.Null(methodDescriptor);
        }

        [Fact]
        public void GetValidHubMethodsWithoutPredicate()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);
            var methodDescriptors = hubManager.GetHubMethods("CoreTestHubWithMethod", predicate: null);

            Assert.NotNull(methodDescriptors);

            MethodDescriptor coreTestHubMethod = methodDescriptors.First(descriptor => descriptor.Name == "AddNumbers");

            Assert.NotNull(coreTestHubMethod);
        }

        [Fact]
        public void GetValidHubMethodsWithPredicate()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);
            var methodDescriptors = hubManager.GetHubMethods("CoreTestHubWithMethod", descriptor => descriptor.Name == "AddNumbers");

            Assert.NotNull(methodDescriptors);
            Assert.Equal(methodDescriptors.First().Name, "AddNumbers");
        }

        [Fact]
        public void GetValidHubMethodsWithInvalidPredicate()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);
            var methodDescriptors = hubManager.GetHubMethods("CoreTestHubWithMethod", descriptor => descriptor.Name == "______AddNumbers______");

            // Still have an ienumerable sequence
            Assert.NotNull(methodDescriptors);
            // But there's nothing in the ienumerable
            Assert.Empty(methodDescriptors);
        }

        [Fact]
        public void ResolveValidHub()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);
            var hubDescriptor = hubManager.ResolveHub("CoreTestHub");

            Assert.NotNull(hubDescriptor);
        }

        [Fact]
        public void ResolveInvalidHub()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);
            var hubDescriptor = hubManager.ResolveHub("____CoreTestHub____");

            Assert.Null(hubDescriptor);
        }

        [Fact]
        public void ResolveHubsIsNotEmpty()
        {
            var sp = ServiceProviderHelper.CreateServiceProvider();
            var hubManager = ActivatorUtilities.CreateInstance<DefaultHubManager>(sp);
            var hubDescriptor = hubManager.ResolveHubs();

            Assert.NotNull(hubDescriptor);
            Assert.NotEmpty(hubDescriptor);
        }
    }
}
