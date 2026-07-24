using Microsoft.CodeAnalysis;

namespace KObjectMapper.SourceGenerator;

internal static class MappingDiagnostics
{
    private const string Category = "KObjectMapper";

    public static readonly DiagnosticDescriptor MissingTargetProperty = new(
        id: "KOM001",
        title: "Missing target property",
        messageFormat: "Source property '{0}' on '{1}' has no matching target property on '{2}' and will not be mapped",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor TypeMismatch = new(
        id: "KOM002",
        title: "Type mismatch",
        messageFormat: "Member '{0}': source type '{1}' is not assignable to target type '{2}' and will not be mapped",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
