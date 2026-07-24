namespace KObjectMapper.SourceGenerator;

internal sealed class MappingPair
{
    public MappingPair(string sourceTypeName, string targetTypeName, string @namespace)
    {
        SourceTypeName = sourceTypeName;
        TargetTypeName = targetTypeName;
        Namespace = @namespace;
    }

    public string SourceTypeName { get; }
    public string TargetTypeName { get; }
    public string Namespace { get; }
}

internal sealed class MappableProperty
{
    public MappableProperty(string name, string typeName)
    {
        Name = name;
        TypeName = typeName;
    }

    public string Name { get; }
    public string TypeName { get; }
}
