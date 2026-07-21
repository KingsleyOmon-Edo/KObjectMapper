# Code review: KObjectMapper

## Scope and review method
This review covers the current .NET 8 solution under `src/KObjectMapper` and `tests/KObjectMapperTests`, including the runtime implementation, extension APIs, mapper surface area, and the current test project structure. The review focuses on correctness, API clarity, maintainability, and areas that would affect real-world usage.

## Validation summary
- Build status: successful
- Test status: 66 tests passed, 0 failed
- Overall impression: the library remains functional for simple, name-based property copying and the test suite covers the main happy paths. The core behavior is stable, but several correctness and API-design issues remain that can still surprise consumers or make the library harder to evolve.

## What is working well
- The solution remains compact and easy to navigate.
- The `Mapper` type and extension methods provide clear entry points for common mapping scenarios.
- The tests are organized by behavior and cover explicit, implicit, and characterization-style cases.
- The core mapping flow works well for straightforward property-to-property copying.

## Findings and recommendations

### 1. Collection mapping overloads still do not honor the supplied target collection
Severity: High

Files:
- `src/KObjectMapper/Mapper.cs`
- `src/KObjectMapper/Extensions/MapperExtensions.cs`

Issue:
The collection overloads that accept an existing target collection still create new target instances instead of populating the provided collection. This is misleading and violates expectations for overloads that appear to update existing state.

Impact:
- The API is semantically surprising.
- Consumers may believe they are updating an existing collection when they are actually creating a new one.

Recommendation:
- Either remove these overloads or implement them so they fill the supplied collection.
- If the intent is to create a new collection, rename the API or document it clearly.

### 2. Null handling for collection overloads remains inconsistent
Severity: High

Files:
- `src/KObjectMapper/Extensions/MapperExtensions.cs`

Issue:
The collection-based extension methods still enumerate input sequences before they consistently validate null input. If the source or target collection is null, callers can still hit a `NullReferenceException` instead of a clear `ArgumentNullException`.

Impact:
- Error handling is inconsistent with the rest of the API.
- Consumers receive less clear exceptions when supplying invalid input.

Recommendation:
- Validate the source and target enumerables before any enumeration.
- Prefer a small shared validation pattern for all collection-based entry points.

### 3. The mapper writes to any matching property name without checking writability
Severity: High

File:
- `src/KObjectMapper/MappingService.cs`

Issue:
`WriteToProperties` assigns values to every matching property name without checking whether the target property is writable. Read-only properties, properties with non-public setters, or properties that should be excluded from mapping will fail or behave unexpectedly at runtime.

Impact:
- The library is fragile in the presence of common property patterns such as read-only wrapper properties or non-public setters.
- Failures surface during execution rather than validation.

Recommendation:
- Filter candidates using `PropertyInfo.CanWrite` and setter availability before calling `SetValue`.
- Optionally skip or surface a warning for non-writable members.

### 4. The property-comparison API name and semantics are still misleading
Severity: Medium

File:
- `src/KObjectMapper/MappingService.cs`

Issue:
`ArePropValuesDifferent` returns `true` when the values are equal and `false` when they differ. The name suggests the opposite behavior and makes maintenance harder.

Impact:
- The method is easy to misuse in future maintenance work.
- The current implementation depends on an inverted interpretation that is not obvious from the name.

Recommendation:
- Rename the method to something like `ArePropValuesSame` or `ArePropValuesEqual`, or invert the implementation and update all call sites to match the name.

### 5. Type checks are still exact and too strict for polymorphic usage
Severity: Medium

Files:
- `src/KObjectMapper/Helpers/Checker.cs`
- `src/KObjectMapper/MappingService.cs`

Issue:
The current validation compares runtime types with `typeof(T)` exactly, so valid assignable types such as derived classes or interface-typed objects are rejected even when they should be acceptable for the operation.

Impact:
- The API is more restrictive than it needs to be.
- Consumers may hit runtime `ArgumentException` failures for valid polymorphic use cases.

Recommendation:
- Use assignability checks such as `typeof(T).IsAssignableFrom(instance.GetType())` where the semantics allow it.
- Document any intentional strictness if exact-type matching is still required.

### 6. Reflection is still repeated on every mapping operation
Severity: Medium

Files:
- `src/KObjectMapper/MappingService.cs`
- `src/KObjectMapper/Mapper.cs`

Issue:
Property metadata is still resolved repeatedly during mapping and the diffing logic uses a nested loop that is effectively O(n^2) for larger sets of properties. This is acceptable for small objects, but it will scale poorly as usage grows.

Impact:
- Performance will degrade for larger object graphs or high-frequency mapping.
- The implementation is more complex than necessary for a library that could reasonably be expected to operate on many objects.

Recommendation:
- Introduce a cached mapping plan or property-metadata cache per type so that repeated mappings avoid repeated reflection and repeated work.

### 7. The codebase still contains overlapping implementation variants
Severity: Medium

Files:
- `src/KObjectMapper/InstanceService.cs`
- `src/KObjectMapper/Extensions/Extras/ObjectExtensionsX.cs`

Issue:
The repository still contains alternate or experimental implementations that duplicate much of the same logic as `MappingService` and `Mapper`. These variants increase maintenance risk and make it harder to know which implementation is authoritative.

Impact:
- The library surface is harder to reason about.
- Changes may need to be applied in several places to keep behavior consistent.

Recommendation:
- Consolidate the active implementation into a single path, remove or archive the duplicate variants, and leave only the supported API surface.

### 8. The public mapping model remains narrow
Severity: Medium

Files:
- `src/KObjectMapper/MappingService.cs`
- `src/KObjectMapper/Mapper.cs`

Issue:
The current library only supports simple, name-based mapping of public, mutable properties. It does not support nested mapping, custom value transformations, or richer conventions without additional code.

Impact:
- The library is suitable for basic scenarios but may feel limiting in more complex object graphs.

Recommendation:
- If the library is intended to remain lightweight, document these restrictions clearly.
- If broader use is desired, consider a plan for custom resolvers, nested mapping, and configurable ignore/include rules.

### 9. Validation logic is still spread across helper classes and call sites
Severity: Low

Files:
- `src/KObjectMapper/Helpers/Checker.cs`
- `src/KObjectMapper/Mapper.cs`
- `src/KObjectMapper/Extensions/MapperExtensions.cs`

Issue:
Guard and validation logic is still distributed across a helper class and several entry points. This adds indirection and makes it easier for behavior to drift between overloads.

Impact:
- The code is slightly harder to follow and maintain.
- New overloads are more likely to introduce inconsistent validation.

Recommendation:
- Continue consolidating validation into the methods that use it, or at least centralize the shared checks in a small, focused helper rather than a general-purpose checker.

## Updated note
Some earlier recommendations, such as simplifying the validation path inside `MappingService`, have been partially addressed, but the remaining findings above still represent the most valuable follow-up work for the library.

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
