---
name: test-driven-development
description: >-
  Guides the Red-Green-Refactor Test-Driven Development workflow: building a
  test list, writing one failing test at a time, using Fake It / Triangulate /
  Obvious Implementation to reach Green with the least code, and refactoring
  deliberately using named, mechanical techniques. Use when asked to "write
  this using TDD", "practice test-driven development", "red-green-refactor",
  "write the test first", or when starting a new class, method, or feature
  that should be built test-first.
metadata:
  author: project-team
  version: "2.0"
  source: >-
    Harmonizes https://xunit.net/docs/tdd-example with Kent Beck's
    "Test-Driven Development By Example" (Addison-Wesley, 2002), Part III.
---

# Test-Driven Development — Red / Green / Refactor

## When this skill applies

Use this workflow whenever a new class, method, or behavior is being built test-first — not when retrofitting tests onto already-written code (that's characterization testing, a different activity). If the codebase has its own testing conventions (assertion library, test naming pattern, mocking library), this skill governs the *process*; project-specific conventions (e.g. in `.github/copilot-instructions.md`) still govern naming, tooling, and structure. Where this skill's illustrative examples use a naming style that differs from the project's, follow the project's convention.

## The core loop

TDD has exactly two states — named in the order the slogan uses them, because that's the order you actually visit them in:

- **Red** — the code doesn't compile, or at least one test fails. This is where every new behavior starts: you cannot be adding real value in the Green state, because by definition nothing new is being demanded of the code yet.
- **Green** — every test passes (and, trivially, having zero tests and zero production code also counts as Green — the state you're in before the very first test of a session).

**While Red, there is only one legal move:** change the production code until you're back to Green. Never change a test's expectations to make a failure go away, and never add a workaround that doesn't actually implement the behavior.

**While Green, there are exactly two legal moves:**
1. **Write a new test** expressing a behavior that doesn't exist yet. This is how you *leave* Green and re-enter Red — you're describing a requirement the code doesn't satisfy yet. If a new test stays Green immediately, pause: either the behavior was already implemented (fine, just recording additional coverage), or an earlier step over-implemented ahead of any test that demanded it.
2. **Refactor** — restructure the *internal* implementation without changing *observable* behavior, while staying Green throughout. See the expanded Refactor section below — this move deserves as much deliberate technique as the other two.

**Never touch tests and production code in the same step.** A single step is either "add/adjust one test, then make production code changes to satisfy only that test" (Red → Green), or "restructure code with tests unchanged" (Green → Green). Mixing the two in one step removes your ability to tell which change caused a result.

## Step 0 — Build a Test List before writing any code

Before the first test, write a plain-language list of every behavior, operation, and edge case you know you'll eventually need — including, explicitly, the "null" or empty-input version of any operation that doesn't exist yet, and any refactorings you already suspect you'll need before the code is clean. This list is the design artifact, and it's expected to grow as you notice new cases mid-implementation: when a tangential idea occurs to you while working on something else, write it on the list and return immediately to what you were doing — don't chase it on the spot.

Order the list from simplest to most complex, but don't force a strict top-down or bottom-up reading of it — a program built this way tends to grow from what you already know toward what you don't, in whichever direction that happens to point. Finishing the list is the working definition of "done" for the unit. A test still on the list at the end of a session stays on the list for next time; a refactoring you've decided is out of scope for now moves to a separate "later" list — but never move a test there. If you can imagine a test that might not pass, making it pass is more important than moving on.

**Picking the next test (`One Step Test`):** choose whichever item on the list will teach you something *and* that you're confident you can get working quickly. Looking down the list, most items will feel either "obviously already handled" or "I have no idea" — the one to implement next is the one that isn't obvious but you're still confident about.

**Picking the very first test (`Starter Test`):** for a brand-new unit, pick the smallest possible input and the most trivial plausible output — an empty collection, a single item, a no-op case. The goal is to get *something* running in the first couple of minutes so the Red-Green-Refactor loop has started; realism and edge cases come from later tests on the list, not the first one.

## RED — writing the test first

Write the test as though the production code already exists and already works. This forces you to design the API from the caller's point of view, which is one of TDD's most valuable side effects as a design technique, independent of the testing benefit.

**Work backward from the assertion (`Assert First`).** When starting a new test, write the final `Assert`/`Should` statement first, then work backward to figure out what values and setup would make that assertion meaningful. Writing a test forces several decisions at once — where the behavior belongs, what to name things, what the right answer is, and how to check it — and pea-sized brains do a better job solving them one at a time. Starting from the assertion isolates "what's the right answer, and how do I check it?" from everything else, and you can adjust names and setup afterward once real usage clarifies them.

**Tests must not affect each other (`Isolated Test`).** A test's pass/fail result should never depend on which other tests ran, or in what order. This is what makes tests composable, safely runnable as a subset, and — most importantly — what makes a pile of failures mean a pile of independent facts about the system rather than one early failure cascading into a wall of noise. If two tests need setup so elaborate that separating it feels like too much work, that's usually a sign the design itself needs breaking into smaller, more independent pieces — not a reason to let tests share state.

**Choose data that reads as a story (`Test Data` / `Evident Data`).** Test data exists to communicate to a future reader, not just to exercise a code path. Never reuse the same value to mean two different things in the same test — if the first argument is `3`, don't also make the second argument `3` unless the test is specifically about equal values; use `3` and `4` so a reader (and the implementation) can't quietly get the arguments backward without a test catching it. Where a numeric relationship exists between input and expected output, write the assertion so that relationship is visible in the assertion itself (e.g. assert against `100 * 0.5m * (1 - 0.015m)` rather than a pre-computed `49.25m`) — this doubles as a hint for what the implementation actually needs to compute.

## GREEN — the three ways to make a test pass

Once Red, there are three named strategies for getting back to Green. None is universally "correct" — picking between them is itself a judgment call, and it's fine to use different ones for different tests in the same session.

**`Fake It` — return a constant, then generalize.** When you don't yet know the cleanest implementation, hard-code the exact expected value and get Green immediately. This is not a shortcut to feel guilty about — a hard-coded return value is a legitimate, deliberate first step, because having something running (even something obviously wrong) gives you a stable place to think from, and it keeps you from prematurely solving problems you don't have yet. The constant survives only until a *second* test, with a different expected value, makes it impossible to satisfy both with one hard-coded return — at that point the duplication between the test's expectation and the fake return value is what forces a real implementation.

```csharp
// First test forces a bare minimum
[Fact]
public void Sum_TwoNumbers_ReturnsTotal()
{
    Assert.Equal(4, Calculator.Sum(3, 1));
}

private static int Sum(int a, int b) => 4; // Fake It — deliberately, not an accident
```

**`Triangulate` — generalize only once two examples force it.** Add a second test whose expected result couldn't be satisfied by the same hard-coded value, and let the two together justify the general implementation:

```csharp
[Fact]
public void Sum_TwoNumbers_ReturnsTotal()
{
    Assert.Equal(4, Calculator.Sum(3, 1));
    Assert.Equal(7, Calculator.Sum(3, 4));
}

private static int Sum(int a, int b) => a + b; // now the general form is justified
```

Triangulate is the most conservative way to drive an abstraction — use it specifically when you're genuinely unsure what the right general implementation looks like. It's not a technique to lean on for every method; for anything you're already confident about, skip straight to the next strategy.

**`Obvious Implementation` — just write the real thing.** For operations you're confident you already know how to implement correctly, write the real implementation directly rather than manufacturing an artificial intermediate step. If you notice you're being surprised by unexpected Red results when you thought you were writing something obvious, that's the signal to downshift back to `Fake It`/`Triangulate` and take smaller steps — don't grind through repeated surprises hoping the next attempt is finally right.

**`One to Many` — implement the single-item case before the collection case.** When an operation needs to eventually work over a collection, get it working for one item first, then widen the parameter to a collection while keeping the test passing (an `Isolate Change` step — see Refactor below), then implement the real iteration, then delete anything left over from the single-item version.

See `references/worked-example-stack.md` for these three strategies applied in sequence across a complete, growing example, with each step explicitly labeled by which strategy it uses.

## REFACTOR — restructuring with tests as your safety net

Refactor when you notice duplication, unclear naming, or structure that will make the next behavior harder to add — not on a fixed schedule, and not "just in case." This section is deliberately as detailed as Red and Green above it: refactoring is not an afterthought tacked onto the end of the loop, it's the step where the design actually gets *good*, and it deserves the same level of technique.

**This step has its own dedicated companion skill.** `.github/skills/refactoring/SKILL.md` is the authoritative source for *what's wrong* (a full code-smell catalog) and *exactly how to fix it* (a step-by-step mechanics catalog) — use it together with this section whenever you reach the Refactor step. This section states the TDD-specific discipline around *when* and *how carefully* to refactor inside the loop; the Refactoring skill supplies the diagnosis and the technique.

**What "refactoring" means specifically inside TDD.** Ordinarily, refactoring means a change that can never alter behavior under any circumstances. Inside TDD, the standard is narrower and more honest: a change is a refactoring if it doesn't change which of your *current* tests pass. That's a real, useful definition — but it puts the burden on you to keep writing enough tests that "passes all tests I currently have" and "preserves all real behavior" stay close to the same statement. If you ever think "I know there's a problem here, but the tests all still pass" — that's not permission to proceed, it's an instruction to write the test that would catch the problem.

**The Two Hats.** At any moment you are either adding function (writing a new test, then production code to pass it) or refactoring (restructuring with tests unchanged) — never both at once. This is the same rule as "never touch tests and production code in the same step" above, restated from the Refactoring skill's own framing, because it's worth hearing twice: if you catch yourself wanting to fix a bug or add a check *while* refactoring, finish the refactor, commit, then deliberately switch hats.

**Run the whole suite after every refactor.** If anything fails, you didn't refactor — you changed behavior. Revert, and either recover the previous structure or acknowledge you're making a deliberate behavior change and start over from a new test.

**Commit after every green step — new test passing or refactor completed.** Follow this project's commit message convention (`.github/copilot-instructions.md`) and use `refactor` as the type for a Refactor-phase commit, naming the specific technique applied. A TDD session that runs ten cycles should typically produce something like ten small commits, not one large one at the end — this keeps rollback surface small if a later step turns out to be wrong, matching the same small-increment discipline the Refactoring skill applies to its own mechanical steps.

### Named refactoring techniques

Two categories exist, and they live in two different places on purpose:

- **General-purpose refactorings** (`Extract Method`, `Inline Method`, `Move Method`, `Extract Interface`, `Method Object`, and many more) are **not duplicated here** — their full mechanics live in `.github/skills/refactoring/SKILL.md` and its `references/refactoring-mechanics-catalog.md`. Use that skill as the reference the moment you're inside this Refactor step.
- **TDD-specific techniques below** are for evolving code while remaining continuously Green during active development — they're about *when and how to sequence* a change relative to the test suite, which is a slightly different concern than the general mechanics catalog:

- **`Reconcile Differences`** — to unify two pieces of code that do almost the same thing, incrementally change one to look more like the other, and only merge them once they're identical. Don't jump straight to "these are basically the same" and merge on faith — the incremental convergence is what keeps each step safely verifiable against the test suite.
- **`Isolate Change`** — before changing one part of a method or object, first extract just that part (typically via `Extract Method`) so the rest is undisturbed while you work. If, once isolated, the change turns out trivial, it's fine to inline the extraction back — don't leave permanent ceremony behind for a change that didn't need it.
- **`Migrate Data`** — when moving from one internal representation to another (e.g. a single field to a collection, as in `One to Many` above), temporarily maintain both the old and new representation side by side, switch all reads over to the new one, and only then delete the old one. This keeps every intermediate step passing the existing tests, rather than one large jump that breaks everything until it's entirely finished.

### The discipline that makes refactoring safe

- Refactor in the smallest steps that are still meaningful — a refactor you can't get back to Green from quickly is a sign you took too large a step, not a sign to push through.
- Never refactor and add behavior in the same step, even when the temptation is strong because "you're already in there."
- If a refactor turns out to require a genuinely large internal rewrite (e.g. replacing a placeholder with real backing storage), it's legitimate to do that rewrite as its own Green→Green step, with the existing suite passing throughout, *before* writing the next test that was going to demand it anyway — the new test then goes from Red to Green with a much smaller, more targeted change on top of an already-solid foundation.

## Designing error conditions is a real design decision, not an afterthought

When a behavior has a failure case (e.g., "what happens if you operate on an empty collection?"), treat the choice of *how* to signal failure as a first-class design decision with real trade-offs — don't default to "just throw" without considering the alternatives:
- **Throwing an exception** — appropriate when the condition represents a genuine misuse the caller could reasonably have avoided, and when the cost of an exception (allocation, stack unwinding) is not a concern in a hot path.
- **A nullable/optional return** — awkward if the successful return type could itself legitimately be absent-like (e.g. `null` is already a meaningful value for a reference type).
- **A `TryX(out result)` pattern, or a `Result`/`Error`-style return value** — better when the failure is a predictable, expected outcome of ordinary usage rather than caller misuse.

If this project already has an established convention for this decision (for example, a documented "could an ordinary caller trigger this through normal usage?" test in `.github/copilot-instructions.md`), apply that convention rather than re-deciding case by case.

**Testing a thrown exception (`Exception Test`).** Whichever failure signal you choose, write a test that pins down the *exact* contract, not just "something goes wrong":

```csharp
[Fact]
public void FindRate_UnknownCurrencyPair_ThrowsArgumentException()
{
    var exchange = new Exchange();

    Assert.Throws<ArgumentException>(() => exchange.FindRate("USD", "GBP"));
}
```

Assert the *specific* exception type your test expects — never a bare `catch` that swallows anything. A helper or collection type you delegate to internally may throw a different, more generic exception than the one you intend as your own type's public contract (an out-of-range index exception leaking through instead of your own domain-specific one, for example); a precise assertion is what catches that leak.

## Test organization by shared context

When you notice that many tests share the same setup (e.g. "given an empty collection," "given a collection with several items"), group them into nested test classes, one per shared context, with the shared setup in each nested class's constructor. This makes it easy to see, at a glance, what's tested for each state, and gives you a natural checklist when a new feature is added: revisit each existing context and ask what the new behavior should do there. See `references/worked-example-stack.md` for a worked example of this organization.

## When to delete a test

More tests are generally better, but redundant tests aren't automatically worth keeping. Judge a candidate for deletion against two criteria: **confidence** — never delete a test if doing so would reduce your confidence that the behavior actually works — and **communication** — if two tests exercise the same code path but tell a different story to a future reader, keep both. Only when a test is redundant on *both* counts is it safe to remove.

## Quick-reference checklist

- [ ] Do I have a Test List? Is the next item something I'm confident about but that isn't already obviously handled?
- [ ] Did I write the test before any production code for this behavior, working backward from the assertion?
- [ ] Is my test data meaningful and non-repeating — would swapped arguments still be caught?
- [ ] Did I watch it fail (or fail to compile) before implementing anything?
- [ ] For this test, am I using Fake It, Triangulate, or Obvious Implementation — and is that the right one for how confident I am?
- [ ] Did I run the *whole* suite, not just the new test?
- [ ] Am I about to change tests and production code in the same step? If yes, stop and split it.
- [ ] Is there duplication or unclear structure worth refactoring right now, with tests unchanged — and do I know which named technique applies (see the Refactoring skill)?
- [ ] Did I commit this Red→Green or Green→Green step before moving to the next one?
- [ ] For any failure case: have I deliberately chosen throw vs. nullable vs. Try-pattern vs. Result, and does a test pin down the exact contract?
- [ ] Are my tests isolated from each other, and organized by shared context if there are several?

See `references/worked-example-stack.md` for a full, step-by-step walkthrough of this loop, with each Green-phase step explicitly labeled Fake It / Triangulate / Obvious Implementation. For the full mechanical steps of general-purpose refactoring techniques, see `.github/skills/refactoring/SKILL.md` and its catalogs; `references/refactoring-patterns-catalog.md` in this skill covers only the three TDD-specific techniques (`Reconcile Differences`, `Isolate Change`, `Migrate Data`) not found there.
