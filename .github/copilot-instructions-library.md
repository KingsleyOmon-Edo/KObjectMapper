---
description: 'Guidelines for building C# NuGet class library projects on .NET 6+'
applyTo: >
  src/**/*.cs,
  tests/**/*.cs,
  **/*.csproj,
  **/*.sln,
  **/.github/workflows/*.yml,
  **/.github/workflows/*.yaml,
  README.md,
  CHANGELOG.md
---

# C# NuGet Library - Language & Design Constraints

## Language & Modern C# Features

- Target **.NET 6** and later using **C# features** appropriate to the minimum supported version.
- Prefer nullable reference types (`#nullable enable`) to catch nullability issues at compile-time.
- Use pattern matching, recursive patterns, and switch expressions for readability.
- Always apply `nameof()` when logging or raising exceptions to avoid hardcoded strings.

## Code Formatting Rules

- Enforce formatting profiles strictly conforming to the project's root `.editorconfig` specification.
- Mandate **file-scoped namespaces** and alphabetical, single-line using statements inside all `.cs` documents.
- **Curly Brace Policy:** Apply Allman style formatting: place opening curly braces on their own newline immediately preceding the code block (applies to classes, methods, `if`, `switch`, loops, and `using` scopes).
- Maintain distinct breathing room: ensure that final `return`, `throw`, or `yield` statements are separated from preceding logical mutations by a single blank newline.
- **Explicit Typing:** Prefer explicit types over `var` in all declarations. Only use `var` when the type is immediately evident from the right-hand side (e.g., `var mapper = new Mapper()`) — never when the type requires the reader to mentally resolve it.

## Architectural Naming & Structure Conventions

- **PascalCase:** Apply strictly to Class definitions, Record schemes, Enum values, Method signatures, and Public members.
- **camelCase:** Apply to local variable scopes, method parameters, and lambda modifiers.
- **Private State Fields:** Prefix private fields exclusively with a single underscore (e.g., `_cache`). Do not prefix local parameters or properties.
- **Interface Naming:** Prepend an uppercase `I` to all public interfaces (e.g., `IObjectMapper`), ensuring they reflect descriptive behavior capabilities.
- **Type Visibility:** Declare all types `internal` and `sealed` by default. Only expose `public` types when they are part of the public library API. Only remove `sealed` when inheritance is explicitly required by design. This prevents accidental API surface growth and maintains clear public contracts.
- **Public API Stability:** Once a type or method is made `public`, it becomes part of the semantic versioning contract. Changes to public signatures require major version bumps. Document public APIs clearly and consider deprecation carefully.

## Documentation & Code Comments Policy

- **Self-Documenting Code First:** Prioritize clear, expressive naming strategies over verbose inline narratives.
- **The "Why" Rule:** Restrict inline comments exclusively to explaining complex, non-obvious logic or design decisions. Never use comments to explain basic framework operations or language features.
- **Algorithm & Complexity Metrics:** For performance-critical paths, document Big O metrics (e.g., `// Time: O(n log n) | Space: O(n)`).
- **Public API Documentation:** Require structural XML triple-slash (`///`) summaries on all public types and members. Utilize `<summary>`, `<param>`, `<returns>`, and `<exception>` markup tags cleanly.

## Project Setup and Structure

- Organize code into a simple structure: `src/` for library code and `tests/` for test projects.
- Keep the API surface minimal and intentional. Every public type should have a clear purpose.
- Group related public types in namespaces that reflect their domain (e.g., `KObjectMapper.Extensions`).
- Prefer composition over inheritance; use `sealed` by default to prevent accidental subclassing.
- Document all public APIs with XML triple-slash comments (`///`) including `<summary>`, `<param>`, `<returns>`, and `<exception>` tags.

## Nullable Reference Types

- Declare variables non-nullable, and check for `null` at entry points.
- Always use `is null` or `is not null` instead of `== null` or `!= null`.
- Trust the C# null annotations and don't add null checks when the type system says a value cannot be null.

## API Design Principles

### 1. Minimal Public Surface
- Only expose types and methods that are essential for the library's purpose.
- Mark everything else as `internal` or `private`.
- Use explicit namespaces to organize the public API logically.

### 2. Clear Method Contracts
- Method signatures should be self-documenting.
- Use expressive parameter names.
- Return values should be predictable and consistent.
- Document exceptions that can be thrown.

### 3. Backward Compatibility
- Never remove public types, methods, or properties without a major version bump.
- When changing public API behavior, consider deprecation attributes first.
- Use judicious extension methods to add functionality without breaking changes.

### 4. Validation at Entry Points
- Validate all public method parameters.
- Use `ArgumentNullException`, `ArgumentException`, etc., consistently.
- Provide clear error messages that help consumers fix the issue.

```csharp
public void DoSomething(object source, object target)
{
	source = source ?? throw new ArgumentNullException(nameof(source));
	target = target ?? throw new ArgumentNullException(nameof(target));

	// Implementation
}
```

## Testing Standards

- Maintain test coverage of the public API.
- Use **xUnit** with fluent assertions (e.g., **FluentAssertions** or **Shouldly**).
- Use the **Red-Green-Refactor** cycle: write failing tests first, implement code to pass, refactor for clarity.
- Test happy paths, edge cases, and error conditions.
- Name tests using the `MethodUnderTest_Condition_ExpectedBehavior` pattern.

```csharp
[Fact]
public void MapTo_WhenSourceIsNull_ThrowsArgumentNullException()
{
	Object target = new Object();

	Assert.Throws<ArgumentNullException>(() => mapper.MapTo(null, target));
}
```

## Package Configuration

- Use semantic versioning: MAJOR.MINOR.PATCH (e.g., 1.0.0, 1.1.0, 2.0.0).
- Update `.csproj` with clear authored, description, and repository metadata.
- Document breaking changes in the CHANGELOG.
- Include clear README with examples.

## Performance Considerations

- Prioritize correctness and clarity; optimize only when measured.
- Use profiling tools to identify bottlenecks before refactoring.
- Cache frequently accessed metadata where appropriate.
- Avoid expensive allocations in hot paths.

## Error Handling

- Use explicit exception types; avoid generic `Exception`.
- Provide actionable error messages.
- Document which exceptions public methods can throw.
- Consider using `Result<T>` or similar discriminated unions for recoverable errors.

---

_Guidelines for simple, focused C# NuGet libraries. Prioritize clarity, correctness, and API stability._
