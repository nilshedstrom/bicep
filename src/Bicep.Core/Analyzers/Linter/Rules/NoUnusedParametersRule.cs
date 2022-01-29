// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoUnusedParametersRule : NoUnusedRuleBase
    {
        public new const string Code = "no-unused-params";
        public NoUnusedParametersRule() : base(
            "parameter",
            code: Code,
            description: CoreResources.ParameterMustBeUsedRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticLabel: Diagnostics.DiagnosticLabel.Unnecessary)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.ParameterMustBeUsedRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            // parameters must have at least two references
            //  1) One reference will be the the paramater syntax declaration
            //  2) VariableAccessSyntax indicates a reference to the parameter
            var unreferencedParams = model.Root.ParameterDeclarations
                                    .Where(sym => !model.FindReferences(sym).OfType<VariableAccessSyntax>().Any());

            return unreferencedParams.Select(param => GetFixableDiagnosticForSpan(param.Name, param.NameSyntax, param.DeclaringSyntax, model.SourceFile.LineStarts));
        }
    }
}
