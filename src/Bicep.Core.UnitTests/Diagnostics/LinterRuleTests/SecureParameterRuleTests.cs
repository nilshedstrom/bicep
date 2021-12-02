// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class SecureParameterRuleTests : LinterRuleTestsBase
    {
        [DataRow(2, @"
param password string = 'xxxx'
param o object = {
    a: 1
}
var sum = 1 + 3
output sub int = sum
")]
        [DataTestMethod]
        public void NotSecureParam_TestPasses(int diagnosticCount, string text)
        {
            AssertLinterRuleDiagnostics(SecureParameterRule.Code, text, diagnosticCount);
        }

        [DataRow(0, @"
@secure()
param password string
var sum = 1 + 3
output sub int = sum
")]
        [DataRow(0, @"
@secure()
param poNoDefault object
")]
        [DataTestMethod]
        public void NoDefault_TestPasses(int diagnosticCount, string text)
        {
            AssertLinterRuleDiagnostics(SecureParameterRule.Code, text, diagnosticCount);
        }

        [DataRow(0, @"
@secure()
param password string
")]
        [DataTestMethod]
        public void EmptyString_TestPasses(int diagnosticCount, string text)
        {
            AssertLinterRuleDiagnostics(SecureParameterRule.Code, text, diagnosticCount);
        }

        [DataRow(0, @"
@secure()
param poEmpty object = {}
")]
        [DataTestMethod]
        public void EmptyObject_TestPasses(int diagnosticCount, string text)
        {
            AssertLinterRuleDiagnostics(SecureParameterRule.Code, text, diagnosticCount);
        }


        [DataRow(0, @"
param param3 int
")]
        [DataRow(2, @"
param param1 string = 'val3'
param param2 string = 'val4'
")]
        [DataTestMethod]
        public void InvalidNonEmptyDefault_TestFails(int diagnosticCount, string text)
        {
            AssertLinterRuleDiagnostics(SecureParameterRule.Code, text, diagnosticCount, OnCompileErrors.Fail);
        }

        [DataRow(1, @"
param poNotEmpty object = {
  abc: 1
}
")]
        [DataTestMethod]
        public void NonEmptySecureObject_TestFails(int diagnosticCount, string text)
        {
            AssertLinterRuleDiagnostics(SecureParameterRule.Code, text, diagnosticCount);
        }

        [DataRow(2, @"
param pi1 int = 1

param param1 string = 'val'

param param2 string =
")]
        [DataRow(0, @"
param pi1 int = 'wrong type'
")]
        [DataRow(1, @"
param psWrongType string = 123
")]
        [DataRow(1, @"
param o object = {
")]
        [DataRow(1, @"
param o object = {
    a:
}")]
        [DataRow(1, @"
param o object = {
    // comments
}")]
        [DataRow(0, @"
@secure() // invalid on an int
param param3 int
var sum = 1 + 2
output sub int = sum
")]
        [DataRow(0, @"
param param3 int
")]
        [DataRow(0, @"
@secure()
output sub int = sum
")]
        [DataTestMethod]
        public void HandlesSyntaxErrors(int diagnosticCount, string text)
        {
            AssertLinterRuleDiagnostics(SecureParameterRule.Code, text, diagnosticCount, OnCompileErrors.Ignore);
        }

    }
}
