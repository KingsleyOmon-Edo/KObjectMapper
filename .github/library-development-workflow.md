---
description: 'Library-focused development workflow — process for features, bug fixes, and improvements in NuGet class library projects'
applyTo: 'docs/specs/**,features/**/*.md,**/*.feature'
---

# Library Development Workflow

> **Scope:** This workflow is designed for NuGet class library projects (like KObjectMapper) that prioritize a minimal, stable public API. It intentionally avoids layered architecture patterns and focuses on correctness, backward compatibility, and clear API design.

## Key Principles

1. **Minimal API Surface:** Every public type and method contributes to the semantic versioning contract.
2. **Backward Compatibility First:** Breaking changes require major version bumps and should be exceptional.
3. **Clear Test Coverage:** Public API and edge cases must be tested.
4. **Semantic Versioning:** MAJOR.MINOR.PATCH — follow semver strictly.

## Workflow Phases

### Phase 1: ANALYZE

**Objective:** Understand the requirement, existing implementation, and impact on the public API.

**Checklist:**

- [ ] Read all related code, tests, and documentation.
- [ ] Assess whether the change affects the public API.
- [ ] If yes, document the change impact:
  - [ ] New public member?
  - [ ] Modified public signature?
  - [ ] Behavior change in existing public method?
- [ ] Identify edge cases and error conditions.
- [ ] Evaluate backward compatibility risk.
- [ ] Determine if this requires a major, minor, or patch version bump.

**Confidence Assessment:**

- **High Confidence (>85%):** Proceed directly to implementation.
- **Medium Confidence (66-85%):** Create a small proof-of-concept before full implementation.
- **Low Confidence (<66%):** Research and prototype; re-analyze before proceeding.

### Phase 2: DESIGN

**Objective:** Document the technical approach and its impact on the API.

**Checklist:**

- [ ] **API Impact:**
  - Document any new public types or methods.
  - Document any changes to existing public signatures.
  - Specify parameter validation requirements.
  - Document exceptions that can be thrown.

- [ ] **Implementation Plan:**
  - Outline the algorithm or approach.
  - Identify performance considerations.
  - Note any internal helper methods or utilities needed.

- [ ] **Test Strategy:**
  - List happy-path test scenarios.
  - List edge-case and error scenarios.
  - Specify assertion style (FluentAssertions, xUnit, etc.).

- [ ] **Backward Compatibility:**
  - Will this change break existing code?
  - If yes, is it worth a major version bump?
  - Can the change be made in a backward-compatible way?

### Phase 3: IMPLEMENT

**Objective:** Write code that passes tests and maintains backward compatibility.

**Process:**

1. **Write failing tests first** (Red-Green-Refactor cycle):
   - Create tests that verify the desired behavior.
   - Tests should initially fail.

2. **Implement code** to make tests pass:
   - Write the minimal code to pass the tests.
   - Follow the coding standards in `.github/copilot-instructions-library.md`.

3. **Refactor for clarity**:
   - Extract duplicated logic.
   - Improve variable names.
   - Simplify complex conditions.
   - Ensure all public members have XML documentation.

4. **Validate:**
   - All new and existing tests pass.
   - No regressions in other tests.
   - Code review checklist met.

**Code Review Checklist:**

- [ ] Public API clearly documented with XML comments.
- [ ] All parameters validated at entry points.
- [ ] Edge cases and error conditions tested.
- [ ] No breaking changes to existing public API.
- [ ] Follows naming conventions (PascalCase, camelCase, `I` prefixes for interfaces).
- [ ] Follows formatting rules (Allman braces, file-scoped namespaces, explicit types).
- [ ] Performance implications considered (no unnecessary allocations).

### Phase 4: TEST

**Objective:** Verify correctness and coverage.

**Test Plan:**

- **Happy paths:** Normal usage scenarios.
- **Edge cases:** Boundary conditions, empty collections, extreme values.
- **Error conditions:** Null arguments, invalid inputs, expected exceptions.
- **Integration:** Multiple public methods working together.

**Test Naming Convention:**

```
MethodName_Condition_ExpectedBehavior
```

Example:
```csharp
[Fact]
public void MapTo_WhenSourceIsNull_ThrowsArgumentNullException()
{
	var target = new object();

	Assert.Throws<ArgumentNullException>(() => mapper.MapTo(null, target));
}
```

**Success Criteria:**

- [ ] All new tests pass.
- [ ] All existing tests still pass.
- [ ] Code coverage on public API paths is high (aim for >80%).

### Phase 5: DOCUMENT

**Objective:** Update user-facing documentation and version tracking.

**Checklist:**

- [ ] **README.md** — Add examples if new public features exposed.
- [ ] **CHANGELOG.md** — Document the change:
  - What changed?
  - Is it a breaking change?
  - Impact on users (if any)?

- [ ] **Inline Code Documentation** — All public types/methods have proper XML comments.

**Version Bump Decision:**

- **MAJOR:** Breaking changes to existing public API.
- **MINOR:** New public functionality, backward compatible.
- **PATCH:** Bug fixes, internal improvements, no API changes.

### Phase 6: RELEASE

**Objective:** Prepare and publish the new version.

**Checklist:**

- [ ] Version number updated in `.csproj`.
- [ ] CHANGELOG.md reflects the version and date.
- [ ] git tag created: `v{version}` (e.g., `v1.2.0`).
- [ ] Package built and published to NuGet (automatic via CI/CD if available).

## Quick Reference: When to Use This Workflow

**Use this workflow for:**
- New public methods or types
- Changes to existing public signatures
- Significant bug fixes affecting public behavior
- New feature that impacts users

**Skip to Phase 4 (TEST) for:**
- Internal refactoring (no API changes)
- Documentation-only updates
- Performance improvements (no behavior change)

**Use a simpler process for:**
- Typo fixes
- Comment clarifications
- Internal helper method additions (marked `internal`)

## Backward Compatibility Guardrails

- **Never remove** a public type, method, or property without releasing a new MAJOR version.
- **Never change** the signature of a public method (parameter names, types, or count) without a major version.
- **Never change** the behavior of a public method in a way that breaks existing valid usage.
- **When in doubt:** Create an overload or add a new method rather than modifying existing ones.

---

_A pragmatic workflow for building stable, reliable NuGet class libraries._
