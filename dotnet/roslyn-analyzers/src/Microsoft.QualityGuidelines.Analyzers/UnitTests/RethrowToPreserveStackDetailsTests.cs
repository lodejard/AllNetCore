﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.UnitTests;
using Microsoft.QualityGuidelines.Analyzers;
using Xunit;

namespace Microsoft.QualityGuidelines.UnitTests
{
    public partial class RethrowToPreserveStackDetailsTests : DiagnosticAnalyzerTestBase
    {
        protected override DiagnosticAnalyzer GetBasicDiagnosticAnalyzer()
        {
            return new BasicRethrowToPreserveStackDetailsAnalyzer();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new CSharpRethrowToPreserveStackDetailsAnalyzer();
        }

        [Fact]
        public void CA2200CSharpTestWithLegalExceptionThrow()
        {
            VerifyCSharp(@"
using System;

class Program
{
    void CatchAndRethrowImplicitly()
    {
        try
        {
            throw new ArithmeticException();
        }
        catch (ArithmeticException e)
        { 
            throw;
        }
    }
}");
        }

        [Fact]
        public void CA2200CSharpTestWithLegalExceptionThrowMultiple()
        {
            VerifyCSharp(@"
using System;

class Program
{
    void CatchAndRethrowExplicitly()
    {
        try
        {
            throw new ArithmeticException();
            throw new Exception();
        }
        catch (ArithmeticException e)
        {
            var i = new Exception();
            throw i;
        }
    }
}");
        }

        [Fact]
        public void CA2200CSharpTestWithLegalExceptionThrowNested()
        {
            VerifyCSharp(@"
using System;

class Program
{
    void CatchAndRethrowExplicitly()
    {   
        try
        {
            try
            {
                throw new ArithmeticException();
            }
            catch (ArithmeticException e)
            {
                throw;
            }
            catch (ArithmeticException)
                try
                {
                    throw new ArithmeticException();
                }
                catch (ArithmeticException i)
                {
                    throw e;
                }
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }
}");
        }

        [Fact]
        public void CA2200CSharpTestWithIllegalExceptionThrow()
        {
            VerifyCSharp(@"
using System;

class Program
{
    void CatchAndRethrowExplicitly()
    {
        try
        {
            ThrowException();
        }
        catch (ArithmeticException e)
        {
            throw e;
        }
    }

    void ThrowException()
    {
        throw new ArithmeticException();
    }
}",
           GetCA2200CSharpResultAt(14, 13));
        }

        [Fact]
        public void CA2200CSharpTestWithIllegalExceptionThrowwithScope()
        {
            VerifyCSharp(@"
using System;

class Program
{
    void CatchAndRethrowExplicitly()
    {
        try
        {
            ThrowException();
        }
        catch (ArithmeticException e)
        {
            throw e;
        }
    }

    [|void ThrowException()
    {
        throw new ArithmeticException();
    }|]
}");
        }

        [Fact]
        public void CA2200CSharpTestWithIllegalExceptionThrowMultiple()
        {
            VerifyCSharp(@"
using System;

class Program
{
    void CatchAndRethrowExplicitly()
    {
        try
        {
            ThrowException();
        }
        catch (ArithmeticException e)
        {
            throw e;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    void ThrowException()
    {
        throw new ArithmeticException();
    }
}",
           GetCA2200CSharpResultAt(14, 13),
           GetCA2200CSharpResultAt(18, 13));
        }

        [Fact]
        public void CA2200CSharpTestWithIllegalExceptionThrowNested()
        {
            VerifyCSharp(@"
using System;

class Program
{
    void CatchAndRethrowExplicitly()
    {
        try
        {
            throw new ArithmeticException();
        }
        catch (ArithmeticException e)
        {
            try
            {
                throw new ArithmeticException();
            }
            catch (ArithmeticException i)
            {
                throw e;
            }
        }
    }
}",
           GetCA2200CSharpResultAt(20, 17));
        }

        [Fact]
        public void CA2200yVisualBasicTestWithLegalExceptionThrow()
        {
            VerifyBasic(@"
Imports System
Class Program
    Sub CatchAndRethrowExplicitly()
        Try
            Throw New ArithmeticException()
        Catch ex As Exception
            Throw
        End Try
    End Sub
End Class");
        }

        [Fact]
        public void CA2200VisualBasicTestWithLegalExceptionThrowMultiple()
        {
            VerifyBasic(@"
Imports System
Class Program
    Sub CatchAndRethrowExplicitly()
        Try
            Throw New ArithmeticException()
            Throw New Exception()
        Catch ex As Exception
            Dim i As New Exception()
            Throw i
        End Try
    End Sub
End Class");
        }

        [Fact]
        public void CA2200VisualBasicTestWithLegalExceptionThrowNested()
        {
            VerifyBasic(@"
Imports System
Class Program
    Sub CatchAndRethrowExplicitly()
        Try
            Try
                Throw New ArithmeticException()
            Catch ex As ArithmeticException
                Throw
            Catch i As ArithmeticException
                Try
                    Throw New ArithmeticException()
                Catch e As Exception
                    Throw ex
                End Try
            End Try
        Catch ex As Exception
            Throw
        End Try
    End Sub
End Class");
        }

        [Fact]
        public void CA2200VisualBasicTestWithIllegalExceptionThrow()
        {
            VerifyBasic(@"
Imports System
Class Program
    Sub CatchAndRethrowExplicitly()

        Try
            Throw New ArithmeticException()
        Catch e As ArithmeticException
            Throw e
        End Try
    End Sub
End Class",
           GetCA2200BasicResultAt(9, 13));
        }

        [Fact]
        public void CA2200VisualBasicTestWithIllegalExceptionThrowMultiple()
        {
            VerifyBasic(@"
Imports System
Class Program
    Sub CatchAndRethrowExplicitly()

        Try
            Throw New ArithmeticException()
        Catch e As ArithmeticException
            Throw e
        Catch e As Exception
            Throw e
        End Try
    End Sub
End Class",
           GetCA2200BasicResultAt(9, 13),
           GetCA2200BasicResultAt(11, 13));
        }

        [Fact]
        public void CA2200VisualBasicTestWithIllegalExceptionThrowMultipleWithScope()
        {
            VerifyBasic(@"
Imports System
Class Program
    Sub CatchAndRethrowExplicitly()

        Try
            Throw New ArithmeticException()
        Catch e As ArithmeticException
            Throw e
        [|Catch e As Exception
            Throw e
        End Try|]
    End Sub
End Class",
           GetCA2200BasicResultAt(11, 13));
        }

        [Fact]
        public void CA2200VisualBasicTestWithIllegalExceptionThrowNested()
        {
            VerifyBasic(@"
Imports System
Class Program
    Sub CatchAndRethrowExplicitly()

        Try
            Throw New ArithmeticException()
        Catch e As ArithmeticException
            Try
                Throw New ArithmeticException()
            Catch ex As Exception
                Throw e
            End Try
        End Try
    End Sub
End Class",
           GetCA2200BasicResultAt(12, 17));
        }

        private static DiagnosticResult GetCA2200BasicResultAt(int line, int column)
        {
            return GetBasicResultAt(line, column, RethrowToPreserveStackDetailsAnalyzer.RuleId, MicrosoftQualityGuidelinesAnalyzersResources.RethrowToPreserveStackDetailsMessage);
        }

        private static DiagnosticResult GetCA2200CSharpResultAt(int line, int column)
        {
            return GetCSharpResultAt(line, column, RethrowToPreserveStackDetailsAnalyzer.RuleId, MicrosoftQualityGuidelinesAnalyzersResources.RethrowToPreserveStackDetailsMessage);
        }
    }
}
