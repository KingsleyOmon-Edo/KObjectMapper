# Code review: KObjectMapper

## Scope and review method
This review covers the full solution under `src/KObjectMapper` and `tests/KObjectMapperTests`, including the runtime implementation, extension APIs, test coverage, and project configuration for the current .NET 6 solution.

## Validation summary
- Build status: successful
- Test status: 64 tests passed, 0 failed
- Overall impression: the library is functional for simple, name-based property mapping and the test suite covers the main happy paths, but the implementation has a few correctness issues and several areas where the API design is likely to surprise consumers.

## What is working well
- The solution is small, focused, and easy to navigate.
- The public API is straightforward: `Mapper` and extension methods provide explicit and implicit mapping entry points.
- The test project is well organized into behavior-focused test groups (`ExplicitMapping`, `ImplicitMapping`, `CharacterizationTests`).
- The current implementation handles straightforward property-to-property copying for simple objects well.

## Findings and recommendations

### 1. Collection mapping overloads do not behave as their signatures suggest
Severity: High

Files:
- `src/KObjectMapper/Mapper.cs`
- `src/KObjectMapper/Extensions/MapperExtensions.cs`

Issue:
The collection-based overloads accept an existing `target` collection, but the implementation ignores it in practice and instead creates new target instances. This makes the API misleading and can lead to unexpected behavior for callers who expect the supplied target collection to be populated or updated.

Examples:
- `Mapper.Map<TSource, TTarget>(IEnumerable<TSource> source, IEnumerable<TTarget> target)` creates a new `List<TTarget>` and never uses the `target` argument.
- `MapperExtensions.MapFrom<TSource, TTarget>(this IEnumerable<TTarget> target, IEnumerable<TSource> source)` similarly creates new instances rather than mapping into the provided target collection.

Impact:
- The overload is semantically incorrect.
- Consumers may believe they are updating an existing collection when they are actually generating a new one.
- This is a likely source of subtle bugs in real applications.

Recommendation:
- Either remove the collection overloads that take an existing target collection, or implement them correctly by mapping into the provided collection.
- If the intent is to generate a new collection, rename the API or document it more clearly to avoid confusion.

### 2. Null handling is inconsistent and can throw the wrong exception type
Severity: High

Files:
- `src/KObjectMapper/Extensions/MapperExtensions.cs`
- `src/KObjectMapper/Helpers/Checker.cs`

Issue:
Several extension methods call `.ToArray()` on supplied enumerables before doing any null checks. If the source or target collection is null, this can surface as `NullReferenceException` instead of the expected `ArgumentNullException`.

Examples:
- `MapperExtensions.MapFrom<TSource, TTarget>(this IEnumerable<TTarget> target, IEnumerable<TSource> source)`
- `MapperExtensions.MapTo<TSource, TTarget>(this IEnumerable<TSource> source, IEnumerable<TTarget> target)`

Impact:
- Error handling is inconsistent with the rest of the API.
- Consumers may see less clear exceptions when supplying null collections.

Recommendation:
- Perform null checks before enumeration.
- For example, validate `source` and `target` first, then enumerate them.
- Prefer a single, consistent validation helper for collection overloads.

### 3. Mapping does not guard against non-writable target properties
Severity: High

File:
- `src/KObjectMapper/MappingService.cs`

Issue:
`WriteToProperties` copies values to every matching property name without checking whether the target property is writable. Read-only properties, properties with private setters, or properties that should be excluded from mapping will fail or behave unexpectedly when `SetValue` is called.

Impact:
- The library is fragile in the presence of common property patterns such as read-only wrapper properties or non-public setters.
- Errors will surface at runtime rather than during validation.

Recommendation:
- Filter properties using `PropertyInfo.CanWrite` and the setter presence before assigning values.
- Consider skipping properties that are not settable and optionally logging or surfacing a warning.
- This would make the mapper safer and easier to reason about.

### 4. The comparison logic has misleading semantics and a confusing name
Severity: Medium

File:
- `src/KObjectMapper/MappingService.cs`

Issue:
`MappingService.ArePropValuesDifferent` returns `true` when the values are equal and `false` when they differ. The name suggests the opposite behavior. This makes the code harder to read and easier to misuse.

Impact:
- The method is easy to misuse in future maintenance work.
- The current implementation works only because the surrounding logic relies on this inverted behavior implicitly.

Recommendation:
- Rename the method to something like `ArePropValuesSame` or `ArePropValuesEqual` and make the semantics match the name.
- Or, if the purpose is to detect differences, reverse the implementation and update all call sites accordingly.

### 5. Type checks are overly strict and do not behave like standard assignability checks
Severity: Medium

File:
- `src/KObjectMapper/Helpers/Checker.cs`

Issue:
`Checker.TypeCheck<T>` compares the runtime type of the object to `typeof(T)` exactly. That means assignable types such as derived classes or interface-typed objects can be rejected even when they are valid for the operation.

Impact:
- The API is more restrictive than it needs to be.
- Consumers may hit runtime `ArgumentException`s for valid polymorphic usage.

Recommendation:
- Replace exact type comparison with an assignability check such as `typeof(T).IsAssignableFrom(testObj.GetType())` when appropriate.
- Review whether the current behavior is intentional; if it is, document it clearly.

### 6. Reflection is used heavily and repeated on every mapping operation
Severity: Medium

Files:
- `src/KObjectMapper/MappingService.cs`
- `src/KObjectMapper/Mapper.cs`

Issue:
The mapper repeatedly reflects over properties and re-evaluates metadata during each mapping. The property-diff work also loops over source and target properties with O(n^2) behavior in the worst case.

Impact:
- Performance will degrade for larger object graphs or high-frequency mapping.
- The implementation is more complex than necessary for a library that could reasonably be expected to operate on many objects.

Recommendation:
- Cache property metadata per type.
- Consider using a strategy that resolves a type’s mapping plan once and reuses it for subsequent mappings.
- This would also improve clarity and make it easier to add features later.

### 7. The library supports only a very narrow mapping model
Severity: Medium

Files:
- `src/KObjectMapper/MappingService.cs`
- `src/KObjectMapper/Mapper.cs`

Issue:
The current implementation is limited to simple property name-based mapping of public, writable properties. It does not support:
- nested object mapping
- collection element mapping
- custom converters or mapping functions
- property-level configuration
- nullable/compatible type conversions beyond the current `Convert.ChangeType` approach

Impact:
- The library may be fine for very simple use cases, but it will be less useful for real-world object graphs.
- The current API does not communicate these limitations clearly enough.

Recommendation:
- Document the supported scenarios and explicitly call out the limitations.
- Consider introducing a basic extension point (for example, a delegate or interface-based mapping strategy) before expanding capabilities.

### 8. The single-object creation overload is not protected against missing parameterless constructors
Severity: Medium

File:
- `src/KObjectMapper/Mapper.cs`

Issue:
`Mapper.Map<TSource, TTarget>(TSource source)` and `Mapper.Map<TSource, TTarget>(IEnumerable<TSource> sources)` create new target instances with `Activator.CreateInstance<TTarget>()` but the generic method signature does not constrain `TTarget` to types with a parameterless constructor.

Impact:
- Consumers will hit runtime exceptions for target types without a public parameterless constructor.
- The API is less discoverable because the contract is not enforced by the type system.

Recommendation:
- Add `where TTarget : new()` to the single-object and collection creation overloads if parameterless construction is required.
- Or, if non-default construction is planned for the future, update the API to make this explicit and validate it earlier.

### 9. The test suite covers the primary happy paths but misses important edge cases
Severity: Medium

Files:
- `tests/KObjectMapperTests/**/*.cs`

Issue:
The current tests focus on successful mapping of simple objects and common null cases. They do not cover:
- mapping into an existing collection
- read-only or non-writable properties
- null values on source or target properties
- nested object mapping
- incompatible but potentially convertible property types
- scenarios where the target type lacks a parameterless constructor

Impact:
- The implementation can regress in ways that are not currently guarded by tests.
- The current suite gives confidence for the happy path, but not for the library’s more failure-prone areas.

Recommendation:
- Add targeted tests for the cases above.
- Consider using a mix of unit tests and a small set of integration-style tests that exercise realistic object graphs.

### 10. Packaging configuration could be made more CI-friendly
Severity: Low

File:
- `src/KObjectMapper/KObjectMapper.csproj`

Issue:
The project is configured to generate a package on every build (`<GeneratePackageOnBuild>True</GeneratePackageOnBuild>`), and the package version uses a timestamp-based pattern. This is convenient for local experimentation, but it can be noisy and less deterministic in CI or release pipelines.

Impact:
- Builds can produce many artifact variants.
- Versioning is less predictable and less suitable for release automation.

Recommendation:
- Use `dotnet pack` in CI explicitly instead of generating packages on every build.
- Use deterministic versioning for release builds and reserve timestamped versions for local development if needed.

## Suggested priority order
1. Fix the collection overload semantics and null-check ordering.
2. Add writable-property guards and make the mapping pipeline resilient to common property patterns.
3. Add missing tests around edge cases and unsupported scenarios.
4. Introduce a caching or planning step for property metadata to improve maintainability and performance.
5. Improve package/versioning behavior for CI and release scenarios.

## Bottom line
The solution is a solid foundation for a small, simple object-mapper, and the current tests confirm that the basic behavior works. The main opportunities are to make the implementation safer, reduce unexpected runtime behavior, and clarify the API so that it better matches the way consumers will likely use it.
