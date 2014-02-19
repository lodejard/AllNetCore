// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNet.Abstractions;
using Microsoft.AspNet.SignalR;

namespace Owin
{
    public static class ObsoleteOwinExtensions
    {
        [Obsolete("Use IBuilder.MapSignalR in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.")]
        public static IBuilder MapHubs(this IBuilder builder)
        {
            return builder.MapSignalR();
        }

        [Obsolete("Use IBuilder.MapSignalR in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.")]
        public static IBuilder MapHubs(this IBuilder builder, HubConfiguration configuration)
        {
            return builder.MapSignalR(configuration);
        }

        [Obsolete("Use IBuilder.MapSignalR in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.")]
        public static IBuilder MapHubs(this IBuilder builder, string path, HubConfiguration configuration)
        {
            if (IsEmptyOrForwardSlash(path))
            {
                builder.RunSignalR(configuration);
                return builder;
            }
            else
            {
                return builder.MapSignalR(path, configuration);
            }
        }

        [Obsolete("Use IBuilder.MapSignalR<TConnection> in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.")]
        public static IBuilder MapConnection<T>(this IBuilder builder, string path) where T : PersistentConnection
        {
            if (IsEmptyOrForwardSlash(path))
            {
                builder.RunSignalR<T>();
                return builder;
            }
            else
            {
                return builder.MapSignalR<T>(path);
            }
        }

        [Obsolete("Use IBuilder.MapSignalR<TConnection> in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.")]
        public static IBuilder MapConnection<T>(this IBuilder builder, string path, ConnectionConfiguration configuration) where T : PersistentConnection
        {
            if (IsEmptyOrForwardSlash(path))
            {
                builder.RunSignalR<T>(configuration);
                return builder;
            }
            else
            {
                return builder.MapSignalR<T>(path, configuration);
            }
        }

        [Obsolete("Use IBuilder.MapSignalR in an Owin Startup class. See http://go.microsoft.com/fwlink/?LinkId=320578 for more details.")]
        public static IBuilder MapConnection(this IBuilder builder, string path, Type connectionType, ConnectionConfiguration configuration)
        {
            if (IsEmptyOrForwardSlash(path))
            {
                builder.RunSignalR(connectionType, configuration);
                return builder;
            }
            else
            {
                return builder.MapSignalR(path, connectionType, configuration);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength")]
        private static bool IsEmptyOrForwardSlash(string path)
        {
            return path == String.Empty || path == "/";
        }
    }
}
