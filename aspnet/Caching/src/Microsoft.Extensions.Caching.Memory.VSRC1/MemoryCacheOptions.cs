// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Internal.VSRC1;
using Microsoft.Extensions.OptionsModel.VSRC1;

namespace Microsoft.Extensions.Caching.Memory.VSRC1
{
    public class MemoryCacheOptions : IOptions<MemoryCacheOptions>
    {
        public ISystemClock Clock { get; set; }

        public bool CompactOnMemoryPressure { get; set; } = true;

        public TimeSpan ExpirationScanFrequency { get; set; } = TimeSpan.FromMinutes(1);

        MemoryCacheOptions IOptions<MemoryCacheOptions>.Value
        {
            get { return this; }
        }
    }
}