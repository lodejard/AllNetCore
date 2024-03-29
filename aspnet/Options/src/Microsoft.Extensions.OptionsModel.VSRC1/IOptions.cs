// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.OptionsModel.VSRC1
{
    public interface IOptions<out TOptions> where TOptions : class, new()
    {
        TOptions Value { get; }
    }
}