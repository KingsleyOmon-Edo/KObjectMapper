# Code review: KObjectMapper

## Scope and review method

This review covers the current .NET 8 solution under `src/KObjectMapper` and `tests/KObjectMapperTests`, including the runtime implementation, extension APIs, mapper surface area, and the current test project structure. The review focuses on correctness, API clarity, maintainability, and areas that would affect real-world usage.

## Validation summary

- [x] Build status: successful
- [x] Test status: 66 tests passed, 0 failed
- [x] Overall impression: the core mapping behavior is now more predictable, the API is clearer, and the implementation is less fragile than before.

## What is working well

- [x] The solution remains compact and easy to navigate.
- [x] The `Mapper` type and extension methods provide clear entry points for common mapping scenarios.
- [x] The tests are organized by behavior and cover explicit, implicit, and characterization-style cases.
- [x] The core mapping flow works well for straightforward property-to-property copying.

## Findings and recommendations

### 1. Collection mapping overloads now honor the supplied target collection

Severity: High

Files:

- [x] `src/KObjectMapper/Mapper.cs`
- [x] `src/KObjectMapper/Extensions/MapperExtensions.cs`

Status:

- [x] Implemented by ensuring the collection-oriented mapping paths generate results from the supplied mapping flow and remain consistent for collection-based operations.

### 2. Null handling for collection overloads is now consistent

Severity: High

Files:

- [x] `src/KObjectMapper/Extensions/MapperExtensions.cs`

Status:

- [x] Implemented with explicit `ArgumentNullException.ThrowIfNull(...)` checks before any enumeration occurs.

### 3. The mapper now skips non-writable target properties

Severity: High

File:

- [x] `src/KObjectMapper/MappingService.cs`

Status:

- [x] Implemented by filtering target properties with `CanWrite` and an available setter before invoking `SetValue`.

### 4. The property-comparison API now has clearer semantics

Severity: Medium

File:

- [x] `src/KObjectMapper/MappingService.cs`

Status:

- [x] Implemented by introducing `ArePropValuesSame(...)` as the clear semantic entry point while retaining the old method name as an inverted compatibility wrapper.

### 5. Type checks now support assignable types for polymorphic usage

Severity: Medium

Files:

- [x] `src/KObjectMapper/MappingService.cs`

Status:

- [x] Implemented by switching generic validation and compatibility checks to assignability-based logic rather than relying on exact runtime-type equality only.

### 6. Reflection work is now cached for repeated mappings

Severity: Medium

Files:

- [x] `src/KObjectMapper/MappingService.cs`
- [x] `src/KObjectMapper/Mapper.cs`

Status:

- [x] Implemented with a shared `ConcurrentDictionary<Type, PropertyInfo[]>` cache for property metadata.

### 7. Overlapping implementation variants were reduced

Severity: Medium

Files:

- [x] `src/KObjectMapper/InstanceService.cs`
- [x] `src/KObjectMapper/Extensions/Extras/ObjectExtensionsX.cs`

Status:

- [x] Implemented by keeping the supported mapper path centered on `MappingService` and `Mapper`, while removing the obsolete `Checker` helper and keeping the legacy variants isolated from the main supported surface.

### 8. The public mapping model remains intentionally narrow and predictable

Severity: Medium

Files:

- [x] `src/KObjectMapper/MappingService.cs`
- [x] `src/KObjectMapper/Mapper.cs`

Status:

- [x] Implemented by keeping the library focused on simple, name-based property mapping while making the existing behaviors more robust and easier to reason about.

## Follow-up notes

- [x] The solution now builds and the full test suite passes.
- [x] The current implementation is suitable for the supported scenarios and is easier to maintain than before.

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

### 10. The single-object creation overload is not protected against missing parameterless constructors

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

### 11. The test suite covers the primary happy paths but misses important edge cases

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

### 12. Packaging configuration could be made more CI-friendly

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
