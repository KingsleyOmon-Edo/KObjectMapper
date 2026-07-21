# Code Smells Catalog

A code smell is a symptom, not a diagnosis — it tells you where to look, not automatically what to do. Use judgment about whether a given instance is worth fixing now, per the "When to refactor" / "When not to" guidance in the main `SKILL.md`.

## Duplicated Code

The most common and costly smell. If the same expression appears in two places, any future change to the logic has to be found and applied twice, and the two copies will eventually drift out of sync. Identical code in two methods of the same class: use `Extract Method`, then call it from both places. Identical code in two sibling subclasses: `Extract Method` in both, then `Pull Up Method` to the shared superclass. Similar-but-not-identical code: use `Reconcile Differences` (see the TDD skill's refactoring catalog) to make the two versions identical first, *then* extract. Duplicated code between entirely unrelated classes: `Extract Class` to give the shared logic a home, and have both original classes use it.

## Long Method

Short methods live longer. A long method accumulates responsibilities, becomes hard to name honestly, and hides its own structure. The single biggest lever for reducing this smell is aggressive use of `Extract Method` — if you have to scroll to read a method, look for a cohesive chunk with a name you could give it, and pull it out. Don't be deterred by a chunk that references several local variables; `Introduce Parameter Object` or `Extract Class`/`Method Object` handle that case. Long, deeply nested conditional logic inside a method is itself often the root cause — see Switch Statements and the Guard Clause refactoring below.

## Large Class

The class-level version of Long Method: a class trying to do too much accumulates fields and methods until no one can hold the whole thing in their head at once. Look for fields that are only used by a subset of the class's methods — that subset is often a class waiting to be extracted (`Extract Class`). If a class inherits behavior it doesn't fully use, `Extract Subclass` for the specialized part.

## Long Parameter List

A method that takes many parameters is hard to call correctly and hard to read at the call site. Prefer deriving a value from another object you already have (`Replace Parameter with Method Call`) or bundling parameters that naturally travel together into their own type (`Introduce Parameter Object`). A long parameter list is very often the same group of primitives showing up together across many methods — see Data Clumps below.

## Divergent Change

One class that gets modified in different, unrelated ways for different reasons — a change to the tax calculation and, separately, a change to how invoices are formatted both land in the same class. This is a sign the class has more than one responsibility hiding inside it. `Extract Class` to separate the reasons for change, so each resulting class only changes for one reason.

## Shotgun Surgery

The inverse smell: one conceptual change requires touching many different classes and methods scattered across the codebase. This is harder to detect than Divergent Change and more dangerous, because it's easy to miss one of the scattered spots. `Move Method` and `Move Field` to bring the scattered pieces back together into one place; if a whole small class exists only to forward to another, `Inline Class` it away.

## Feature Envy

A method that spends most of its logic reaching into another object's data (via getters) rather than its own. The method is, structurally, more interested in the other class than its own — `Move Method` to relocate it there. When a method uses several features of two different classes, move it to whichever class owns the majority of what it needs, or split it so each part lives with the data it actually uses.

## Data Clumps

The same small cluster of fields or parameters (e.g. a start/end pair, or an amount/currency pair) reappearing together, over and over, across classes and method signatures. A useful test: if you deleted one of the fields, would the others still make sense on their own? If not, they're a clump, and deserve to become their own class (`Extract Class`) — which then also simplifies every method that previously took them as separate parameters (`Introduce Parameter Object`).

## Primitive Obsession

Using a raw primitive (a `string`, an `int`, a `decimal`) to represent something that is really a domain concept with its own rules — a currency amount, a phone number, a range. This under-uses the type system: nothing stops a raw `decimal` meant to be a percentage from being passed where a raw `decimal` meant to be a currency amount was expected. `Replace Data Value with Object` gives the concept its own type; `Replace Type Code with Class` does the same for values that are really a small closed set of named options (see also Switch Statements below).

## Switch Statements / Repeated Conditionals

A single `switch` (or long `if`/`else if` chain) on a type code is a code smell if the *same* switch, on the *same* type code, appears at more than one call site — each new case then has to be found and added in every location. `Replace Conditional with Polymorphism` turns the cases into subclasses (or, in modern C#, a smart-enum/strategy pattern) so adding a new case means adding one new type, not editing every switch. A single switch that appears exactly once, and is unlikely to grow new cases, is not automatically a smell — don't over-apply this fix to code that doesn't actually have the duplication problem it exists to solve.

## Lazy Class / Lazy Element

A class, method, or other structural element that no longer earns the overhead of existing separately — perhaps a class that was going to grow but never did, or a method reduced by earlier refactoring to a single trivial line. `Inline Class` or `Inline Method` to fold it back into wherever it's used, and simplify.

## Speculative Generality

Abstraction, parameters, or hook methods built to accommodate an imagined future need that hasn't materialized. Unlike Lazy Class (which shrank over time), this was over-built from the start. Unused abstract methods, unused parameters, and hierarchies with only one real subclass are the usual signs. `Collapse Hierarchy`, `Inline Class`, and removing unused parameters bring the code back down to only what current, real requirements justify. The exception is genuinely reusable library or framework code being deliberately designed for extension — this smell applies to application code carrying speculative flexibility it doesn't yet need.

## Temporary Field

A field that's only given a value in certain circumstances, and is otherwise unused or `null` — readers have no way to know, just by looking at the class, when it's safe to read that field. `Extract Class` to give the temporary field (and the logic that uses it) its own home, where its lifecycle is obvious from the smaller class's own construction and methods.

## Message Chains

A chain like `order.Customer.Address.PostalCode.Country` couples the caller to the entire intermediate structure — any change to how a `Customer` stores its `Address` breaks every caller with a chain like this. `Hide Delegate` — give the intermediate objects a method that does the forwarding, so callers ask for what they want directly rather than navigating the path themselves.

## Middle Man

The opposite extreme from Message Chains: a class where most methods just delegate to another object, adding no behavior of its own. Some delegation is legitimate (see Hide Delegate above), but when *most* of a class's methods are pure forwarding, `Remove Middle Man` and let callers talk to the real object directly.

## Inappropriate Intimacy

Two classes that reach into each other's private fields and internal methods far more than a clean boundary between them should allow. `Move Method`/`Move Field` to relocate the offending members to wherever they're actually used most, or `Change Bidirectional Association to Unidirectional` if the two-way relationship isn't genuinely needed both ways.

## Alternative Classes with Different Interfaces

Two classes that do the same job but expose it under different method names or signatures, usually because they were written independently rather than to a shared contract. `Rename Method` to align their public members, then `Extract Interface` (or `Extract Superclass` if they share more than just the interface) so callers can treat them interchangeably.

## Data Class

A class that's nothing but fields with getters and setters — all data, no behavior. This is often a sign that logic which *should* live on the class has instead been scattered across other classes that manipulate its data from outside. Look at where its fields are used from the outside and consider `Move Method` to bring that behavior in; encapsulate any publicly mutable field.

## Refused Bequest

A subclass that inherits from a base class but only actually uses a small fraction of what it inherits — a sign the inheritance relationship doesn't really reflect an is-a relationship. If the subclass genuinely doesn't want most of the base class's contract, `Replace Inheritance with Delegation` — hold a reference to an instance of the base type instead of inheriting from it, and forward only the members that are actually wanted.

## Comments

Fowler is explicit that comments themselves aren't the smell — a comment used to explain *why* something non-obvious is done is valuable, and this project's own conventions (see `.github/copilot-instructions.md`) already require exactly that kind of comment. The smell is a comment that exists to compensate for code that isn't clear enough to explain itself — a comment describing *what* a block of code does is often a sign that block deserves `Extract Method` with a name that says the same thing the comment was saying, so the comment becomes unnecessary rather than being maintained forever alongside code it describes.
