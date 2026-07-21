# TDD-Specific Refactoring Techniques

**General-purpose refactorings live elsewhere.** `Extract Method`, `Inline Method`, `Move Method`, `Extract Interface`, `Method Object`, and the full mechanics catalog for restructuring code are authoritatively covered in the companion `.github/skills/refactoring/SKILL.md` skill and its `references/refactoring-mechanics-catalog.md`. This file intentionally does not duplicate them.

What's kept here are three techniques that are specifically about *sequencing a change relative to a running test suite* during active TDD — a slightly different concern from the general mechanics catalog, and not named this way in Martin Fowler's refactoring catalog. All examples are original C# illustrations.

## Reconcile Differences

**When:** two pieces of code do almost the same thing, and you want to unify them, but they aren't identical yet.

**How:** don't merge on faith. Incrementally change one implementation to structurally resemble the other — same variable order, same intermediate steps, same shape — running the suite after each small change, until the two are line-for-line identical. Only then delete one and point every caller at the other.

This is slower than "these look basically the same, let me just combine them," but every intermediate step stays independently verifiable against the existing tests, which is exactly what keeps a big consolidation safe.

## Isolate Change

**When:** you need to modify one part of a method or class, and want to protect the rest of it from disturbance while you work.

**How:** use `Extract Method` first (see the Refactoring skill), purely to carve out just the part you're about to change, even if that extracted piece is small. Make your change inside the newly isolated method. If, once finished, the extraction turns out to have been unnecessary ceremony for a change that was simpler than expected, it's fine to `Inline Method` it back — the isolation was a scaffold for safety, not a permanent structural statement.

## Migrate Data

**When:** moving from one internal representation to another — e.g. a single nullable field becoming a collection, or a primitive becoming a value object — where switching over in one large step would break every test until the whole migration is finished.

**How:**
1. Add the new representation alongside the old one, initially kept in sync by whichever code currently writes the old one.
2. Migrate readers to the new representation one at a time, running the suite after each.
3. Once nothing reads the old representation, migrate the remaining writers to populate only the new one.
4. Delete the old representation and remove any now-unnecessary sync code.

```csharp
// Step 1 — old field kept, new field added and kept in sync
public sealed class Invoice
{
    private decimal _totalAmount;              // old representation
    private readonly List<InvoiceLine> _lines = []; // new representation

    public void AddLine(InvoiceLine line)
    {
        _lines.Add(line);
        _totalAmount = _lines.Sum(l => l.Total); // kept in sync during migration
    }
}

// Step 4 — old field and sync code removed once every reader uses _lines
public sealed class Invoice
{
    private readonly List<InvoiceLine> _lines = [];

    public decimal TotalAmount => _lines.Sum(l => l.Total);

    public void AddLine(InvoiceLine line) => _lines.Add(line);
}
```

Each intermediate state above passes the full existing test suite — that's the entire point of doing it in steps rather than one large rewrite.
