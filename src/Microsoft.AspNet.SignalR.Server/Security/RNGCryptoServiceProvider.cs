// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if K10 // TODO: Crypto
namespace System.Security.Cryptography
{
    public class RNGCryptoServiceProvider : IDisposable
    {
        public void Dispose()
        {

        }

        public void GetBytes(byte[] bytes)
        {
            var random = new Random((int)DateTime.UtcNow.Ticks);
            random.NextBytes(bytes);
        }
    }
}
#endif