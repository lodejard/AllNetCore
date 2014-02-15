// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;

namespace Microsoft.AspNet.SignalR.Hubs
{
#if NET45
    [Serializable]
#endif
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException() { }
        public NotAuthorizedException(string message) : base(message) { }
        public NotAuthorizedException(string message, Exception inner) : base(message, inner) { }
#if NET45
        protected NotAuthorizedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
}
