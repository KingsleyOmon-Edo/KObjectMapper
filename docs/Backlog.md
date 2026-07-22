# Backlog

## Production-Grade Mapper User Stories

### Epic: Configuration and Mapping Contracts

1. **Configurable mapping profiles**
   - As a developer, I want to define mapping profiles per source/target pair so that mapping rules are explicit, reusable, and versionable.
   - **Acceptance criteria:**
     - Profile API exists for registering type maps.
     - Profile registration supports assembly scanning.
     - Profile validation reports invalid/missing rules at startup.

2. **Custom member mapping and ignore rules**
   - As a developer, I want to map members with different names and ignore specific members so that DTO/domain shape differences are supported safely.
   - **Acceptance criteria:**
     - Configure `ForMember` mapping from differently named source properties.
     - Configure `Ignore` per member.
     - Unmapped required members are reported by validation.

3. **Null-handling policy configuration**
   - As a developer, I want configurable null behavior (propagate, ignore, substitute) so that mapping behavior is predictable across services.
   - **Acceptance criteria:**
     - Global and per-map null policy options exist.
     - Optional null substitution values can be configured per member.
     - Behavior is verified by tests for object and collection mappings.

4. **Strict mapping mode and startup validation**
   - As a platform engineer, I want strict mode and startup validation so that mapping defects fail fast before runtime traffic.
   - **Acceptance criteria:**
     - Strict mode fails when no valid map exists.
     - Validation catches ambiguous constructor/member bindings.
     - Validation output is structured and actionable.

### Epic: Type System and Conversion Depth

5. **Pluggable type converters**
   - As a developer, I want to register custom converters so that complex domain transformations are supported beyond `Convert.ChangeType`.
   - **Acceptance criteria:**
     - Converter interface and registration API are available.
     - Converters can be global and per-map.
     - Built-in converters cover common primitives, enums, GUID, and DateTimeOffset patterns.

6. **Enum and string conversion safety**
   - As a developer, I want safe enum conversion with validation so that invalid enum values do not silently corrupt data.
   - **Acceptance criteria:**
     - Supports numeric-to-enum and string-to-enum conversions.
     - Invalid values return deterministic mapping failure details.
     - Supports case-insensitive string enum parsing via option.

7. **Nested graph and circular reference handling**
   - As a developer, I want robust nested-object graph mapping with circular reference handling so that large object graphs map without stack overflow or duplicated nodes.
   - **Acceptance criteria:**
     - Reference-preservation option exists.
     - Circular-reference detection prevents infinite recursion.
     - Tests cover deep nesting and cyclical graphs.

### Epic: Collections and Queryable Support

8. **Advanced collection mapping strategies**
   - As a developer, I want configurable collection merge strategies so that updates to existing collections can preserve identity and apply add/update/remove deterministically.
   - **Acceptance criteria:**
     - Supports replace, merge-by-key, and append modes.
     - Merge-by-key supports custom key selectors.
     - Behavior is deterministic and tested for lists and read-only targets.

9. **Queryable projection support**
   - As a developer, I want expression-based projection support for `IQueryable` so that ORM queries can be projected server-side efficiently.
   - **Acceptance criteria:**
     - API supports mapping expressions to destination types.
     - Works with EF Core translation-friendly expressions.
     - Falls back with explicit error when expression cannot be translated.

### Epic: Diagnostics, Resilience, and Operability

10. **Structured mapping result and diagnostics**
    - As an operator, I want structured mapping diagnostics so that failed mappings are observable and debuggable in production.
    - **Acceptance criteria:**
      - Non-throwing API variant returns success/failure result with error details.
      - Errors include member path, source/target types, and failure reason.
      - Optional integration hooks for logging and metrics.

11. **Thread-safety and performance hardening**
    - As a platform engineer, I want thread-safe caching and precompiled mapping delegates so that mapping throughput is stable under high concurrency.
    - **Acceptance criteria:**
      - Thread-safe cache for map plans/delegates.
      - Cold-start and warm-path benchmarks are documented.
      - No shared mutable state race conditions in parallel tests.

12. **Cancellation and async mapping hooks**
    - As a developer, I want async mapping hooks with cancellation support for expensive conversion pipelines so that requests can be cancelled cleanly.
    - **Acceptance criteria:**
      - Async mapping API supports `CancellationToken`.
      - Custom async converters are supported.
      - Cancellation behavior is tested.

### Epic: Security and API Stability

13. **Sensitive data mapping guards**
    - As a security-conscious developer, I want member-level opt-out for sensitive fields so that secrets and PII are not accidentally mapped.
    - **Acceptance criteria:**
      - Attribute- and configuration-based sensitive member exclusion.
      - Policy supports default-deny mode for marked types.
      - Tests verify excluded fields are never copied.

14. **Public API stabilization and versioning policy**
    - As a maintainer, I want a stable API contract and semantic versioning policy so that consumers can upgrade safely.
    - **Acceptance criteria:**
      - Public API baseline is generated and tracked.
      - Breaking-change checks are part of CI.
      - Upgrade guidance is documented in changelog/release notes.

15. **Documentation completeness and production guidance**
    - As a consumer, I want complete production usage guidance so that I can adopt the mapper safely.
    - **Acceptance criteria:**
      - Document configuration patterns, failure modes, and edge-case behavior.
      - Include DI usage through the provided registration extension.
      - Include migration notes for API contract changes.
