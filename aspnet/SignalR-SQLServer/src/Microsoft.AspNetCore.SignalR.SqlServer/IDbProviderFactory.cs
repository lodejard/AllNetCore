// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data;
using System.Data.Common;

namespace Microsoft.AspNetCore.SignalR.SqlServer
{
    public interface IDbProviderFactory
    {
#if NET451
        IDbConnection CreateConnection();
#else
        DbConnection CreateConnection();
#endif
        DbParameter CreateParameter();
    }
}