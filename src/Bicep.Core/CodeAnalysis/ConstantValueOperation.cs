// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.CodeAnalysis
{
    public class ConstantValueOperation : Operation
    {
        public ConstantValueOperation(object value)
        {
            Value = value;
        }

        public object Value { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitConstantValueOperation(this);
    }
}
