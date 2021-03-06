// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Test.Utilities.CSharpSecurityCodeFixVerifier<
    Microsoft.NetFramework.CSharp.Analyzers.CSharpDoNotUseInsecureDtdProcessingInApiDesignAnalyzer,
    Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;
using VerifyVB = Test.Utilities.VisualBasicSecurityCodeFixVerifier<
    Microsoft.NetFramework.VisualBasic.Analyzers.BasicDoNotUseInsecureDtdProcessingInApiDesignAnalyzer,
    Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

namespace Microsoft.NetFramework.Analyzers.UnitTests
{
    public partial class DoNotUseInsecureDtdProcessingInApiDesignAnalyzerTests
    {
        private static DiagnosticResult GetCA3077NoConstructorCSharpResultAt(int line, int column, string name)
        {
            return new DiagnosticResult(DoNotUseInsecureDtdProcessingInApiDesignAnalyzer.RuleDoNotUseInsecureDtdProcessingInApiDesign).WithLocation(line, column).WithArguments(string.Format(MicrosoftNetFrameworkAnalyzersResources.XmlDocumentDerivedClassNoConstructorMessage, name));
        }

        private static DiagnosticResult GetCA3077NoConstructorBasicResultAt(int line, int column, string name)
        {
            return new DiagnosticResult(DoNotUseInsecureDtdProcessingInApiDesignAnalyzer.RuleDoNotUseInsecureDtdProcessingInApiDesign).WithLocation(line, column).WithArguments(string.Format(MicrosoftNetFrameworkAnalyzersResources.XmlDocumentDerivedClassNoConstructorMessage, name));
        }

        [Fact]
        public async Task NonXmlDocumentDerivedTypeWithNoConstructorShouldNotGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;
using System.Xml;

namespace TestNamespace
{
    class TestClass : XmlResolver
    {
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            throw new NotImplementedException();
        }
    }
}"
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System
Imports System.Xml

Namespace TestNamespace
    Class TestClass
        Inherits XmlResolver
        Public Overrides Function GetEntity(absoluteUri As Uri, role As String, ofObjectToReturn As Type) As Object
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace");
        }

        [Fact]
        public async Task NonXmlDocumentDerivedTypeWithConstructorShouldNotGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;
using System.Xml;

namespace TestNamespace
{
    class TestClass : XmlResolver
    {
        public TestClass() {}

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            throw new NotImplementedException();
        }
    }
}"
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System
Imports System.Xml

Namespace TestNamespace
    Class TestClass
        Inherits XmlResolver
        Public Sub New()
        End Sub

        Public Overrides Function GetEntity(absoluteUri As Uri, role As String, ofObjectToReturn As Type) As Object
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace");
        }

        [Fact]
        public async Task XmlDocumentDerivedTypeWithNoConstructorShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;
using System.Xml;

namespace TestNamespace
{
    class TestClass : XmlDocument {}
}",
                GetCA3077NoConstructorCSharpResultAt(7, 11, "TestClass")
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass
        Inherits XmlDocument
    End Class
End Namespace",
                GetCA3077NoConstructorBasicResultAt(5, 11, "TestClass")
            );
        }
    }
}
