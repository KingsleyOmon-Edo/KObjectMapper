// Copyright (c) KObjectMapper contributors. All rights reserved.
namespace KObjectMapper.Configuration;

public sealed class GraphMappingOptions
{
    public bool PreserveReferences { get; private set; }
    public int MaxDepth { get; private set; } = 64;

    public GraphMappingOptions WithReferencePreservation()
    {
        PreserveReferences = true;
        return this;
    }

    public GraphMappingOptions WithMaxDepth(int maxDepth)
    {
        MaxDepth = maxDepth;
        return this;
    }
}
