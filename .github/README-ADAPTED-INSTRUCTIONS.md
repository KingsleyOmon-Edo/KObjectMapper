# KObjectMapper: Adapted Instruction Files for NuGet Libraries

## Overview

The following files have been created/adapted to be suitable for developing a NuGet class library project like KObjectMapper. These replace the Clean Architecture-focused instructions that were originally copied from a different project type.

## Files Created/Adapted

### 1. `.github/copilot-instructions-library.md` ✨ NEW

**Purpose:** Language, formatting, and API design guidelines specific to NuGet libraries.

**Key differences from Clean Architecture version:**
- Removed all layered architecture patterns (Domain, Application, Infrastructure, Presentation)
- Removed entity/aggregate root patterns and strongly typed IDs (not applicable to utility libraries)
- Focused on **minimal public API surface**, **backward compatibility**, and **semantic versioning**
- Emphasized XML documentation for public APIs
- Included API design principles: validation at entry points, clear method contracts
- Removed all patterns related to repositories, services, CQRS, and dependency injection layering

**When to use:**
- As the primary reference for coding standards when adding features to KObjectMapper
- For naming conventions, formatting rules, and documentation requirements
- For understanding the library's API design philosophy

### 2. `.github/library-development-workflow.md` ✨ NEW

**Purpose:** A 6-phase workflow adapted for NuGet library development.

**Phases:**
1. **ANALYZE** — Understand requirements and API impact
2. **DESIGN** — Document API changes, implementation plan, test strategy
3. **IMPLEMENT** — Red-Green-Refactor using TDD
4. **TEST** — Verify correctness and coverage
5. **DOCUMENT** — Update README, CHANGELOG, inline docs, version number
6. **RELEASE** — Tag and publish to NuGet

**Key differences from Clean Architecture workflow:**
- No layered architecture decisions or database schema concerns
- Focus on **backward compatibility** and **semantic versioning**
- Every public API change must be intentional and documented
- Simplified test strategy (no repository patterns, CQRS, etc.)
- Version bump decision tree: MAJOR (breaking), MINOR (new features), PATCH (bug fixes)

**When to use:**
- When planning a significant feature or API change
- To ensure new public methods don't break backward compatibility
- As a checklist for releasing a new version

### 3. `.github/library-tdd-supplement.md` ✨ NEW

**Purpose:** Test-Driven Development practices tailored for library code.

**Contents:**
- Library-specific test priorities (public API contracts, validation, edge cases)
- Comprehensive examples for testing public methods
- Collection and enumerable handling patterns
- Extension method testing
- Backward compatibility testing
- Test organization and file structure for libraries
- ObjectMother pattern for test data

**When to use:**
- When writing tests for public API methods
- To understand the expected test names and patterns for KObjectMapper
- For guidance on structuring test files and test data

### 4. `.github/library-refactoring-guide.md` ✨ NEW

**Purpose:** Safe refactoring techniques that preserve the public API.

**Key principle:** Refactoring must never change observable behavior of public methods.

**Techniques covered:**
1. Extract Method
2. Rename for Clarity
3. Replace Magic Numbers and Strings
4. Simplify Complex Conditions
5. Use Switch Expressions
6. Cache Repeated Calculations
7. Extract Constants

**When to use:**
- When internal code is unclear or duplicated
- After making tests pass (the third step of Red-Green-Refactor)
- To keep the implementation clean without changing public behavior

**Anti-patterns highlighted:**
- Don't mix refactoring with feature development
- Don't change public API during refactoring
- Don't refactor before tests exist
- Don't optimize prematurely

---

## How These Files Relate

```
┌─ copilot-instructions-library.md ────── Coding standards & API design principles
│
├─ library-development-workflow.md ──── 6-phase workflow for features & releases
│  ├─ Phase 3 (IMPLEMENT) delegates to →
│  │  ├─ .github/skills/test-driven-development/SKILL.md (core TDD)
│  │  └─ ../library-tdd-supplement.md (library-specific TDD patterns)
│  │
│  └─ Phase 3 (REFACTOR) delegates to →
│     └─ ../library-refactoring-guide.md (safe refactoring techniques)
│
└─ library-tdd-supplement.md ────────── Library-specific test patterns
   (Complements the existing .github/skills/test-driven-development/SKILL.md)
```

---

## Quick Start for Contributors

### To add a new feature to KObjectMapper:

1. **Reference `.github/copilot-instructions-library.md`** for coding standards.
2. **Follow `.github/library-development-workflow.md`** for the phased approach.
3. **Use `.github/library-tdd-supplement.md`** for test naming and structure.
4. **Apply `.github/library-refactoring-guide.md`** when cleaning up code.

### Example: Adding a new public method

```
Step 1: ANALYZE (using workflow)
  ✓ Will this change the public API?
  ✓ Is it backward compatible?
  ✓ What tests are needed?

Step 2: DESIGN (document impact)
  ✓ New public signature in design.md
  ✓ Test plan outlined
  ✓ No breaking changes

Step 3: IMPLEMENT with TDD
  ✓ Write failing test (using library-tdd-supplement patterns)
  ✓ Implement code (following copilot-instructions-library)
  ✓ Refactor for clarity (using library-refactoring-guide)

Step 4: TEST
  ✓ All new tests pass
  ✓ All existing tests still pass
  ✓ Happy path, edge cases, error cases covered

Step 5: DOCUMENT
  ✓ XML comments on public method (copilot-instructions-library)
  ✓ Update CHANGELOG.md
  ✓ Add example to README if significant

Step 6: RELEASE
  ✓ Update version in .csproj (MAJOR/MINOR/PATCH)
  ✓ Create git tag
  ✓ Publish to NuGet (via CI/CD)
```

---

## File Purposes at a Glance

| File | Purpose | Read When |
|------|---------|-----------|
| `copilot-instructions-library.md` | Coding standards, naming, API design | Starting work on KObjectMapper |
| `library-development-workflow.md` | 6-phase workflow for features and releases | Planning a significant change |
| `library-tdd-supplement.md` | Test patterns, naming, assertions | Writing tests for public API |
| `library-refactoring-guide.md` | Safe refactoring techniques | Cleaning up or improving internal code |
| `.github/skills/test-driven-development/SKILL.md` | Core TDD philosophy (Red-Green-Refactor) | Learning or practicing TDD |

---

## What Was Removed / What Doesn't Apply

❌ **Not applicable to NuGet libraries:**
- Clean Architecture layering (Domain, Application, Infrastructure, Presentation)
- Repository patterns and unit-of-work abstractions
- CQRS (Command Query Responsibility Segregation)
- Service layer patterns
- EF Core, DbContext, migrations
- Dependency Injection container configuration for multiple layers
- Entity/Aggregate Root patterns
- Strongly Typed IDs (unless you want them, but they're not required)
- Multi-tenant architecture
- Background jobs and messaging patterns
- Event sourcing or domain events
- Audit tables and soft deletes

✅ **What Remains Applicable:**
- Code formatting and naming conventions
- Nullable reference types
- Pattern matching and switch expressions
- XML documentation
- Testing strategies
- Performance considerations
- Error handling with appropriate exception types

---

## Migration Checklist

If you're adapting these files for another NuGet library:

- [ ] Review `copilot-instructions-library.md` and adjust namespace/package names
- [ ] Review `library-development-workflow.md` and adjust version bumping strategy if different
- [ ] Review `library-tdd-supplement.md` and adjust test framework (xUnit/NUnit/MSTest)
- [ ] Review `library-refactoring-guide.md` (generally framework-agnostic)
- [ ] Ensure `.github/skills/test-driven-development/SKILL.md` is already present
- [ ] Add these files to your project's `.github/` directory
- [ ] Reference them in your project's README or CONTRIBUTING guide

---

## Contact & Feedback

These files are tailored for NuGet class library projects. If you find they need adjustment for KObjectMapper specifically, or if you create additional guidance files (e.g., for package publishing or versioning), document them in this index and keep everyone on the same page.

---

_Last updated: Adapted from Clean Architecture template for NuGet library use._
