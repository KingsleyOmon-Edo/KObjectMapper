---
name: refactoring
description: >-
  Guides controlled, low-risk refactoring: recognizing code smells, choosing
  the right named technique, and applying it in small, independently
  verifiable, committed steps so behavior never changes mid-refactor. Use
  when asked to "refactor this", "clean up this code", "reduce technical
  debt", "improve the design without changing behavior", during the Refactor
  step of a Red-Green-Refactor TDD cycle, or when working through an existing
  codebase that has accumulated complexity or duplication.
metadata:
  author: project-team
  version: "1.0"
  source: >-
    Adapted from Martin Fowler, "Refactoring: Improving the Design of
    Existing Code" (Addison-Wesley, 1999), particularly Chapters 2, 3, and 5.
    Companion to the Test-Driven Development skill at
    .github/skills/test-driven-development/SKILL.md.
---

# Refactoring — Improving Design Without Changing Behavior

## What refactoring is, precisely

Refactoring is a change to the internal structure of code that makes it easier to understand and cheaper to modify, without changing its observable behavior. Two properties define it and both are non-negotiable: the change is **behavior-preserving**, and it proceeds in a **disciplined, verifiable sequence** rather than as one large rewrite. A change that alters what the software does — even a fix, even an improvement — is not a refactoring; it's a different kind of change that deserves its own commit, its own tests, and (if this project uses the Test-Driven Development skill) its own Red-Green cycle first.

## When this skill applies — two scenarios

**Scenario 1 — the Refactor step of a TDD cycle.** The Test-Driven Development skill (`.github/skills/test-driven-development/SKILL.md`) already establishes the Red-Green-Refactor loop and the rule that refactoring only happens while Green, one small step at a time. This skill is the companion that answers the two questions that skill deliberately leaves open: *what specifically is wrong with this code* (the code smells below), and *exactly how do I fix it safely* (the mechanics catalog below). Use both skills together during Refactor — the TDD skill for the loop discipline, this skill for the diagnosis and the technique.

**Scenario 2 — an existing codebase with accumulated technical debt, outside any active TDD cycle.** Here there is no immediate failing test driving the work; the trigger is recognizing a smell during other work, a deliberate cleanup task, or a code review. The same behavior-preservation discipline applies, but with one critical precondition first: **if the code you're about to refactor has no tests, do not refactor it yet.** Write characterization tests first — tests that pin down what the code *currently* does, bugs included, purely to give yourself a safety net. Writing characterization tests is a distinct activity from TDD (you are describing existing behavior, not designing new behavior) and from refactoring (you are adding coverage, not restructuring) — do not blend it with either. Once the code you're about to touch has adequate characterization coverage, refactor exactly as described below.

## The Two Hats

At any moment, you are wearing exactly one of two hats, never both:

- **Adding function** — you're extending what the software does, and you're allowed to add new tests, because you're adding new behavior for them to describe.
- **Refactoring** — you're restructuring what already exists, and you must not add, remove, or change what any test currently asserts. You may still add tests *during* a refactor, but only to increase confidence in behavior that already exists — never to describe something new.

This is the same discipline the TDD skill states as "never touch tests and production code in the same step," restated from its own origin: you cannot simultaneously be improving structure and changing behavior, because if something breaks, you won't know which of the two things you were doing caused it. Notice which hat you're wearing at every step; if you catch yourself wanting to fix a bug or add a check *while* refactoring, stop, finish the refactor, commit, then switch hats deliberately.

## When to refactor

Refactoring is not a scheduled activity you block out calendar time for — it's a response to specific triggers, folded into other work:

- **Preparatory refactoring** — before adding a feature, if the existing structure makes the feature awkward to add, reshape the structure first so the feature becomes a natural, easy addition. This is often the single highest-leverage moment to refactor: it's far easier to see the right structure once you know exactly what new capability needs to fit into it.
- **Comprehension refactoring** — when you have to read confusing code to understand it, refactor it into a form that reflects your understanding as you build it, rather than just holding the understanding in your head. If you decide the change isn't worth keeping, it's fine to discard it once it's served its purpose of building your understanding — but if it clarified things for you, it will clarify them for the next reader too, so lean toward keeping it.
- **Litter-pickup refactoring** — when you notice something suboptimal while working nearby for an unrelated reason, make it a little better before you leave, even if you don't have time to make it ideal. Leave the code slightly better than you found it.
- **Refactoring in code review** — when reviewing someone else's change, use the review as an opportunity to suggest or make small structural improvements while the change is still fresh and fully in mind.
- **The Rule of Three** — the first time you do something, just do it. The second time you do something similar, wince at the duplication but do it anyway. The third time, refactor to remove the duplication. This isn't a rigid count so much as a reminder that some duplication is fine short-term, and that the point where it becomes worth fixing arrives sooner than perfectionist instinct suggests, but not on the very first occurrence.

## When *not* to refactor

- **If the code doesn't need to be touched and isn't confusing you right now**, leave it. Refactoring exists to make *future* work in that code easier — code you have no reason to change or understand doesn't need that investment yet.
- **If it would be faster to rewrite than to refactor**, rewrite. This is rare, but a sufficiently tangled piece of code with poor test coverage and no one who understands it can cost more to incrementally untangle than to replace outright. This is a judgment call, not a default.
- **If you're close to a deadline and the refactor isn't required to ship**, defer it — but defer it *explicitly*, as a recorded decision, not by silently walking away. If this project uses the Spec-Driven Workflow's Technical Debt Management process, log the deferred refactor there with its location, reason, and impact, so it doesn't just quietly become permanent.
- **If the code has no test coverage**, don't refactor it yet — see Scenario 2 above. Add characterization tests first.

## The discipline that makes refactoring safe

This is the part that separates "controlled and efficient" refactoring from a risky rewrite in disguise:

1. **Take the smallest step that's still meaningful.** Each named refactoring below has its own mechanical steps — follow them individually rather than doing the whole refactoring in your head and only externalizing the final result. A refactoring you can't quickly verify was correct is a sign you took too large a step, not a sign to push through.
2. **Run the tests after every mechanical step**, not just at the end of the whole refactoring. If a step breaks a test, you now know exactly which small change caused it, and can undo just that one step.
3. **Commit after every step that leaves the suite green.** This is the same small-increment discipline this project's commit conventions already require (see `.github/copilot-instructions.md`'s commit message convention) — apply it *inside* a refactoring, not just at the end of one. A refactoring that took eight mechanical steps should usually produce eight small commits, not one large one, specifically so that if something is later found to be wrong, the rollback has a small, easy-to-reason-about surface area rather than reverting an entire redesign at once. Use `refactor` as the commit type, and let the message describe the specific technique applied (e.g. `refactor(invoicing): extract CalculateDiscount from CalculateTotal`).
4. **If you get lost, revert to the last commit rather than trying to debug your way out.** A refactoring session that goes wrong is cheap to recover from exactly because of the small commits from step 3 — that's the entire reason they're worth making.
5. **Never mix a refactoring step with a behavior change**, per the Two Hats rule above. If a test fails during what was supposed to be a refactor and the fix requires actually changing behavior, stop, revert to the last good commit, and start that piece over as a deliberate, separate change.

## Recognizing when to refactor — code smells

A code smell is a surface indication that usually corresponds to a deeper structural problem — not a rigid rule, a prompt to look closer and decide whether action is warranted now. The full catalog with detailed descriptions is in `references/code-smells-catalog.md`; the essentials:

| Smell | What it looks like | Typical fix |
|---|---|---|
| Duplicated Code | The same or near-identical structure appears in more than one place | Extract Method, Pull Up Method, Extract Class |
| Long Method | A method that keeps growing and doing more | Extract Method, Replace Conditional with Polymorphism |
| Large Class | A class trying to do too much, with too many fields/methods | Extract Class, Extract Subclass |
| Long Parameter List | A method signature that's hard to read or call correctly | Introduce Parameter Object, Preserve Whole Object |
| Divergent Change | One class is edited for many unrelated reasons | Extract Class, per reason |
| Shotgun Surgery | One change requires edits scattered across many classes | Move Method, Move Field, Inline Class |
| Feature Envy | A method is more interested in another class's data than its own | Move Method |
| Data Clumps | The same small group of fields/parameters travels together everywhere | Extract Class, Introduce Parameter Object |
| Primitive Obsession | Primitives standing in for a concept that deserves its own type | Replace Data Value with Object, Replace Type Code with Class |
| Switch Statements / Repeated Conditionals | The same type-check or switch, repeated at multiple call sites | Replace Conditional with Polymorphism |
| Lazy Class / Lazy Element | A class or method that isn't earning its structural overhead | Inline Class, Inline Method |
| Speculative Generality | Abstraction built for a future need that hasn't arrived | Collapse Hierarchy, Inline Class, remove unused parameters |
| Message Chains | `a.getB().getC().getD()...` | Hide Delegate, Extract Method |
| Middle Man | A class that only forwards calls to another | Remove Middle Man, Inline Method |
| Inappropriate Intimacy | Two classes reach into each other's internals too much | Move Method, Move Field, Change Bidirectional to Unidirectional Association |
| Data Class | A class with only getters/setters and no behavior | Move Method (bring behavior in), Encapsulate Field |
| Refused Bequest | A subclass uses almost none of what it inherits | Replace Inheritance with Delegation, Push Down Method |
| Comments (as a smell) | A comment exists to explain unclear code | Extract Method, Rename Method — write the comment as code instead |

## The mechanics catalog

`references/refactoring-mechanics-catalog.md` has the full, step-by-step, C#-adapted mechanics for the most broadly useful named refactorings — Extract Method, Inline Method, Extract Variable, Rename Method, Move Method, Move Field, Extract Class, Replace Conditional with Polymorphism, Replace Nested Conditional with Guard Clauses, Introduce Null Object, Introduce Parameter Object, Replace Magic Number with Symbolic Constant, Consolidate Duplicate Conditional Fragments, and Introduce Assertion — each in the Motivation / Mechanics format below, adapted from Fowler's own template. `Extract Method`, `Inline Method`, `Move Method`, `Extract Interface`, and `Method Object` are also referenced from the TDD skill's Refactor section; this catalog is their authoritative, detailed source. The TDD skill's own `references/refactoring-patterns-catalog.md` retains only `Reconcile Differences`, `Isolate Change`, and `Migrate Data` — three TDD-specific techniques for evolving code while remaining continuously Green that aren't part of Fowler's catalog under those names, and are complementary to, not a duplicate of, what's here.

**Recipe format**, matching Fowler's own:
- **Summary** — what the refactoring does and the shape of the before/after change.
- **Motivation** — why you'd reach for this one, and what smell it typically addresses.
- **Mechanics** — the exact small steps, in order, with the reminder to compile and run tests after each one that changes behavior-adjacent structure.

## A note on `Replace Error Code with Exception` and this project's `Result`/`Error` convention

Fowler's book (1999) treats throwing an exception as the modern alternative to an ambiguous numeric or sentinel return code — the point of that refactoring is eliminating an *undocumented, easy-to-ignore* failure signal, not mandating exceptions specifically over every other alternative. If this project has an established `Result`/`Error` convention for predictable outcomes (see the Validation Strategy section of `.github/copilot-instructions.md`, if present), treat that convention as satisfying the same underlying goal this refactoring is aimed at: replacing a magic/ambiguous return value with an explicit, impossible-to-ignore failure representation. Do not use this refactoring to argue for exceptions over an established `Result` type in a codebase that has deliberately chosen `Result` for a category of outcome — apply whichever explicit, honest signal this project's own conventions already call for.

## Applying this to legacy or high-debt code specifically

- Don't attempt to refactor a whole file or module in one sitting. Pick the smell that's actually blocking the work you're doing right now, fix that one thing with the smallest applicable technique, commit, and stop — the rest of the debt will still be there next time, recorded rather than rushed.
- Where this project uses the Spec-Driven Workflow's Technical Debt Management process, log any smell you noticed but didn't fix using its Auto-Issue-Creation template, including location and the specific smell name from the catalog above, so it's discoverable later rather than forgotten.
- Prefer the smallest fix that removes the smell over the most elegant possible redesign — elegance is not the goal, removing the specific structural problem you identified is. A larger redesign, if warranted, is its own deliberate, separately-planned piece of work.

## Quick-reference checklist

- [ ] Am I refactoring, or am I actually changing behavior? (Two Hats — pick one.)
- [ ] Does the code I'm about to touch have adequate test coverage? If not, write characterization tests first.
- [ ] Have I named the specific smell driving this refactoring, and picked the technique the catalog above maps to it?
- [ ] Am I following that technique's mechanical steps individually, rather than doing the whole thing in my head?
- [ ] Did I run the full test suite after each mechanical step, not just at the end?
- [ ] Did I commit after each step that stayed green, with a `refactor(...)` message naming the specific technique?
- [ ] If something went wrong, did I revert to the last good commit rather than debugging forward?
- [ ] Did I avoid mixing this refactor with any bug fix or new behavior?
