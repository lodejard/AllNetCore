﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;

namespace TestApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Out.WriteLine("** Standard Out 1 **");
            Console.Error.WriteLine("** Standard Error 1 **");
            Console.Out.WriteLine("** Standard Out 2 **");
            Console.Error.WriteLine("** Standard Error 2 **");
        }
    }
}
