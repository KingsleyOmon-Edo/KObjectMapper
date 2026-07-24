# Generator Debugging

## Viewing Generated Files

By default, source-generated files are written only to an in-memory compilation and are not persisted to disk. To inspect the generated output, add the following properties to your project file:

```xml
<PropertyGroup>
  <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
  <CompilerGeneratedFilesOutputPath>$(BaseIntermediateOutputPath)Generated</CompilerGeneratedFilesOutputPath>
</PropertyGroup>
```

After adding these properties, run `dotnet build`. Generated files will appear under:

```
obj/Generated/KObjectMapper.SourceGenerator/KObjectMapper.SourceGenerator.MappingPlanGenerator/
```

Each generated file is named `{Source}To{Target}Mapper.g.cs`.

## Inspecting Diagnostics

### In the IDE

Diagnostics emitted by the generator (`KOM001`, `KOM002`) appear in the **Errors and Warnings** tool window alongside regular compiler diagnostics. No additional setup is required.

### Via dotnet build

Run:

```bash
dotnet build
```

Warnings from the generator are printed to the console with their diagnostic ID, message, and severity. To treat generator warnings as errors, add:

```xml
<PropertyGroup>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
</PropertyGroup>
```

## Diagnostic Reference

| ID | Severity | Meaning |
|----|----------|---------|
| `KOM001` | Warning | A source property has no matching property on the target type and will not be mapped. |
| `KOM002` | Warning | A property exists on both types but the source type is not assignable to the target type and will not be mapped. |
