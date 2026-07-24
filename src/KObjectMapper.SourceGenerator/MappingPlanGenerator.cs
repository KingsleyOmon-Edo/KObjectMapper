using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace KObjectMapper.SourceGenerator;

/// <summary>
/// Incremental source generator that produces compile-time mapping implementations
/// for registered source/target pairs declared via CreateMap&lt;TSource, TTarget&gt;() calls
/// inside MappingProfile subclasses.
/// </summary>
[Generator]
public sealed class MappingPlanGenerator : IIncrementalGenerator
{
    internal const string MappingProfileBaseClass = "MappingProfile";
    internal const string CreateMapMethodName = "CreateMap";
    internal const string GeneratedNamespace = "KObjectMapper.Generated";
    internal const string GeneratedClassSuffix = "Mapper";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Collect all class declarations that inherit from MappingProfile
        IncrementalValuesProvider<MappingPlanDescriptor> mappingPlans = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsMappingProfileSubclass(node),
                transform: static (ctx, _) => ExtractMappingPlans(ctx))
            .Where(static descriptor => descriptor is not null)
            .Select(static (descriptor, _) => descriptor!);

        context.RegisterSourceOutput(mappingPlans, static (spc, descriptor) =>
            GenerateMappingSource(spc, descriptor));
    }

    private static bool IsMappingProfileSubclass(SyntaxNode node)
    {
        if (node is not ClassDeclarationSyntax classDecl)
            return false;

        return classDecl.BaseList?.Types.Count > 0;
    }

    private static MappingPlanDescriptor? ExtractMappingPlans(GeneratorSyntaxContext context)
    {
        ClassDeclarationSyntax classDecl = (ClassDeclarationSyntax)context.Node;

        INamedTypeSymbol? classSymbol = context.SemanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
        if (classSymbol is null)
            return null;

        // Check if this class inherits from MappingProfile (directly or indirectly)
        if (!InheritsFromMappingProfile(classSymbol))
            return null;

        List<MappingTypePair> typePairs = new List<MappingTypePair>();

        // Find all CreateMap<TSource, TTarget>() invocations in the Configure method
        foreach (MethodDeclarationSyntax method in classDecl.Members.OfType<MethodDeclarationSyntax>())
        {
            if (method.Identifier.Text != "Configure")
                continue;

            foreach (InvocationExpressionSyntax invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                // Handle both bare CreateMap<S,T>() and this.CreateMap<S,T>() / chained calls
                GenericNameSyntax? genericName = invocation.Expression switch
                {
                    GenericNameSyntax gn => gn,
                    MemberAccessExpressionSyntax { Name: GenericNameSyntax mgn } => mgn,
                    _ => null
                };

                if (genericName is null)
                    continue;

                if (genericName.Identifier.Text != CreateMapMethodName)
                    continue;

                if (genericName.TypeArgumentList.Arguments.Count != 2)
                    continue;

                TypeSyntax sourceTypeSyntax = genericName.TypeArgumentList.Arguments[0];
                TypeSyntax targetTypeSyntax = genericName.TypeArgumentList.Arguments[1];

                ITypeSymbol? sourceType = context.SemanticModel.GetTypeInfo(sourceTypeSyntax).Type;
                ITypeSymbol? targetType = context.SemanticModel.GetTypeInfo(targetTypeSyntax).Type;

                if (sourceType is null || targetType is null)
                    continue;

                // Validate that both types have accessible public properties
                List<string> diagnosticMessages = new List<string>();
                List<PropertyMapping> propertyMappings = ResolvePropertyMappings(sourceType, targetType, diagnosticMessages);

                typePairs.Add(new MappingTypePair(
                    SourceType: sourceType,
                    TargetType: targetType,
                    PropertyMappings: propertyMappings,
                    DiagnosticMessages: diagnosticMessages));
            }
        }

        if (typePairs.Count == 0)
            return null;

        return new MappingPlanDescriptor(
            ProfileClassName: classSymbol.Name,
            ProfileNamespace: classSymbol.ContainingNamespace?.ToDisplayString() ?? string.Empty,
            TypePairs: typePairs);
    }

    private static bool InheritsFromMappingProfile(INamedTypeSymbol classSymbol)
    {
        INamedTypeSymbol? baseType = classSymbol.BaseType;
        while (baseType is not null)
        {
            if (baseType.Name == MappingProfileBaseClass)
                return true;
            baseType = baseType.BaseType;
        }
        return false;
    }

    private static List<PropertyMapping> ResolvePropertyMappings(
        ITypeSymbol sourceType,
        ITypeSymbol targetType,
        List<string> diagnosticMessages)
    {
        List<PropertyMapping> mappings = new List<PropertyMapping>();

        IEnumerable<IPropertySymbol> sourceProps = GetPublicInstanceProperties(sourceType);
        IEnumerable<IPropertySymbol> targetProps = GetPublicInstanceProperties(targetType);

        Dictionary<string, IPropertySymbol> targetPropMap = new Dictionary<string, IPropertySymbol>();
        foreach (IPropertySymbol prop in targetProps)
        {
            if (prop.SetMethod is not null && prop.SetMethod.DeclaredAccessibility == Accessibility.Public)
                targetPropMap[prop.Name] = prop;
        }

        foreach (IPropertySymbol sourceProp in sourceProps)
        {
            if (!targetPropMap.TryGetValue(sourceProp.Name, out IPropertySymbol? targetProp))
            {
                diagnosticMessages.Add(
                    $"Source property '{sourceType.Name}.{sourceProp.Name}' has no matching writable property on '{targetType.Name}'.");
                continue;
            }

            if (!SymbolEqualityComparer.Default.Equals(sourceProp.Type, targetProp.Type))
            {
                diagnosticMessages.Add(
                    $"Type mismatch: '{sourceType.Name}.{sourceProp.Name}' ({sourceProp.Type}) cannot be directly assigned to '{targetType.Name}.{targetProp.Name}' ({targetProp.Type}). A type converter may be required.");
                continue;
            }

            mappings.Add(new PropertyMapping(sourceProp.Name, targetProp.Name, sourceProp.Type.ToDisplayString()));
        }

        return mappings;
    }

    private static IEnumerable<IPropertySymbol> GetPublicInstanceProperties(ITypeSymbol type)
    {
        List<IPropertySymbol> props = new List<IPropertySymbol>();
        ITypeSymbol? current = type;
        while (current is not null && current.SpecialType != SpecialType.System_Object)
        {
            foreach (ISymbol member in current.GetMembers())
            {
                if (member is IPropertySymbol prop
                    && prop.DeclaredAccessibility == Accessibility.Public
                    && !prop.IsStatic
                    && !prop.IsIndexer
                    && prop.GetMethod is not null)
                {
                    props.Add(prop);
                }
            }
            current = current.BaseType;
        }
        return props;
    }

    private static void GenerateMappingSource(SourceProductionContext context, MappingPlanDescriptor descriptor)
    {
        foreach (MappingTypePair pair in descriptor.TypePairs)
        {
            // Emit diagnostics for unsupported patterns
            foreach (string message in pair.DiagnosticMessages)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    MappingDiagnostics.UnsupportedMemberPattern,
                    Location.None,
                    message));
            }

            if (pair.PropertyMappings.Count == 0)
                continue;

            string sourceFullName = pair.SourceType.ToDisplayString();
            string targetFullName = pair.TargetType.ToDisplayString();
            string sourceName = pair.SourceType.Name;
            string targetName = pair.TargetType.Name;
            string generatedClassName = $"{sourceName}To{targetName}{GeneratedClassSuffix}";

            // Use '\n' (LF) consistently to avoid CRLF/LF mismatches across platforms
            StringBuilder sb = new StringBuilder();
            sb.Append("// <auto-generated/>").Append('\n');
            sb.Append("// Generated by KObjectMapper.SourceGenerator \u2014 do not edit manually.").Append('\n');
            sb.Append('\n');
            sb.Append($"namespace {GeneratedNamespace};").Append('\n');
            sb.Append('\n');
            sb.Append("/// <summary>").Append('\n');
            sb.Append($"/// Compile-time generated mapper from <see cref=\"{sourceFullName}\"/> to <see cref=\"{targetFullName}\"/>.").Append('\n');
            sb.Append("/// </summary>").Append('\n');
            sb.Append($"public static class {generatedClassName}").Append('\n');
            sb.Append('{').Append('\n');
            sb.Append($"    /// <summary>Maps all compatible properties from <paramref name=\"source\"/> into <paramref name=\"target\"/>.</summary>").Append('\n');
            sb.Append($"    public static void Map({sourceFullName} source, {targetFullName} target)").Append('\n');
            sb.Append("    {").Append('\n');
            sb.Append("        if (source is null) throw new System.ArgumentNullException(nameof(source));").Append('\n');
            sb.Append("        if (target is null) throw new System.ArgumentNullException(nameof(target));").Append('\n');
            sb.Append('\n');

            foreach (PropertyMapping mapping in pair.PropertyMappings)
            {
                sb.Append($"        target.{mapping.TargetPropertyName} = source.{mapping.SourcePropertyName};").Append('\n');
            }

            sb.Append("    }").Append('\n');
            sb.Append('\n');
            sb.Append($"    /// <summary>Creates a new <see cref=\"{targetFullName}\"/> and maps all compatible properties from <paramref name=\"source\"/>.</summary>").Append('\n');
            sb.Append($"    public static {targetFullName} Map({sourceFullName} source)").Append('\n');
            sb.Append("    {").Append('\n');
            sb.Append($"        {targetFullName} target = new();").Append('\n');
            sb.Append("        Map(source, target);").Append('\n');
            sb.Append("        return target;").Append('\n');
            sb.Append("    }").Append('\n');
            sb.Append('}').Append('\n');

            string hintName = $"{generatedClassName}.g.cs";
            context.AddSource(hintName, SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }
}
