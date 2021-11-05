// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;

namespace Bicep.Core.CodeAnalysis
{
    public class ResourceNameOperation : Operation
    {
        public ResourceNameOperation(ResourceMetadata metadata, IndexReplacementContext? indexContext)
        {
            Metadata = metadata;
            IndexContext = indexContext;
        }

        public ResourceMetadata Metadata { get; }

        public IndexReplacementContext? IndexContext { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitResourceNameOperation(this);
    }
}
