// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Abstractions;
using Microsoft.AspNet.DependencyInjection;
using Microsoft.AspNet.DependencyInjection.Fallback;
using Microsoft.AspNet.SignalR.Hosting;

namespace Microsoft.AspNet.SignalR
{
    public static class BuilderExtensions
    {
        /// <summary>
        /// Maps SignalR hubs to the app builder pipeline at "/signalr".
        /// </summary>
        /// <param name="builder">The app builder</param>
        public static IBuilder MapSignalR(this IBuilder builder)
        {
            return builder.MapSignalR(new HubConfiguration());
        }

        /// <summary>
        /// Maps SignalR hubs to the app builder pipeline at "/signalr".
        /// </summary>
        /// <param name="builder">The app builder</param>
        /// <param name="configuration">The <see cref="HubConfiguration"/> to use</param>
        public static IBuilder MapSignalR(this IBuilder builder, HubConfiguration configuration)
        {
            return builder.MapSignalR("/signalr", configuration);
        }

        /// <summary>
        /// Maps SignalR hubs to the app builder pipeline at the specified path.
        /// </summary>
        /// <param name="builder">The app builder</param>
        /// <param name="path">The path to map signalr hubs</param>
        /// <param name="configuration">The <see cref="HubConfiguration"/> to use</param>
        public static IBuilder MapSignalR(this IBuilder builder, string path, HubConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            return builder.Map(path, subApp => subApp.RunSignalR(configuration));
        }

        /// <summary>
        /// Adds SignalR hubs to the app builder pipeline.
        /// </summary>
        /// <param name="builder">The app builder</param>
        public static void RunSignalR(this IBuilder builder)
        {
            builder.RunSignalR(new HubConfiguration());
        }

        /// <summary>
        /// Adds SignalR hubs to the app builder pipeline.
        /// </summary>
        /// <param name="builder">The app builder</param>
        /// <param name="configuration">The <see cref="HubConfiguration"/> to use</param>
        public static void RunSignalR(this IBuilder builder, HubConfiguration configuration)
        {
            builder.UseMiddleware<HubDispatcherMiddleware>(configuration);
        }

        /// <summary>
        /// Maps the specified SignalR <see cref="PersistentConnection"/> to the app builder pipeline at 
        /// the specified path.
        /// </summary>
        /// <typeparam name="TConnection">The type of <see cref="PersistentConnection"/></typeparam>
        /// <param name="builder">The app builder</param>
        /// <param name="path">The path to map the <see cref="PersistentConnection"/></param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is syntactic sugar")]
        public static IBuilder MapSignalR<TConnection>(this IBuilder builder, string path) where TConnection : PersistentConnection
        {
            return builder.MapSignalR(path, typeof(TConnection), new ConnectionConfiguration());
        }

        /// <summary>
        /// Maps the specified SignalR <see cref="PersistentConnection"/> to the app builder pipeline at 
        /// the specified path.
        /// </summary>
        /// <typeparam name="TConnection">The type of <see cref="PersistentConnection"/></typeparam>
        /// <param name="builder">The app builder</param>
        /// <param name="path">The path to map the persistent connection</param>
        /// <param name="configuration">The <see cref="ConnectionConfiguration"/> to use</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is syntactic sugar")]
        public static IBuilder MapSignalR<TConnection>(this IBuilder builder, string path, ConnectionConfiguration configuration) where TConnection : PersistentConnection
        {
            return builder.MapSignalR(path, typeof(TConnection), configuration);
        }

        /// <summary>
        /// Maps the specified SignalR <see cref="PersistentConnection"/> to the app builder pipeline at 
        /// the specified path.
        /// </summary>
        /// <param name="builder">The app builder</param>
        /// <param name="path">The path to map the persistent connection</param>
        /// <param name="connectionType">The type of <see cref="PersistentConnection"/></param>
        /// <param name="configuration">The <see cref="ConnectionConfiguration"/> to use</param>
        public static IBuilder MapSignalR(this IBuilder builder, string path, Type connectionType, ConnectionConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            return builder.Map(path, subApp => subApp.RunSignalR(connectionType, configuration));
        }

        /// <summary>
        /// Adds the specified SignalR <see cref="PersistentConnection"/> to the app builder.
        /// </summary>
        /// <typeparam name="TConnection">The type of <see cref="PersistentConnection"/></typeparam>
        /// <param name="builder">The app builder</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is syntactic sugar")]
        public static void RunSignalR<TConnection>(this IBuilder builder) where TConnection : PersistentConnection
        {
            builder.RunSignalR<TConnection>(new ConnectionConfiguration());
        }

        /// <summary>
        /// Adds the specified SignalR <see cref="PersistentConnection"/> to the app builder.
        /// </summary>
        /// <typeparam name="TConnection">The type of <see cref="PersistentConnection"/></typeparam>
        /// <param name="builder">The app builder</param>
        /// <param name="configuration">The <see cref="ConnectionConfiguration"/> to use</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is syntactic sugar")]
        public static void RunSignalR<TConnection>(this IBuilder builder, ConnectionConfiguration configuration) where TConnection : PersistentConnection
        {
            builder.RunSignalR(typeof(TConnection), configuration);
        }

        /// <summary>
        /// Adds the specified SignalR <see cref="PersistentConnection"/> to the app builder.
        /// </summary>
        /// <param name="builder">The app builder</param>
        /// <param name="connectionType">The type of <see cref="PersistentConnection"/></param>
        /// <param name="configuration">The <see cref="ConnectionConfiguration"/> to use</param>
        /// <returns></returns>
        public static void RunSignalR(this IBuilder builder, Type connectionType, ConnectionConfiguration configuration)
        {
            builder.UseMiddleware<PersistentConnectionMiddleware>(connectionType, configuration);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This class wires up new dependencies from the host")]
        private static IBuilder UseMiddleware<T>(this IBuilder builder, params object[] args)
        {
            return builder.Use(next =>
            {
                IServiceProvider sp = builder.ServiceProvider;
                var typeActivator = sp.GetService<ITypeActivator>();

                // TODO: Handle errors when requires services haven't been registered

                var instance = typeActivator.CreateInstance(sp, typeof(T), new[] { next }.Concat(args).ToArray());
                var invoke = typeof(T).GetTypeInfo().GetDeclaredMethod("Invoke");
                return (RequestDelegate)invoke.CreateDelegate(typeof(RequestDelegate), instance);
            });
        }

        // Temporary until we get map middleware
        private static IBuilder Map(this IBuilder app, string pathMatch, Action<IBuilder> configuration)
        {
            // put middleware in pipeline before creating branch
            var options = new MapOptions { PathMatch = new PathString(pathMatch) };
            IBuilder result = app.Use(next => MapMiddleware(next, options));

            // create branch and assign to options
            IBuilder branch = app.New();
            configuration(branch);
            options.Branch = branch.Build();

            return result;
        }

        private static RequestDelegate MapMiddleware(RequestDelegate next, MapOptions options)
        {
            return async context =>
            {
                PathString path = context.Request.Path;

                PathString remainingPath;
                if (path.StartsWithSegments(options.PathMatch, out remainingPath))
                {
                    // Update the path
                    PathString pathBase = context.Request.PathBase;
                    context.Request.PathBase = pathBase + options.PathMatch;
                    context.Request.Path = remainingPath;

                    await options.Branch(context);

                    context.Request.PathBase = pathBase;
                    context.Request.Path = path;
                }
                else
                {
                    await next(context);
                }
            };
        }

        private class MapOptions
        {
            public PathString PathMatch { get; set; }
            public RequestDelegate Branch { get; set; }
        }
    }
}
