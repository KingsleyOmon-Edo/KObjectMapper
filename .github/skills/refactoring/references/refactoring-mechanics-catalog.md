# Refactoring Mechanics Catalog

Each entry follows Fowler's own format: **Summary** (what changes shape), **Motivation** (why, and what smell it addresses), **Mechanics** (the exact small steps). All code below is original, written fresh in C# to illustrate the technique — not translated line-for-line from any source. Run the test suite after every mechanical step marked with a ▶, not just once at the end.

## Extract Method

**Summary:** turn a fragment of a method into its own, well-named method, and call it from where the fragment used to be.

**Motivation:** the most frequently used refactoring in the whole catalog. Use it to eliminate duplication between two methods, or simply to make a method's own structure easier to read by giving each part a name. A method should do one thing; if you can draw a box around a few lines and give the box a name, that's a candidate.

**Mechanics:**
1. Create a new method, named after *what* the extracted code does, not *how* it does it.
2. Copy the extracted fragment into the new method.
3. Check for local variables from the original method that the fragment uses — pass them in as parameters.
4. Check whether the fragment assigns to a local variable that's used *after* the fragment in the original method — if so, that value needs to become the new method's return value.
5. Replace the original fragment with a call to the new method. ▶ Compile and run tests.

```csharp
// Before
public decimal CalculateInvoiceTotal(Invoice invoice)
{
    Console.WriteLine($"Invoice for {invoice.CustomerName}");
    Console.WriteLine("------------------------");

    decimal subtotal = invoice.Lines.Sum(l => l.UnitPrice * l.Quantity);
    return subtotal * 1.2m; // add tax
}

// After
public decimal CalculateInvoiceTotal(Invoice invoice)
{
    PrintHeader(invoice.CustomerName);
    decimal subtotal = invoice.Lines.Sum(l => l.UnitPrice * l.Quantity);
    return ApplyTax(subtotal);
}

private static void PrintHeader(string customerName)
{
    Console.WriteLine($"Invoice for {customerName}");
    Console.WriteLine("------------------------");
}

private static decimal ApplyTax(decimal subtotal) => subtotal * 1.2m;
```

## Inline Method

**Summary:** replace a call to a method with the method's own body, then delete the method.

**Motivation:** the reverse of `Extract Method` — used when a method's body is exactly as clear as its name (so the indirection adds nothing), or when a chain of small methods has become harder to follow than the logic it wraps and you want to flatten it before re-abstracting differently.

**Mechanics:**
1. Check the method isn't polymorphic (overridden in a subclass) — if it is, don't inline it.
2. Find every call site.
3. Replace each call site with the method's body, adjusting parameter names to match the arguments actually passed at that call site.
4. ▶ Compile and run tests after each call site is replaced.
5. Once every call site is replaced, delete the method.

## Extract Variable

**Summary:** give a complex or non-obvious expression a name by assigning it to a local variable first.

**Motivation:** an expression that's doing several things at once (e.g. combining a discount check with a threshold comparison) is hard to read in place. Naming the intermediate result documents what it means without needing a comment.

**Mechanics:**
1. Confirm the expression has no side effects that would change if evaluated at a different point.
2. Declare a new local variable, assign the expression to it, and give it a name that explains what it represents.
3. Replace the original expression with the new variable. ▶ Compile and run tests.

```csharp
// Before
if (order.Quantity * order.ItemPrice - Math.Max(0, order.Quantity - 500) * order.ItemPrice * 0.05m > 10000m)
{
    // ...
}

// After
decimal basePrice = order.Quantity * order.ItemPrice;
decimal quantityDiscount = Math.Max(0, order.Quantity - 500) * order.ItemPrice * 0.05m;
if (basePrice - quantityDiscount > 10000m)
{
    // ...
}
```

## Rename Method (Rename Symbol)

**Summary:** change the name of a method, field, or variable to better communicate its purpose.

**Motivation:** a name is the cheapest, most valuable piece of documentation a piece of code has. If you find yourself wanting to add a comment explaining what something does, try renaming it to say that instead — a good name outlives a comment because it can't silently drift out of sync with the code.

**Mechanics:**
1. Use the IDE/tooling's rename-refactoring feature rather than manual find-and-replace, so every reference (including in other files) updates consistently and no string literal or comment is accidentally changed.
2. ▶ Compile and run tests once the rename completes.

## Move Method

**Summary:** relocate a method from the class it currently lives on to the class it uses most.

**Motivation:** directly addresses Feature Envy — if a method spends most of its logic reading another object's data via its public members, that method's logic is conceptually closer to the other class than the one it's currently declared on.

**Mechanics:**
1. Examine everything the method uses from its current class — if it needs several members from there too, reconsider whether moving it fully is right, or whether only part of it should move.
2. Declare the method on the target class, adjusting references so it reads from the target's own fields directly instead of through accessor calls.
3. Decide what to do with the original declaration: delete it and update every call site, or leave a thin forwarding method behind temporarily if there are many call sites to migrate gradually. ▶ Compile and run tests after each call site is updated.

```csharp
// Before — InvoiceLine reaching into Money more than its own data
public class InvoiceLine
{
    public Money UnitPrice { get; }
    public int Quantity { get; }

    public Money CalculateLineTotal() =>
        Money.Create(UnitPrice.Amount * Quantity, UnitPrice.Currency);
}

// After — the calculation moved to Money, which already owns arithmetic invariants
public class InvoiceLine
{
    public Money UnitPrice { get; }
    public int Quantity { get; }

    public Money CalculateLineTotal() => UnitPrice.Multiply(Quantity);
}

public partial class Money
{
    public Money Multiply(int factor) => Create(Amount * factor, Currency);
}
```

## Move Field

**Summary:** relocate a field from the class it's declared on to the class that actually uses it.

**Motivation:** the data counterpart to `Move Method` — if more methods on another class use a field than methods on its own declaring class do, the field is in the wrong place, and every method that reaches across to use it is a small instance of Inappropriate Intimacy.

**Mechanics:**
1. Encapsulate the field first if it isn't already (private with accessor methods/properties), so every read and write goes through a single point.
2. Create the corresponding field on the target class.
3. Change the source's accessors to delegate to the target's new field instead of a local field.
4. ▶ Compile and run tests.
5. Once nothing depends on the source class exposing this data directly, consider moving the accessors themselves to the target class too.

## Extract Class

**Summary:** split part of a class's fields and methods off into a brand-new class, and connect the two either by composition or delegation.

**Motivation:** the primary fix for Large Class and Divergent Change. If a subset of a class's fields are only used by a subset of its methods, that subset is a class trying to get out.

**Mechanics:**
1. Decide how to split the responsibilities.
2. Create the new class, initially empty.
3. Create a way for the original class to reach the new one (a field holding an instance of it).
4. For each field being moved, use `Move Field` to relocate it to the new class, then update the original class's accessors to delegate.
5. For each method being moved, use `Move Method`. ▶ Compile and run tests after each individual field or method move — do not move everything in one step.
6. Once the split is complete, reconsider the original class's public surface — some delegating methods may no longer be needed if callers can be updated to talk to the new class directly.

## Replace Nested Conditional with Guard Clauses

**Summary:** replace a deeply nested `if`/`else` structure — where one branch is the normal case and the others are all edge cases — with a sequence of early returns, one per edge case, followed by the normal-case logic unindented at the end.

**Motivation:** nested conditionals bury the "normal path" logic inside indentation that exists purely to handle exceptional cases. Guard clauses make the exceptional cases visually and structurally distinct from the main logic, and let the main logic read as a straight line rather than the innermost branch of a pyramid. This project's own convention (`Guard` clauses inside domain factories — see `.github/copilot-instructions.md`) is a direct application of this same idea at the API-boundary level, not just inside individual methods.

**Mechanics:**
1. Pick the outermost condition that represents an edge case, not the normal path.
2. Replace it with a guard clause — `if (edgeCase) return earlyResult;` — at the top of the method, unindented.
3. ▶ Compile and run tests.
4. Repeat for each remaining edge-case condition, one at a time, running tests after each.
5. Once every edge case is a guard clause, the remaining, unindented code is the normal-path logic.

```csharp
// Before
public decimal CalculatePayout(Employee employee)
{
    decimal result;
    if (employee.IsSeparated)
    {
        result = 0m;
    }
    else
    {
        if (employee.IsRetired)
        {
            result = employee.PensionAmount;
        }
        else
        {
            result = employee.Salary * employee.PayoutRate;
        }
    }
    return result;
}

// After
public decimal CalculatePayout(Employee employee)
{
    if (employee.IsSeparated) return 0m;
    if (employee.IsRetired) return employee.PensionAmount;

    return employee.Salary * employee.PayoutRate;
}
```

## Replace Conditional with Polymorphism

**Summary:** replace a `switch`/`if-else` chain that branches on a type code with one method per case, placed on a type per case, so the language's own dispatch mechanism selects the right behavior instead of an explicit conditional.

**Motivation:** directly addresses the Switch Statements smell — specifically when the *same* switch, on the *same* type code, is duplicated at more than one call site. Every new case, without this refactoring, means finding and editing every duplicated switch; with it, adding a case means adding one new type. In this project, `Enumeration<TEnum, TId>` (see `.github/copilot-instructions.md`) is the idiomatic vehicle for this — behavior lives as a virtual/overridden member on each concrete enumeration instance rather than in a switch scattered across callers.

**Mechanics:**
1. If a base type doesn't already exist for the cases being switched on, create one — in this project, that's typically a new `Enumeration<TEnum, TId>` or a small class hierarchy.
2. Add a method to the base type, named after what the switch computes, with a default or abstract implementation.
3. For each `case`, override the method on the corresponding concrete case with that branch's logic.
4. Replace each call site's switch with a direct call to the new polymorphic method. ▶ Compile and run tests after each call site is converted.
5. Once every call site uses the polymorphic method, delete the original switch.

```csharp
// Before — the same switch duplicated wherever a display label is needed
public string GetStatusLabel(InvoiceStatus status)
{
    switch (status)
    {
        case InvoiceStatus.Draft: return "Not yet sent";
        case InvoiceStatus.Submitted: return "Awaiting payment";
        case InvoiceStatus.Paid: return "Complete";
        default: throw new ArgumentOutOfRangeException(nameof(status));
    }
}

// After — behavior lives on the Enumeration itself; no switch to duplicate
public sealed class InvoiceStatus : Enumeration<InvoiceStatus>
{
    public static readonly InvoiceStatus Draft = new(1, nameof(Draft), "Not yet sent");
    public static readonly InvoiceStatus Submitted = new(2, nameof(Submitted), "Awaiting payment");
    public static readonly InvoiceStatus Paid = new(3, nameof(Paid), "Complete");

    private InvoiceStatus(int id, string name, string label) : base(id, name) => Label = label;

    public string Label { get; }
}
```

## Introduce Null Object

**Summary:** replace repeated `if (x == null)` checks scattered across callers with a real object that implements the same interface but represents "nothing," so callers can treat the absent case uniformly with the present case.

**Motivation:** when many call sites all handle "this reference might be null" the same way, that repeated handling is itself a form of duplication. A Null Object collapses it into one place — the object itself — so callers no longer need the check at all.

**Mechanics:**
1. Create a subclass or alternate implementation representing the "nothing" case, with its methods returning sensible neutral values (zero, empty collection, no-op).
2. Change the method that currently might return `null` to return the Null Object instance instead.
3. Find each call site with a null check, and remove the check now that the Null Object handles the same calls safely.
4. ▶ Compile and run tests after each call site is simplified.

```csharp
public interface ICustomerDiscount
{
    decimal ApplyTo(decimal amount);
}

public sealed class PercentageDiscount : ICustomerDiscount
{
    private readonly decimal _rate;
    public PercentageDiscount(decimal rate) => _rate = rate;
    public decimal ApplyTo(decimal amount) => amount * (1 - _rate);
}

// Null Object — every caller can call ApplyTo without a null check first
public sealed class NoDiscount : ICustomerDiscount
{
    public static readonly NoDiscount Instance = new();
    public decimal ApplyTo(decimal amount) => amount;
}
```

**A note for this project:** where the "absent" case is itself a predictable, meaningful outcome the caller needs to react to differently (not just a neutral no-op), prefer this project's `Result`/`Error` convention over a Null Object — the two solve different problems. Null Object is for "there's nothing here, proceed as normal"; `Result.Failure` is for "something specific didn't happen, and the caller needs to know what."

## Introduce Parameter Object

**Summary:** replace a group of parameters that habitually travel together with a single object carrying them all.

**Motivation:** the direct fix for Long Parameter List when the cause is Data Clumps — several parameters that always appear together across multiple method signatures. Bundling them both shortens every affected signature and gives the clump a name and a place to eventually grow its own behavior.

**Mechanics:**
1. Create a new type holding the group of parameters as properties.
2. Add a new overload of the method taking the new type instead of the individual parameters, with a body that unpacks it and calls the original.
3. Migrate call sites to the new overload one at a time. ▶ Compile and run tests after each.
4. Once every call site uses the new overload, delete the old one and fold its body into the new one directly.

```csharp
// Before
public decimal CalculateShippingCost(string originPostalCode, string destPostalCode, decimal weightKg) { /* ... */ return 0m; }

// After
public sealed record ShippingRequest(string OriginPostalCode, string DestinationPostalCode, decimal WeightKg);

public decimal CalculateShippingCost(ShippingRequest request) { /* ... */ return 0m; }
```

## Replace Magic Number with Symbolic Constant

**Summary:** give a bare literal that has a specific, non-obvious meaning a named constant instead.

**Motivation:** a literal like `1.2m` scattered through tax-related code says nothing about *why* it's `1.2` — a named constant documents the meaning at the point of use and gives future changes exactly one place to happen instead of many.

**Mechanics:**
1. Declare a `const` or `static readonly` field with a name describing what the value represents.
2. Replace each occurrence of the literal with the new named constant. ▶ Compile and run tests after replacing each occurrence — a literal that looks the same in two places doesn't always mean the same thing, so verify each replacement is actually correct before moving to the next.

```csharp
// Before
decimal total = subtotal * 1.2m;

// After
private const decimal StandardTaxMultiplier = 1.2m; // 20% VAT
decimal total = subtotal * StandardTaxMultiplier;
```

## Consolidate Duplicate Conditional Fragments

**Summary:** if the exact same code appears inside every branch of a conditional, move it outside the conditional entirely.

**Motivation:** duplicated code inside a conditional's branches is easy to miss, because each branch looks reasonable in isolation — it's only the repetition across branches that's the smell.

**Mechanics:**
1. Identify the fragment that's identical in every branch.
2. Move one copy of it to just before or after the conditional (whichever preserves the original execution order).
3. Delete the now-redundant copies from each branch. ▶ Compile and run tests.

```csharp
// Before
if (isPreferredCustomer)
{
    ApplyDiscount(order);
    LogOrder(order);
}
else
{
    LogOrder(order);
}

// After
if (isPreferredCustomer)
{
    ApplyDiscount(order);
}
LogOrder(order);
```

## Introduce Assertion

**Summary:** make an assumption the code silently depends on explicit, by stating it as an assertion at the point where the assumption is made.

**Motivation:** code often depends on some condition being true at a given point (a value being within a certain range, a reference being non-null) without ever stating it — a future reader has no way to know the assumption exists until it's violated and something fails mysteriously elsewhere. An assertion documents the assumption and fails fast, at the actual point of violation, rather than somewhere downstream.

**Mechanics:**
1. Identify the implicit assumption.
2. State it explicitly using this project's `Guard` clauses (see `.github/copilot-instructions.md`) or `Debug.Assert` for assumptions that should be impossible in correct code rather than genuine input validation.
3. ▶ Compile and run tests — an assertion should never fire during normal test runs; if one does, it has either found a real bug or the assumption was wrong and needs revising, not silencing.

```csharp
public decimal CalculateDiscountedPrice(decimal price, decimal discountRate)
{
    Guard.AgainstOutOfRange(discountRate, 0m, 1m, "Discount rate must be between 0 and 1.");
    return price * (1 - discountRate);
}
```
