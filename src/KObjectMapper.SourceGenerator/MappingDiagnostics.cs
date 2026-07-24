using Microsoft.CodeAnalysis;

namespace KObjectMapper.SourceGenerator;

/// <summary>
/// Diagnostic descriptors emitted by the KObjectMapper source generator.
/// </summary>
public static class MappingDiagnostics
{
    private const string Category = "KObjectMapper";

    /// <summary>
    /// KOM001: Emitted when a source property has no matching writable property on the target,
    /// or when a type mismatch prevents direct assignment.
    /// </summary>
    public static readonly DiagnosticDescriptor UnsupportedMemberPattern = new DiagnosticDescriptor(
        id: "KOM001",
        title: "Unsupported member mapping pattern",
        messageFormat: "{0}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "The source generator identified a member that cannot be directly mapped. " +
                     "Consider adding a type converter or an explicit ForMember configuration.");
}
