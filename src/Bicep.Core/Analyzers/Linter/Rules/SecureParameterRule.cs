// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class SecureParameterRule : LinterRuleBase
    {
        public new const string Code = "secure-parameter";

        public SecureParameterRule() : base(
            code: Code,
            description: CoreResources.AddSecureDecorator,
            diagnosticLevel: DiagnosticLevel.Info
            )
        { }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var types = new[] { "string", "object" };
            var syntaxes = model.Root.ParameterDeclarations.Where(p => !p.IsSecure()
                && types.Any(t => p.DeclaringParameter.ParameterType?.TypeName == t));
            foreach (var syntax in syntaxes)
            {
                var span = new Parsing.TextSpan(syntax.DeclaringSyntax.Span.Position, 0);

                yield return CreateFixableDiagnosticForSpan(syntax.DeclaringSyntax.Span,
                    new CodeFix(CoreResources.AddSecureDecorator, false,
                            new CodeReplacement(span, $"@secure(){System.Environment.NewLine}")));
            }
        }
    }
}
