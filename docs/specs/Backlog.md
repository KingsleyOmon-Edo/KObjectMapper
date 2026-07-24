# Backlog

## Spec-Driven Milestones

> Source generators are tracked as an **additional feature set** in this backlog.

## Milestone M1 - Core Mapping Contracts (Alpha foundation)

### Epic: Configuration and Mapping Contracts

- [x] **US-01: Configurable mapping profiles**
  - As a developer, I want to define mapping profiles per source/target pair so that mapping rules are explicit, reusable, and versionable.
  - **Acceptance criteria:**
    - Profile API exists for registering type maps.
    - Profile registration supports assembly scanning.
    - Profile validation reports invalid/missing rules at startup.

- [ ] **US-02: Custom member mapping and ignore rules**
  - As a developer, I want to map members with different names and ignore specific members so that DTO/domain shape differences are supported safely.
  - **Acceptance criteria:**
    - Configure `ForMember` mapping from differently named source properties.
    - Configure `Ignore` per member.
    - Unmapped required members are reported by validation.

- [ ] **US-03: Null-handling policy configuration**
  - As a developer, I want configurable null behavior (propagate, ignore, substitute) so that mapping behavior is predictable across services.
  - **Acceptance criteria:**
    - Global and per-map null policy options exist.
    - Optional null substitution values can be configured per member.
    - Behavior is verified by tests for object and collection mappings.

- [ ] **US-04: Strict mapping mode and startup validation**
  - As a platform engineer, I want strict mode and startup validation so that mapping defects fail fast before runtime traffic.
  - **Acceptance criteria:**
    - Strict mode fails when no valid map exists.
    - Validation catches ambiguous constructor/member bindings.
    - Validation output is structured and actionable.

### Epic: Type System and Conversion Depth (Phase 1)

- [ ] **US-05: Pluggable type converters**
  - As a developer, I want to register custom converters so that complex domain transformations are supported beyond `Convert.ChangeType`.
  - **Acceptance criteria:**
    - Converter interface and registration API are available.
    - Converters can be global and per-map.
    - Built-in converters cover common primitives, enums, GUID, and DateTimeOffset patterns.

- [ ] **US-06: Enum and string conversion safety**
  - As a developer, I want safe enum conversion with validation so that invalid enum values do not silently corrupt data.
  - **Acceptance criteria:**
    - Supports numeric-to-enum and string-to-enum conversions.
    - Invalid values return deterministic mapping failure details.
    - Supports case-insensitive string enum parsing via option.

## Milestone M2 - Performance and Source-Generation Pipeline

### Epic: Compile-Time Mapping and Source Generators

- [ ] **US-07: Source-generated mapping plans for configured type pairs**
  - As a developer, I want mapping code generated at compile-time so that runtime reflection cost is reduced on hot paths.
  - **Acceptance criteria:**
    - Incremental source generator produces mapping implementations for registered source/target pairs.
    - Generated mappers compile without requiring consumer-written partial methods.
    - Build diagnostics identify unsupported member patterns with clear error or warning messages.

- [ ] **US-08: Runtime fallback and feature flags for generator adoption**
  - As a maintainer, I want source generation to be opt-in with safe fallback so that existing consumers can upgrade incrementally.
  - **Acceptance criteria:**
    - Configuration flag allows enabling/disabling generated mapping per profile or globally.
    - When generation is unavailable, runtime mapper falls back to current reflection/delegate pipeline.
    - Behavior parity tests verify equivalent mapping outputs between generated and runtime modes.

- [ ] **US-09: Source generator debugging, diagnostics, and docs**
  - As a developer, I want actionable diagnostics and documentation for generated code so that I can troubleshoot mapping issues quickly.
  - **Acceptance criteria:**
    - Generator emits deterministic diagnostics with member path and type details.
    - Optional generated-file emission is documented for debugging.
    - README/spec docs include setup, constraints, and troubleshooting for generator mode.

### Epic: Diagnostics, Resilience, and Operability (Phase 1)

- [ ] **US-10: Structured mapping result and diagnostics**
  - As an operator, I want structured mapping diagnostics so that failed mappings are observable and debuggable in production.
  - **Acceptance criteria:**
    - Non-throwing API variant returns success/failure result with error details.
    - Errors include member path, source/target types, and failure reason.
    - Optional integration hooks for logging and metrics.

- [ ] **US-11: Thread-safety and performance hardening**
  - As a platform engineer, I want thread-safe caching and precompiled mapping delegates so that mapping throughput is stable under high concurrency.
  - **Acceptance criteria:**
    - Thread-safe cache for map plans/delegates.
    - Cold-start and warm-path benchmarks are documented.
    - No shared mutable state race conditions in parallel tests.

## Milestone M3 - Collection/Queryable and Graph Completeness

### Epic: Collections and Queryable Support

- [ ] **US-12: Advanced collection mapping strategies**
  - As a developer, I want configurable collection merge strategies so that updates to existing collections can preserve identity and apply add/update/remove deterministically.
  - **Acceptance criteria:**
    - Supports replace, merge-by-key, and append modes.
    - Merge-by-key supports custom key selectors.
    - Behavior is deterministic and tested for lists and read-only targets.

- [ ] **US-13: Queryable projection support**
  - As a developer, I want expression-based projection support for `IQueryable` so that ORM queries can be projected server-side efficiently.
  - **Acceptance criteria:**
    - API supports mapping expressions to destination types.
    - Works with EF Core translation-friendly expressions.
    - Falls back with explicit error when expression cannot be translated.

### Epic: Type System and Conversion Depth (Phase 2)

- [ ] **US-14: Nested graph and circular reference handling**
  - As a developer, I want robust nested-object graph mapping with circular reference handling so that large object graphs map without stack overflow or duplicated nodes.
  - **Acceptance criteria:**
    - Reference-preservation option exists.
    - Circular-reference detection prevents infinite recursion.
    - Tests cover deep nesting and cyclical graphs.

- [ ] **US-15: Cancellation and async mapping hooks**
  - As a developer, I want async mapping hooks with cancellation support for expensive conversion pipelines so that requests can be cancelled cleanly.
  - **Acceptance criteria:**
    - Async mapping API supports `CancellationToken`.
    - Custom async converters are supported.
    - Cancellation behavior is tested.

## Milestone M4 - Security, Stability, and GA Readiness

### Epic: Security and API Stability

- [ ] **US-16: Sensitive data mapping guards**
  - As a security-conscious developer, I want member-level opt-out for sensitive fields so that secrets and PII are not accidentally mapped.
  - **Acceptance criteria:**
    - Attribute- and configuration-based sensitive member exclusion.
    - Policy supports default-deny mode for marked types.
    - Tests verify excluded fields are never copied.

- [ ] **US-17: Public API stabilization and versioning policy**
  - As a maintainer, I want a stable API contract and semantic versioning policy so that consumers can upgrade safely.
  - **Acceptance criteria:**
    - Public API baseline is generated and tracked.
    - Breaking-change checks are part of CI.
    - Upgrade guidance is documented in changelog/release notes.

- [ ] **US-18: Documentation completeness and production guidance**
  - As a consumer, I want complete production usage guidance so that I can adopt the mapper safely.
  - **Acceptance criteria:**
    - Document configuration patterns, failure modes, and edge-case behavior.
    - Include DI usage through the provided registration extension.
    - Include migration notes for API contract changes.
