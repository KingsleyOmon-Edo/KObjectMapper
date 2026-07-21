---
description: 'Specification-Driven Workflow v1 — a heavyweight, documentation-first process for substantial features, epics, and architectural changes. Does not apply to small fixes, single-file edits, or routine changes; see scope note below.'
applyTo: 'docs/specs/**,features/**/*.md,**/*.feature'
---
# Spec Driven Workflow v1

> **Scope note:** This workflow governs *process* for substantial, multi-file features and architectural changes — it intentionally no longer applies to every file in the repository (`applyTo` was narrowed from `**`). For day-to-day edits, bug fixes, and routine changes, follow `.github/copilot-instructions.md` directly without invoking the full 6-phase loop below. Use this workflow when a change is large enough to warrant its own `requirements.md` / `design.md` / `tasks.md` triad — typically a new feature, a new aggregate, or a cross-cutting architectural change — and reference it explicitly (e.g. "follow the spec-driven workflow for this") when you want it invoked. All technical standards (error handling, CQRS, repository patterns, naming conventions) still come from `.github/copilot-instructions.md`; this file governs process and documentation cadence only, not code-level rules. Phase 3 (IMPLEMENT) specifically delegates its actual coding process to the Test-Driven Development skill at `.github/skills/test-driven-development/SKILL.md`, and its Refactor step further delegates to the Refactoring skill at `.github/skills/refactoring/SKILL.md` — see that phase below for how the three fit together. A standalone technical-debt cleanup with no new feature behavior does not need this 6-phase workflow at all — use the Refactoring skill directly for that case; see Technical Debt Management below for how the two relate.

**Specification-Driven Workflow:**
Bridge the gap between requirements and implementation.

**Maintain these artifacts at all times:**

- **`requirements.md`**: User stories and acceptance criteria in structured EARS notation.
- **`design.md`**: Technical architecture, sequence diagrams, implementation considerations.
- **`tasks.md`**: Detailed, trackable implementation plan.

## Universal Documentation Framework

**Documentation Rule:**
Use the detailed templates as the **primary source of truth** for all documentation.

**Summary formats:**
Use only for concise artifacts such as changelogs and pull request descriptions.

### Detailed Documentation Templates

#### Action Documentation Template (All Steps/Executions/Tests)

```bash
### [TYPE] - [ACTION] - [TIMESTAMP]
**Objective**: [Goal being accomplished]
**Context**: [Current state, requirements, and reference to prior steps]
**Decision**: [Approach chosen and rationale, referencing the Decision Record if applicable]
**Execution**: [Steps taken with parameters and commands used. For code, include file paths.]
**Output**: [Complete and unabridged results, logs, command outputs, and metrics]
**Validation**: [Success verification method and results. If failed, include a remediation plan.]
**Next**: [Automatic continuation plan to the next specific action]
```

#### Decision Record Template (All Decisions)

```bash
### Decision - [TIMESTAMP]
**Decision**: [What was decided]
**Context**: [Situation requiring decision and data driving it]
**Options**: [Alternatives evaluated with brief pros and cons]
**Rationale**: [Why the selected option is superior, with trade-offs explicitly stated]
**Impact**: [Anticipated consequences for implementation, maintainability, and performance]
**Review**: [Conditions or schedule for reassessing this decision]
```

### Summary Formats (for Reporting)

#### Streamlined Action Log

For generating concise changelogs. Each log entry is derived from a full Action Document.

`[TYPE][TIMESTAMP] Goal: [X] → Action: [Y] → Result: [Z] → Next: [W]`

#### Compressed Decision Record

For use in pull request summaries or executive summaries.

`Decision: [X] | Rationale: [Y] | Impact: [Z] | Review: [Date]`

## Execution Workflow (6-Phase Loop)

**Never skip any step. Use consistent terminology. Reduce ambiguity.**

### **Phase 1: ANALYZE**

**Objective:**

- Understand the problem.
- Analyze the existing system.
- Produce a clear, testable set of requirements.
- Think about the possible solutions and their implications.

**Checklist:**

- [ ] Read all provided code, documentation, tests, and logs.
      - Document file inventory, summaries, and initial analysis results.
- [ ] Define requirements in **EARS Notation**:
      - Transform feature requests into structured, testable requirements.
      - Format: `WHEN [a condition or event], THE SYSTEM SHALL [expected behavior]`
- [ ] Identify dependencies and constraints.
      - Document a dependency graph with risks and mitigation strategies.
- [ ] Map data flows and interactions.
      - Document system interaction diagrams and data models.
- [ ] Catalog edge cases and failures.
      - Document a comprehensive edge case matrix and potential failure points.
- [ ] Assess confidence.
      - Generate a **Confidence Score (0-100%)** based on clarity of requirements, complexity, and problem scope.
      - Document the score and its rationale.

**Critical Constraint:**

- **Do not proceed until all requirements are clear and documented.**

### **Phase 2: DESIGN**

**Objective:**

- Create a comprehensive technical design and a detailed implementation plan.

**Checklist:**

- [ ] **Define adaptive execution strategy based on Confidence Score:**
  - **High Confidence (>85%)**
    - Draft a comprehensive, step-by-step implementation plan.
    - Skip proof-of-concept steps.
    - Proceed with full, automated implementation.
    - Maintain standard comprehensive documentation.
  - **Medium Confidence (66–85%)**
    - Prioritize a **Proof-of-Concept (PoC)** or **Minimum Viable Product (MVP)**.
    - Define clear success criteria for PoC/MVP.
    - Build and validate PoC/MVP first, then expand plan incrementally.
    - Document PoC/MVP goals, execution, and validation results.
  - **Low Confidence (<66%)**
    - Dedicate first phase to research and knowledge-building.
    - Use semantic search and analyze similar implementations.
    - Synthesize findings into a research document.
    - Re-run ANALYZE phase after research.
    - Escalate only if confidence remains low.

- [ ] **Document technical design in `design.md`:**
  - **Architecture:** High-level overview of components and interactions.
  - **Data Flow:** Diagrams and descriptions.
  - **Interfaces:** API contracts, schemas, public-facing function signatures.
  - **Data Models:** Data structures and database schemas.

- [ ] **Document error handling:**
  - Create an error matrix with procedures and expected responses.

- [ ] **Define unit testing strategy.**
      - The strategy itself (framework, assertion style, naming convention) is project-specific; the *process* used to write those tests in Phase 3 is the Test-Driven Development skill (`.github/skills/test-driven-development/SKILL.md`).

- [ ] **Create implementation plan in `tasks.md`:**
  - For each task, include description, expected outcome, and dependencies.

**Critical Constraint:**

- **Do not proceed to implementation until design and plan are complete and validated.**

### **Phase 3: IMPLEMENT**

**Objective:**

- Write production-quality code according to the design and plan.

**TDD Governs This Phase:**

For any implementation task that produces testable behavior (a new class, method, or feature — anything with a defined input/output or observable effect), this phase is executed using the **Test-Driven Development skill** (`.github/skills/test-driven-development/SKILL.md`), not written directly. The Refactor step of every cycle specifically uses the **Refactoring skill** (`.github/skills/refactoring/SKILL.md`) for diagnosing what to improve (its code smell catalog) and exactly how (its mechanics catalog) — the TDD skill governs *when* to refactor inside the loop, the Refactoring skill governs *what technique to apply*. Concretely:

- Build the Red-Green-Refactor Test List called for in that skill *from* the acceptance criteria already captured in `requirements.md` — each EARS requirement becomes one or more Test List entries, so the two artifacts stay traceable to each other rather than diverging.
- Each "small, testable increment" in the checklist below **is** one full Red → Green → Refactor cycle from that skill, not an informal chunk of work — do not write production code before its failing test exists.
- Document each increment (as the checklist below requires) using the test's name and the specific Red/Green/Refactor step it corresponds to, so the Action Documentation trail doubles as a TDD log.
- Purely structural work with no independently testable behavior (e.g. scaffolding a folder layout, wiring DI registrations with no logic of their own) is not subject to the TDD skill's Red-Green-Refactor loop, but should still be committed as its own small, reviewable increment.
- **Commit after every Red → Green step and every completed Refactor step**, per `.github/copilot-instructions.md`'s "Committing during TDD and refactoring cycles" guidance — never batch several cycles into one commit. This keeps the rollback surface small if a later increment turns out to be wrong: reverting one commit undoes one test's worth of change, not an entire feature's worth.

**Checklist:**

- [ ] Code in small, testable increments — each increment is one Red → Green → Refactor cycle per the TDD skill, using the Refactoring skill for the Refactor step's technique.
      - Document each increment with the test name, the code changes, results, and test links.
- [ ] Build and maintain the Test List (TDD skill, Step 0) derived from `requirements.md`'s EARS criteria before writing the first test.
      - Document the initial Test List, and update it in place as new cases are discovered mid-implementation.
- [ ] Commit each Red → Green step and each completed refactoring step separately, per `.github/copilot-instructions.md`'s commit conventions.
      - Document the commit hash or reference alongside each increment's entry in the Action Documentation trail.
- [ ] Implement from dependencies upward.
      - Document resolution order, justification, and verification.
- [ ] Follow conventions.
      - Document adherence and any deviations with a Decision Record.
- [ ] Add meaningful comments.
      - Focus on intent ("why"), not mechanics ("what").
- [ ] Create files as planned.
      - Document file creation log.
- [ ] Update task status in real time.

**Critical Constraint:**

- **Do not merge or deploy code until all implementation steps are documented and tested.**

### **Phase 4: VALIDATE**

**Objective:**

- Verify that implementation meets all requirements and quality standards.

**Checklist:**

- [ ] Execute automated tests.
      - Document outputs, logs, and coverage reports.
      - For failures, document root cause analysis and remediation.
- [ ] Perform manual verification if necessary.
      - Document procedures, checklists, and results.
- [ ] Test edge cases and errors.
      - Document results and evidence of correct error handling.
- [ ] Verify performance.
      - Document metrics and profile critical sections.
- [ ] Log execution traces.
      - Document path analysis and runtime behavior.

**Critical Constraint:**

- **Do not proceed until all validation steps are complete and all issues are resolved.**

### **Phase 5: REFLECT**

**Objective:**

- Improve codebase, update documentation, and analyze performance.

**Checklist:**

- [ ] Refactor for maintainability.
      - Document decisions, before/after comparisons, and impact.
- [ ] Update all project documentation.
      - Ensure all READMEs, diagrams, and comments are current.
- [ ] Identify potential improvements.
      - Document backlog with prioritization.
- [ ] Validate success criteria.
      - Document final verification matrix.
- [ ] Perform meta-analysis.
      - Reflect on efficiency, tool usage, and protocol adherence.
- [ ] Auto-create technical debt issues.
      - Document inventory and remediation plans.

**Critical Constraint:**

- **Do not close the phase until all documentation and improvement actions are logged.**

### **Phase 6: HANDOFF**

**Objective:**

- Package work for review and deployment, and transition to next task.

**Checklist:**

- [ ] Generate executive summary.
      - Use **Compressed Decision Record** format.
- [ ] Prepare pull request (if applicable):
    1. Executive summary.
    2. Changelog from **Streamlined Action Log**.
    3. Links to validation artifacts and Decision Records.
    4. Links to final `requirements.md`, `design.md`, and `tasks.md`.
- [ ] Finalize workspace.
      - Archive intermediate files, logs, and temporary artifacts to `.agent_work/`.
- [ ] Continue to next task.
      - Document transition or completion.

**Critical Constraint:**

- **Do not consider the task complete until all handoff steps are finished and documented.**

## Troubleshooting & Retry Protocol

**If you encounter errors, ambiguities, or blockers:**

**Checklist:**

1. **Re-analyze**:
   - Revisit the ANALYZE phase.
   - Confirm all requirements and constraints are clear and complete.
2. **Re-design**:
   - Revisit the DESIGN phase.
   - Update technical design, plans, or dependencies as needed.
3. **Re-plan**:
   - Adjust the implementation plan in `tasks.md` to address new findings.
4. **Retry execution**:
   - Re-execute failed steps with corrected parameters or logic.
5. **Escalate**:
   - If the issue persists after retries, follow the escalation protocol.

**Critical Constraint:**

- **Never proceed with unresolved errors or ambiguities. Always document troubleshooting steps and outcomes.**

## Technical Debt Management (Automated)

**Remediation uses the Refactoring skill.** When actually fixing an item logged below — as opposed to deferring it — use `.github/skills/refactoring/SKILL.md` directly. Its code smell catalog gives the vocabulary for the `Reason`/`Remediation` fields below, its mechanics catalog gives the exact controlled steps to apply, and its own commit-per-step discipline (see `.github/copilot-instructions.md`) applies independent of whether this remediation is happening inside a spec-driven feature or as its own standalone cleanup task outside this workflow entirely.

### Identification & Documentation

- **Code Quality**: Continuously assess code quality during implementation using static analysis.
- **Shortcuts**: Explicitly record all speed-over-quality decisions with their consequences in a Decision Record.
- **Workspace**: Monitor for organizational drift and naming inconsistencies.
- **Documentation**: Track incomplete, outdated, or missing documentation.

### Auto-Issue Creation Template

```text
**Title**: [Technical Debt] - [Brief Description]
**Priority**: [High/Medium/Low based on business impact and remediation cost]
**Location**: [File paths and line numbers]
**Reason**: [Why the debt was incurred, linking to a Decision Record if available. Name the specific code smell from the Refactoring skill's catalog where applicable.]
**Impact**: [Current and future consequences (e.g., slows development, increases bug risk)]
**Remediation**: [Specific, actionable resolution steps — name the specific refactoring technique(s) from the Refactoring skill's mechanics catalog where applicable]
**Effort**: [Estimate for resolution (e.g., T-shirt size: S, M, L)]
```

### Remediation (Auto-Prioritized)

- Risk-based prioritization with dependency analysis.
- Effort estimation to aid in future planning.
- Propose migration strategies for large refactoring efforts.
- Before remediating code with no existing test coverage, add characterization tests first — see the Refactoring skill's "Applying this to legacy or high-debt code" section.

## Quality Assurance (Automated)

### Continuous Monitoring

- **Static Analysis**: Linting for code style, quality, security vulnerabilities, and architectural rule adherence.
- **Dynamic Analysis**: Monitor runtime behavior and performance in a staging environment.
- **Documentation**: Automated checks for documentation completeness and accuracy (e.g., linking, format).

### Quality Metrics (Auto-Tracked)

- Code coverage percentage and gap analysis.
- Cyclomatic complexity score per function/method.
- Maintainability index assessment.
- Technical debt ratio (e.g., estimated remediation time vs. development time).
- Documentation coverage percentage (e.g., public methods with comments).

## EARS Notation Reference

**EARS (Easy Approach to Requirements Syntax)** - Standard format for requirements:

- **Ubiquitous**: `THE SYSTEM SHALL [expected behavior]`
- **Event-driven**: `WHEN [trigger event] THE SYSTEM SHALL [expected behavior]`
- **State-driven**: `WHILE [in specific state] THE SYSTEM SHALL [expected behavior]`
- **Unwanted behavior**: `IF [unwanted condition] THEN THE SYSTEM SHALL [required response]`
- **Optional**: `WHERE [feature is included] THE SYSTEM SHALL [expected behavior]`
- **Complex**: Combinations of the above patterns for sophisticated requirements

Each requirement must be:

- **Testable**: Can be verified through automated or manual testing
- **Unambiguous**: Single interpretation possible
- **Necessary**: Contributes to the system's purpose
- **Feasible**: Can be implemented within constraints
- **Traceable**: Linked to user needs and design elements
