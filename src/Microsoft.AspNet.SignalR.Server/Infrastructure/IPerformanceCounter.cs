// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System.Diagnostics;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
    public interface IPerformanceCounter
    {
        string CounterName { get; }
        long Decrement();
        long Increment();
        long IncrementBy(long value);
#if NET45
        CounterSample NextSample();
#endif
        long RawValue { get; set; }
        void Close();
        void RemoveInstance();
    }
}
