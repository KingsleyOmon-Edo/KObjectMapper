using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace KObjectMapper.SourceGenerator;

/// <summary>Describes all mapping plans extracted from a single MappingProfile subclass.</summary>
internal sealed record MappingPlanDescriptor(
    string ProfileClassName,
    string ProfileNamespace,
    List<MappingTypePair> TypePairs);

/// <summary>Describes a single source-to-target type pair with resolved property mappings.</summary>
internal sealed record MappingTypePair(
    ITypeSymbol SourceType,
    ITypeSymbol TargetType,
    List<PropertyMapping> PropertyMappings,
    List<string> DiagnosticMessages);

/// <summary>Describes a single property-level mapping between source and target.</summary>
internal sealed record PropertyMapping(
    string SourcePropertyName,
    string TargetPropertyName,
    string PropertyTypeName);
