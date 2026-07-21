---
description: 'Guidelines for building C# NuGet class library projects on .NET 6+'
applyTo: >
  src/**/*.cs,
  tests/**/*.cs,
  **/*.csproj,
  **/*.sln,
  **/.github/workflows/*.yml,
  **/.github/workflows/*.yaml,
  README.md,
  CHANGELOG.md
---

# C# NuGet Library - Language & Design Constraints

## Language & Modern C# Features

- Target **.NET 6** and later using **C# features** appropriate to the minimum supported version.
- Prefer nullable reference types (`#nullable enable`) to catch nullability issues at compile-time.
- Use pattern matching, recursive patterns, and switch expressions for readability.
- Always apply `nameof()` when logging or raising exceptions to avoid hardcoded strings.

## Code Formatting Rules

- Enforce formatting profiles strictly conforming to the project's root `.editorconfig` specification.
- Mandate **file-scoped namespaces** and alphabetical, single-line using statements inside all `.cs` documents.
- **Curly Brace Policy:** Apply Allman style formatting: place opening curly braces on their own newline immediately preceding the code block (applies to classes, methods, `if`, `switch`, loops, and `using` scopes).
- Maintain distinct breathing room: ensure that final `return`, `throw`, or `yield` statements are separated from preceding logical mutations by a single blank newline.
- **Explicit Typing:** Prefer explicit types over `var` in all declarations. Only use `var` when the type is immediately evident from the right-hand side (e.g., `var invoice = new Invoice()`) — never when the type requires the reader to mentally resolve it.

## Architectural Naming & Structure Conventions

- **PascalCase:** Apply strictly to Class definitions, Record schemes, Enum values, Method signatures, and Public members.
- **camelCase:** Apply to local variable scopes, method parameters, and lambda modifiers.
- **Private State Fields:** Prefix private fields exclusively with a single underscore (e.g., `_cache`). Do not prefix local parameters or properties.
- **Interface Naming:** Prepend an uppercase `I` to all public interfaces (e.g., `IObjectMapper`), ensuring they reflect descriptive behavior capabilities.
- **Type Visibility:** Declare all types `internal` and `sealed` by default. Only expose `public` types when they are part of the public library API. Only remove `sealed` when inheritance is explicitly required by design. This prevents accidental API surface growth and maintains clear public contracts.
- **Public API Stability:** Once a type or method is made `public`, it becomes part of the semantic versioning contract. Changes to public signatures require major version bumps. Document public APIs clearly and consider deprecation carefully.

## Documentation & Code Comments Policy

- **Self-Documenting Code First:** Prioritize clear, expressive naming strategies for domain invariants over writing verbose inline text block narratives.
- **The "Why" Rule:** Restrict inline comments exclusively to explaining complex, non-obvious business domain exceptions or architectural constraints. Never use comments to explain basic framework operations, CRUD updates, or language features.
- **Algorithm & Complexity Metrics:** Do not add complexity notations for controllers, mappings, or standard repository routines. Require strict Big O metrics (e.g., `// Time: O(n log n) | Space: O(n)`) *only* inside background workers, performance-critical data filters, or custom calculation engines.
- **Public API Documentation:** Require structural XML triple-slash (`///`) summaries exclusively on public-facing endpoints or reusable core library packages. Utilize `<summary>`, `<param>`, `<response>`, and specialized `<exception>` markup tags cleanly.

## Project Setup and Structure

- Organize code into a simple structure: `src/` for library code and `tests/` for test projects.
- Keep the API surface minimal and intentional. Every public type should have a clear purpose.
- Group related public types in namespaces that reflect their domain (e.g., `KObjectMapper.Extensions`).
- Prefer composition over inheritance; use `sealed` by default to prevent accidental subclassing.
- Document all public APIs with XML triple-slash comments (`///`) including `<summary>`, `<param>`, and `<exception>` tags.

## Nullable Reference Types

- Declare variables non-nullable, and check for `null` at entry points.
- Always use `is null` or `is not null` instead of `== null` or `!= null`.
- Trust the C# null annotations and don't add null checks when the type system says a value cannot be null.

## Domain Modelling Patterns

These patterns apply to all types in the **Domain layer** and must be followed consistently. Copilot must never generate plain POCO classes, public setters, or raw `enum` declarations for domain concepts — always apply the patterns below.

### 1. Strongly Typed ID Domain Standards

All domain entity identifiers must be encapsulated as strongly typed IDs using the **`Techbiquity.Domain.Identity`** base classes. Never use raw `Guid`, `int`, `long`, or `string` as an entity identifier directly across Domain or Application boundaries.

The package provides two base classes:
- **`StronglyTypedId<T>`** — generic base for any non-nullable backing type (`Guid`, `string`, etc.). Hosts the generic `Create<TId>()` and `From<TId>(T)` factory methods used by all concrete ID types.
- **`GuidId`** — positional-record convenience base for GUID-backed IDs. Hosts the `From<TId>(string)` string-parsing shortcut. Inherits the `Guid.Empty` guard from `StronglyTypedId<T>.From<TId>()`.

Both are `abstract record` types — **reference types**, not structs. Never use `readonly record struct` for strongly typed IDs; those cannot inherit from `StronglyTypedId<T>`.

`ToString()` is `sealed` on `StronglyTypedId<T>` and returns `Value.ToString()` — subclasses must never override it.

---

**Base class implementations (`Techbiquity.Domain.Identity` package):**

```csharp
// Techbiquity.Domain.Identity/StronglyTypedId.cs
public abstract record StronglyTypedId<T>(T Value) where T : notnull
{
    private static readonly ConcurrentDictionary<Type, Func<T, object>> ConstructorCache = new();

    /// <summary>
    /// Generates a new unique identity for the concrete TId type.
    /// Supported backing types: Guid (Guid.NewGuid()), string ("{typename}_{guid:N}").
    /// Throws NotSupportedException for int, long, or other numeric types
    /// — those must use From() with a database-assigned value.
    /// </summary>
    public static TId Create<TId>() where TId : StronglyTypedId<T>
    {
        T value = typeof(T) switch
        {
            var t when t == typeof(Guid)   => (T)(object)Guid.NewGuid(),
            var t when t == typeof(string) => (T)(object)$"{typeof(TId).Name.ToLower()}_{Guid.NewGuid():N}",
            _ => throw new NotSupportedException(
                $"'{typeof(T).Name}' does not support parameterless generation. Use From() instead.")
        };

        return From<TId>(value);
    }

    /// <summary>
    /// Reconstitutes a concrete TId from an existing primitive value.
    /// Guards: null check, Guid.Empty check for Guid-backed types,
    /// null/whitespace check for string-backed types.
    /// Requires the concrete TId to expose a public single-argument constructor
    /// accepting a single T parameter — located via reflection and cached.
    /// </summary>
    public static TId From<TId>(T value) where TId : StronglyTypedId<T>
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is Guid guid && guid == Guid.Empty)
            throw new ArgumentException(
                $"The identifier value for '{typeof(TId).Name}' cannot be an empty GUID.", nameof(value));

        if (value is string str && string.IsNullOrWhiteSpace(str))
            throw new ArgumentException(
                $"The identifier value for '{typeof(TId).Name}' cannot be null, empty, or whitespace.", nameof(value));

        var factory = ConstructorCache.GetOrAdd(typeof(TId), type =>
        {
            var ctor = type.GetConstructor(new[] { typeof(T) })
                ?? throw new InvalidOperationException(
                    $"'{type.Name}' must declare a public constructor accepting a single '{typeof(T).Name}' parameter.");
            return param => Activator.CreateInstance(type, param)!;
        });

        return (TId)factory(value);
    }

    // Sealed — prevents type name leaking into logs, routes, or serialisation pipelines
    public sealed override string ToString() => Value.ToString() ?? string.Empty;
}

// Techbiquity.Domain.Identity/GuidId.cs
// Positional record — Value is inherited from StronglyTypedId<Guid>(Value)
public abstract record GuidId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    /// <summary>
    /// String-parsing shortcut for GUID-backed types.
    /// Delegates to StronglyTypedId<Guid>.From<TId>(Guid) after parsing,
    /// which applies the Guid.Empty guard.
    /// </summary>
    public static TId From<TId>(string value) where TId : GuidId
    {
        if (!Guid.TryParse(value, out Guid parsed))
            throw new ArgumentException(
                $"'{value}' is not a valid GUID format.", nameof(value));

        return StronglyTypedId<Guid>.From<TId>(parsed); // Guid.Empty guard applied here
    }
}
```

---

**Critical constraint — public constructor required on every concrete subclass:**

`StronglyTypedId<T>.From<TId>()` uses reflection to locate a **public single-argument constructor** accepting `T`. If the constructor is private or internal, the factory cache throws `InvalidOperationException` at runtime. Every concrete ID class must expose exactly this constructor.

---

**Canonical concrete subclass pattern — GUID-backed (the common case):**

```csharp
// Domain/Invoicing/InvoiceId.cs
public sealed record InvoiceId(Guid Value) : GuidId(Value)
{
    // Required — located by StronglyTypedId<Guid>.From<InvoiceId>() via reflection
    // Must be public — private or internal constructors break the factory cache
    public InvoiceId(Guid value) : this(value) { }

    // Static shortcuts — delegate to the generic base factories
    // These make call sites read as InvoiceId.Create() and InvoiceId.From(...)
    // rather than the more verbose StronglyTypedId<Guid>.Create<InvoiceId>()
    public static InvoiceId Create()           => StronglyTypedId<Guid>.Create<InvoiceId>();
    public static InvoiceId From(Guid value)   => StronglyTypedId<Guid>.From<InvoiceId>(value);
    public static InvoiceId From(string value) => GuidId.From<InvoiceId>(value);
}

// Domain/Invoicing/CustomerId.cs
public sealed record CustomerId(Guid Value) : GuidId(Value)
{
    public CustomerId(Guid value) : this(value) { }

    public static CustomerId Create()           => StronglyTypedId<Guid>.Create<CustomerId>();
    public static CustomerId From(Guid value)   => StronglyTypedId<Guid>.From<CustomerId>(value);
    public static CustomerId From(string value) => GuidId.From<CustomerId>(value);
}

// Usage
InvoiceId newId    = InvoiceId.Create();              // Guid.NewGuid() — via StronglyTypedId<Guid>.Create<InvoiceId>()
InvoiceId existing = InvoiceId.From(someGuid);        // Reconstituted from persistence — Guid.Empty guard applied
InvoiceId fromStr  = InvoiceId.From("3fa85f64-...");  // Reconstituted from route/JSON — parse + guard applied
Guid      raw      = newId.Value;                     // Underlying Guid
string    asStr    = newId.ToString();                // Guid.ToString() — no type metadata (sealed)

// Round-trip guarantee
InvoiceId roundTrip = InvoiceId.From(newId.ToString());
// roundTrip == newId ✅ always holds
```

**Declaring a string-backed ID (inherit from `StronglyTypedId<string>`):**

```csharp
// Domain/Catalogue/SkuId.cs
public sealed record SkuId(string Value) : StronglyTypedId<string>(Value)
{
    // Required public constructor — located by StronglyTypedId<string>.From<SkuId>() via reflection
    public SkuId(string value) : this(value) { }

    public static SkuId Create()           => StronglyTypedId<string>.Create<SkuId>();
    public static SkuId From(string value) => StronglyTypedId<string>.From<SkuId>(value);
}
// Note: Create() generates "skuid_{guid:N}" — a predictable string-backed identity
```

**Declaring a numeric-backed ID (`int`, `long`) — no `Create()`, database assigns value:**

```csharp
// Domain/Tenancy/TenantSequenceId.cs
public sealed record TenantSequenceId(long Value) : StronglyTypedId<long>(Value)
{
    // Required public constructor for From<TenantSequenceId>()
    public TenantSequenceId(long value) : this(value) { }

    // No Create() — long is not supported by the base Create<TId>() and throws NotSupportedException
    public static TenantSequenceId From(long value) => StronglyTypedId<long>.From<TenantSequenceId>(value);
}
```

**EF Core value converters — register in `OnModelCreating`:**

```csharp
// GUID-backed — From(Guid) delegates to StronglyTypedId<Guid>.From<InvoiceId>()
modelBuilder.Entity<Invoice>()
    .Property(e => e.Id)
    .HasConversion(
        id    => id.Value,
        value => InvoiceId.From(value));

// String-backed — From(string) delegates to StronglyTypedId<string>.From<SkuId>()
modelBuilder.Entity<Product>()
    .Property(e => e.Sku)
    .HasConversion(
        id    => id.Value,
        value => SkuId.From(value));
```

**Strongly Typed ID — Banned Patterns:**

| Banned | Correct alternative |
|---|---|
| `public readonly record struct InvoiceId(Guid Value)` | `public sealed record InvoiceId(Guid Value) : GuidId(Value)` |
| Private or internal constructor on a concrete ID class | Public single-arg constructor required — `StronglyTypedId<T>.From<TId>()` locates it via reflection |
| `new InvoiceId(guid)` called outside the ID class itself | `InvoiceId.From(guid)` — routes through the guarded base factory |
| `StronglyTypedId<Guid>.Create<InvoiceId>()` at call sites | `InvoiceId.Create()` — always use the concrete type's static shortcut |
| `InvoiceId.From(Guid.Empty)` | Throws `ArgumentException` — guarded in `StronglyTypedId<T>.From<TId>()` |
| Raw `Guid` as a method parameter where an ID is expected | The concrete strongly typed wrapper (e.g. `InvoiceId`) |
| Overriding `ToString()` on a subclass | `ToString()` is `sealed` on `StronglyTypedId<T>` — do not override |
| `InvoiceId.Create()` for numeric-backed (`int`/`long`) types | No `Create()` — use `TenantSequenceId.From(dbAssignedValue)` |
### 2. Abstract Base Classes — Entity and AggregateRoot

Every entity inherits from `Entity<TId>`. Every aggregate root inherits from `AggregateRoot<TId>`, which extends `Entity<TId>` with domain event dispatching.

```csharp
// Domain/Primitives/Entity.cs
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : StronglyTypedId<Guid>
{
    public TId Id { get; protected init; }

    protected Entity(TId id) => Id = id;

    protected Entity() { } // Required for EF Core proxy materialisation only

    public bool Equals(Entity<TId>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj) => obj is Entity<TId> entity && Equals(entity);
    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) => Equals(left, right);
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !Equals(left, right);
}

// Domain/Primitives/AggregateRoot.cs
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : StronglyTypedId<Guid>
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(TId id) : base(id) { }
    protected AggregateRoot() { } // Required for EF Core proxy materialisation only

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}

// Domain/Primitives/IDomainEvent.cs
public interface IDomainEvent : MediatR.INotification
{
    Guid EventId { get; }
    DateTimeOffset OccurredOn { get; }
}
```

---

### 3. Value Objects as Records

Implement all value objects as `record` types. Records provide structural equality, immutability, and non-destructive `with`-expression mutation for free. Never use a `record` for entities — entity equality is identity-based (same ID), not structure-based.

```csharp
// Domain/ValueObjects/Money.cs
public sealed record Money(decimal Amount, string Currency)
{
    public static readonly Money Zero = new(0, "GBP");

    public static Result<Money> Create(decimal amount, string currency)
    {
        if (amount < 0)
            return Result.Failure<Money>(Error.Validation("Money.NegativeAmount", "Amount cannot be negative."));

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            return Result.Failure<Money>(Error.Validation("Money.InvalidCurrency", "Currency must be a 3-letter ISO 4217 code."));

        return Result.Success(new Money(amount, currency));
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add {Currency} and {other.Currency}.");

        return this with { Amount = Amount + other.Amount };
    }

    public override string ToString() => $"{Amount:F2} {Currency}";
}

// Domain/ValueObjects/EmailAddress.cs
public sealed record EmailAddress(string Value)
{
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static Result<EmailAddress> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<EmailAddress>(Error.Validation("Email.Empty", "Email address cannot be empty."));

        if (!EmailRegex.IsMatch(value))
            return Result.Failure<EmailAddress>(Error.Validation("Email.Invalid", "Email address format is invalid."));

        return Result.Success(new EmailAddress(value.Trim().ToLowerInvariant()));
    }

    public override string ToString() => Value;
}
```

**EF Core — store value objects as owned entities or as JSON columns:**
```csharp
modelBuilder.Entity<Invoice>().OwnsOne(e => e.TotalAmount, money =>
{
    money.Property(m => m.Amount).HasColumnName("total_amount");
    money.Property(m => m.Currency).HasColumnName("total_currency").HasMaxLength(3);
});
```

---

### 4. Smart Enumeration Pattern (Techbiquity.Domain.Abstractions.Enumeration)

Replace all plain `enum` declarations for domain concepts with the **`Techbiquity.Domain.Abstractions.Enumeration<TEnum, TId>`** base class. This attaches behavior, display names, transition rules, and business logic directly to the enumeration value — eliminating scattered switch statements across the codebase.

The base class ships in two forms:
- **`Enumeration<TEnum, TId>`** — fully generic; use when the backing identifier is a `string`, `Guid`, or any type implementing `IComparable<TId>`. Lookup calls require explicit generic type arguments: `Enumeration<PaymentMethod, string>.FromValue<PaymentMethod>(v)`.
- **`Enumeration<TEnum>`** — convenience shortcut that fixes `TId` to `int`. Adds non-generic static overloads (`GetAll()`, `FromValue(int)`, `TryFromValue(int, out TEnum?)`, `FromName(string)`, `TryFromName(string, out TEnum?)`) directly on the concrete type — always prefer these for int-backed enumerations as they require no type arguments.

**Constructor parameter order is `(TId id, string name)`** — note this is `id` first, `name` second. Never reverse the order.

```csharp
// Domain/Enumerations/InvoiceStatus.cs
// Inherit from Enumeration<TEnum> for int-backed enumerations (the common case)
public sealed class InvoiceStatus : Enumeration<InvoiceStatus>
{
    // Constructor order is (int id, string name) — id first, name second
    public static readonly InvoiceStatus Draft     = new(1, nameof(Draft));
    public static readonly InvoiceStatus Submitted = new(2, nameof(Submitted));
    public static readonly InvoiceStatus Paid      = new(3, nameof(Paid));
    public static readonly InvoiceStatus Cancelled = new(4, nameof(Cancelled));

    private InvoiceStatus(int id, string name) : base(id, name) { }

    // Behavior belongs on the concrete type — not on the base class
    public bool CanTransitionTo(InvoiceStatus next) =>
        (this == Draft     && (next == Submitted || next == Cancelled)) ||
        (this == Submitted && (next == Paid      || next == Cancelled));
}
```

For string-backed enumerations (e.g. status codes that map to external system identifiers):

```csharp
// Domain/Enumerations/PaymentMethod.cs
public sealed class PaymentMethod : Enumeration<PaymentMethod, string>
{
    public static readonly PaymentMethod Card        = new("CARD",   nameof(Card));
    public static readonly PaymentMethod BankTransfer = new("BACS",  "Bank Transfer");
    public static readonly PaymentMethod DirectDebit  = new("DD",    "Direct Debit");

    private PaymentMethod(string id, string name) : base(id, name) { }
}
```

Look up members using the static helpers. For **int-backed** (`Enumeration<TEnum>`) types, call the convenience overloads directly on the concrete type — no generic type arguments needed. For **non-int-backed** (`Enumeration<TEnum, TId>`) types, use the explicit generic form on the base class.

```csharp
// ── int-backed (Enumeration<TEnum>) — use direct convenience overloads ──────

// Lookup by Id — call directly on the concrete type, no type arguments needed
InvoiceStatus paid = InvoiceStatus.FromValue(3);

// Safe lookup — no exception on miss; prefer this for untrusted input
if (InvoiceStatus.TryFromValue(statusId, out InvoiceStatus? status))
{
    // status is non-null here
}

// Lookup by Name
InvoiceStatus draft = InvoiceStatus.FromName("Draft");

// List all members — e.g. to populate a dropdown or seed validation
IEnumerable<InvoiceStatus> all = InvoiceStatus.GetAll();

// ── string-backed (Enumeration<TEnum, TId>) — use explicit generic form ─────

// Generic type arguments are required because convenience overloads
// only exist on the int-backed Enumeration<TEnum> shortcut
PaymentMethod card = Enumeration<PaymentMethod, string>.FromValue<PaymentMethod>("CARD");

if (Enumeration<PaymentMethod, string>.TryFromValue<PaymentMethod>(code, out PaymentMethod? method))
{
    // method is non-null here
}

IEnumerable<PaymentMethod> allMethods = Enumeration<PaymentMethod, string>.GetAll<PaymentMethod>();
```

**EF Core — persist as the `Id` value:**
```csharp
// int-backed (Enumeration<TEnum>) — use the direct convenience overload
modelBuilder.Entity<Invoice>()
    .Property(e => e.Status)
    .HasConversion(
        s => s.Id,
        v => InvoiceStatus.FromValue(v));

// string-backed (Enumeration<TEnum, string>) — explicit generic form required
modelBuilder.Entity<Payment>()
    .Property(e => e.Method)
    .HasConversion(
        m => m.Id,
        v => Enumeration<PaymentMethod, string>.FromValue<PaymentMethod>(v));
```

Never use a raw `enum` for any domain concept that has behavior, display requirements, or valid transition rules.
Never use `Ardalis.SmartEnum` — use `Techbiquity.Domain.Abstractions.Enumeration<TEnum, TId>` exclusively.

---

### 5. Encapsulated Construction — the `Create` Factory Method Pattern

All entities and value objects must use a **private constructor** combined with a `public static Result<T> Create(...)` factory method. This guarantees that no invalid object can ever be constructed — all invariants are enforced at the point of creation.

Rules:
- The constructor is always `private` (or `protected` for the EF Core parameterless overload only).
- `Create` returns `Result<T>` — never `T` directly and never `void`.
- `throw` is reserved for programmer errors and unrecoverable infrastructure faults only (e.g., a null argument that must never be null, or a concurrency conflict detected by the database). Predictable domain failures always return `Result.Failure(Error.BusinessRule(...))`, `Result.Failure(Error.Validation(...))`, or the appropriate `Error` factory method from `Techbiquity.Utilities.Functional`. Never throw a custom domain exception for a condition the caller could reasonably anticipate.
- `Create` raises the relevant domain event (e.g., `InvoiceCreatedDomainEvent`) before returning.

```csharp
// Domain/Invoicing/Invoice.cs
public sealed class Invoice : AggregateRoot<InvoiceId>
{
    public CustomerId CustomerId { get; private set; }
    public Money TotalAmount { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }

    private readonly List<InvoiceLineItem> _lineItems = [];
    public IReadOnlyList<InvoiceLineItem> LineItems => _lineItems.AsReadOnly();

    // EF Core materialisation only — never call directly
    private Invoice() { }

    private Invoice(
        InvoiceId id,
        CustomerId customerId,
        Money totalAmount,
        DateTimeOffset createdAt)
    {
        Id = id;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        Status = InvoiceStatus.Draft;
        CreatedAt = createdAt;
    }

    public static Result<Invoice> Create(
        CustomerId customerId,
        Money totalAmount,
        DateTimeOffset createdAt)
    {
        if (totalAmount.Amount <= 0)
            return Result.Failure<Invoice>(
                Error.Validation("Invoice.InvalidAmount", "Invoice total must be greater than zero."));

        Invoice invoice = new(
            InvoiceId.Create(),
            customerId,
            totalAmount,
            createdAt);

        invoice.RaiseDomainEvent(new InvoiceCreatedDomainEvent(
            invoice.Id,
            invoice.CustomerId,
            DateTimeOffset.UtcNow));

        return Result.Success(invoice);
    }

    // All mutations go through expressive domain methods — never via public setters
    public Result MarkAsPaid(DateTimeOffset paidAt)
    {
        if (!Status.CanTransitionTo(InvoiceStatus.Paid))
            return Result.Failure(
                Error.BusinessRule("Invoice.InvalidTransition",
                    $"Cannot transition invoice from {Status.Name} to Paid."));

        Status = InvoiceStatus.Paid;
        PaidAt = paidAt;

        RaiseDomainEvent(new InvoicePaidDomainEvent(Id, CustomerId, paidAt));

        return Result.Success();
    }

    public Result Cancel()
    {
        if (!Status.CanTransitionTo(InvoiceStatus.Cancelled))
            return Result.Failure(
                Error.BusinessRule("Invoice.InvalidTransition",
                    $"Cannot cancel an invoice in {Status.Name} status."));

        Status = InvoiceStatus.Cancelled;

        RaiseDomainEvent(new InvoiceCancelledDomainEvent(Id, DateTimeOffset.UtcNow));

        return Result.Success();
    }
}
```

---

### 6. Private Setters & Encapsulated Domain Mutations

**All entity properties must use `private set` or `private init`.** Public setters are banned on domain entities without exception. Every state change must flow through a named domain method that enforces invariants and raises the corresponding domain event.

```csharp
// BANNED — Copilot must never generate this pattern on domain entities
public string Status { get; set; }
public decimal Amount { get; set; }

// CORRECT — all mutations through domain methods with private setters
public InvoiceStatus Status { get; private set; }
public Money TotalAmount { get; private set; }
```

Naming convention for domain mutation methods:
- **Commands that change state:** `MarkAsPaid`, `Submit`, `Cancel`, `Activate`, `Deactivate`
- **Queries that derive state:** `IsOverdue()`, `CalculateTax()`, `HasLineItems()`
- Never name a method `SetX` or `UpdateX` — these imply public setter semantics and bypass domain intent.

---

### 7. Result Pattern

All factory methods, domain mutations, and application use-case handlers must return `Result<T>` or `Result` rather than throwing for predictable failure states.

```csharp
// Application/Invoicing/Commands/CreateInvoiceCommandHandler.cs
internal sealed class CreateInvoiceCommandHandler
    : IRequestHandler<CreateInvoiceCommand, Result<InvoiceResponse>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<InvoiceResponse>> Handle(
        CreateInvoiceCommand command,
        CancellationToken cancellationToken)
    {
        Result<Money> moneyResult = Money.Create(command.Amount, command.Currency);

        if (moneyResult.IsFailure)
            return Result.Failure<InvoiceResponse>(moneyResult.Error);

        Result<Invoice> invoiceResult = Invoice.Create(
            CustomerId.From(command.CustomerId),
            moneyResult.Value,
            DateTimeOffset.UtcNow);

        if (invoiceResult.IsFailure)
            return Result.Failure<InvoiceResponse>(invoiceResult.Error);

        await _invoiceRepository.AddAsync(invoiceResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(InvoiceResponse.FromDomain(invoiceResult.Value));
    }
}
```

Map `Result` states to HTTP responses via the `ToProblemResult()` extension — the `ErrorType` drives the status code:
- `ErrorType.Validation` → `400 Bad Request`
- `ErrorType.NotFound` → `404 Not Found`
- `ErrorType.Conflict` → `409 Conflict`
- `ErrorType.BusinessRule` → `422 Unprocessable Entity`
- `ErrorType.Failure` / unmatched → `500 Internal Server Error`

---

### 8. Domain Events

Every significant state change on an aggregate root must raise a domain event. Domain events are dispatched **after** the unit of work commits, via a MediatR `INotificationHandler<T>` in the Application layer.

```csharp
// Domain/Invoicing/Events/InvoiceCreatedDomainEvent.cs
public sealed record InvoiceCreatedDomainEvent(
    InvoiceId InvoiceId,
    CustomerId CustomerId,
    DateTimeOffset OccurredOn) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
}

// Infrastructure — dispatch after SaveChangesAsync via an interceptor or UoW decorator
public sealed class DomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public DomainEventDispatcher(IPublisher publisher) =>
        _publisher = publisher;

    public async Task DispatchAsync(
        IEnumerable<AggregateRoot<dynamic>> aggregates,
        CancellationToken cancellationToken)
    {
        IEnumerable<IDomainEvent> events = aggregates
            .SelectMany(a => a.DomainEvents);

        foreach (IDomainEvent domainEvent in events)
            await _publisher.Publish(domainEvent, cancellationToken);
    }
}
```

Domain events are `sealed record` types — immutable, named in past tense (`InvoiceCreatedDomainEvent`, `InvoicePaidDomainEvent`), and carry only the data needed by handlers. They must not reference domain services or repositories directly.

---

### Domain Layer — Banned Patterns Checklist

Copilot must never generate the following patterns inside the Domain layer:

| Banned | Correct alternative |
|---|---|
| `public string Status { get; set; }` | `public InvoiceStatus Status { get; private set; }` |
| `new Invoice(...)` called from outside the entity | `Invoice.Create(...)` returning `Result<Invoice>` |
| Raw `enum InvoiceStatus` | `Enumeration<InvoiceStatus>` via `Techbiquity.Domain.Abstractions` |
| `public Guid Id { get; set; }` on an entity | `public InvoiceId Id { get; private init; }` — using a `GuidId`-derived type |
| `record` used for entities | `record` for value objects only; `class` for entities |
| Domain exceptions for validation failures | `Result.Failure(...)` for predictable failures |
| `void` return type on domain mutation methods | `Result` or `Result<T>` always |
| EF Core `DbContext` referenced in Domain layer | Zero infrastructure dependencies in Domain |

---

## Data Access Patterns

### Core Philosophy — Full Encapsulation, Zero EF Leakage

The data access layer is implemented exclusively using **Entity Framework Core** with the **Npgsql.EntityFrameworkCore.PostgreSQL** provider. All data access — reads, writes, queries, and projections — is fully encapsulated behind **Repository** and **Unit of Work** abstractions defined in the Application layer and implemented in the Infrastructure layer.

**These rules are absolute and apply everywhere without exception:**

- `IQueryable<T>` must never cross the Infrastructure layer boundary. It must never appear in Domain layer interfaces, Application layer handler signatures, or any type outside of Infrastructure.
- `DbContext`, `DbSet<T>`, EF Core LINQ extension methods (`.Include()`, `.AsNoTracking()`, `.Where()`, `.Select()`), and any other EF Core type must never be referenced outside of the Infrastructure layer.
- Application layer handlers depend only on repository interfaces and `IUnitOfWork` — both defined in the **Domain layer** and injected by Infrastructure via DI. They have no knowledge of how data is fetched, tracked, or persisted.
- Every query — including simple lookups and paginated list reads — is expressed as a named method on a typed repository interface. There are no "pass-through" query paths.

---

### 1. Generic Repository Interface (Domain Layer)

Define the generic base repository interface in the **Domain layer**, alongside the aggregate root primitives it constrains. The Domain layer has zero external dependencies — this interface is pure domain code: no EF Core, no infrastructure concerns. Application references Domain directly and consumes these interfaces via DI. Infrastructure references Application directly (and therefore Domain transitively) and provides the concrete implementations.

```csharp
// Domain/Abstractions/IRepository.cs
public interface IRepository<TEntity, TId>
    where TEntity : AggregateRoot<TId>
    where TId : StronglyTypedId<Guid>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
```

---

### 2. Typed Repository Interfaces (Domain Layer)

Every aggregate root has its own **typed repository interface** co-located in the **Domain layer**, alongside the aggregate it belongs to. All domain-specific query methods are declared here as explicit, named, intention-revealing methods. Never add generic filter parameters, expression trees, or `IQueryable` to these interfaces — those are Infrastructure concerns.

```csharp
// Domain/Invoicing/Abstractions/IInvoiceRepository.cs
public interface IInvoiceRepository : IRepository<Invoice, InvoiceId>
{
    Task<Invoice?> GetByIdWithLineItemsAsync(
        InvoiceId id,
        CancellationToken cancellationToken = default);

    Task<PagedList<Invoice>> GetPagedByCustomerAsync(
        CustomerId customerId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Invoice>> GetOverdueAsync(
        DateTimeOffset asOf,
        CancellationToken cancellationToken = default);

    Task<bool> HasUnpaidInvoicesAsync(
        CustomerId customerId,
        CancellationToken cancellationToken = default);

    Task<InvoiceSummaryDto> GetSummaryByIdAsync(
        InvoiceId id,
        CancellationToken cancellationToken = default);
}
```

Query methods that return read-optimised projections (e.g., `InvoiceSummaryDto`) return concrete DTO types — not entities. The Domain interface declares only the method signature and return type. The Application layer defines the DTO shape. The Infrastructure layer performs the EF Core projection internally using `.Select()` and `.AsNoTracking()` — hidden from all callers.

---

### 3. Unit of Work Interface (Domain Layer)

Define `IUnitOfWork` in the **Domain layer**, alongside `IRepository`. It controls transaction boundaries and commits all pending changes from all repositories in a single atomic operation. Application handlers call it directly — they reference Domain and therefore see the interface. Infrastructure provides the concrete implementation by referencing Application (and Domain transitively). Repositories never call `SaveChangesAsync` directly.

```csharp
// Domain/Abstractions/IUnitOfWork.cs
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

---

### 4. Repository Implementation (Infrastructure Layer)

Repository implementations live exclusively in the Infrastructure layer. All EF Core concerns — `DbContext`, `DbSet<T>`, `.Include()`, `.AsNoTracking()`, `.Select()` projections, and pagination logic — are contained here and never exposed upward.

```csharp
// Infrastructure/Repositories/InvoiceRepository.cs
internal sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly AppDbContext _dbContext;

    public InvoiceRepository(AppDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task<Invoice?> GetByIdAsync(
        InvoiceId id,
        CancellationToken cancellationToken = default) =>
        await _dbContext.Invoices
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<Invoice?> GetByIdWithLineItemsAsync(
        InvoiceId id,
        CancellationToken cancellationToken = default) =>
        await _dbContext.Invoices
            .Include(i => i.LineItems)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<PagedList<Invoice>> GetPagedByCustomerAsync(
        CustomerId customerId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Invoice> query = _dbContext.Invoices
            .AsNoTracking()
            .Where(i => i.CustomerId == customerId)
            .OrderByDescending(i => i.CreatedAt);

        int totalCount = await query.CountAsync(cancellationToken);

        List<Invoice> items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<Invoice>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<InvoiceSummaryDto> GetSummaryByIdAsync(
        InvoiceId id,
        CancellationToken cancellationToken = default) =>
        await _dbContext.Invoices
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new InvoiceSummaryDto(
                i.Id.Value,
                i.CustomerId.Value,
                i.TotalAmount.Amount,
                i.TotalAmount.Currency,
                i.Status.Name,
                i.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken)
        ?? throw new InvalidOperationException($"Invoice {id} not found during projection.");

    public async Task<bool> ExistsAsync(
        InvoiceId id,
        CancellationToken cancellationToken = default) =>
        await _dbContext.Invoices
            .AnyAsync(i => i.Id == id, cancellationToken);

    public async Task<bool> HasUnpaidInvoicesAsync(
        CustomerId customerId,
        CancellationToken cancellationToken = default) =>
        await _dbContext.Invoices
            .AsNoTracking()
            .AnyAsync(i =>
                i.CustomerId == customerId &&
                i.Status != InvoiceStatus.Paid &&
                i.Status != InvoiceStatus.Cancelled,
                cancellationToken);

    public async Task<IReadOnlyList<Invoice>> GetOverdueAsync(
        DateTimeOffset asOf,
        CancellationToken cancellationToken = default)
    {
        List<Invoice> results = await _dbContext.Invoices
            .AsNoTracking()
            .Where(i => i.DueAt < asOf && i.Status == InvoiceStatus.Submitted)
            .ToListAsync(cancellationToken);

        return results.AsReadOnly();
    }

    public async Task AddAsync(
        Invoice entity,
        CancellationToken cancellationToken = default) =>
        await _dbContext.Invoices.AddAsync(entity, cancellationToken);

    public void Update(Invoice entity) =>
        _dbContext.Invoices.Update(entity);

    public void Remove(Invoice entity) =>
        _dbContext.Invoices.Remove(entity);
}
```

---

### 5. Unit of Work Implementation (Infrastructure Layer)

```csharp
// Infrastructure/Persistence/UnitOfWork.cs
internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public UnitOfWork(AppDbContext dbContext) =>
        _dbContext = dbContext;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Database.CommitTransactionAsync(cancellationToken);

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Database.RollbackTransactionAsync(cancellationToken);
}
```

---

### 6. Application Layer Handler — Correct Usage Pattern

Application layer handlers depend only on repository interfaces and `IUnitOfWork` — both defined in the Domain layer and injected via DI by Infrastructure. No EF Core type, namespace, or concept appears in the Application layer.

```csharp
// Application/Invoicing/Queries/GetInvoiceSummaryQueryHandler.cs
internal sealed class GetInvoiceSummaryQueryHandler
    : IRequestHandler<GetInvoiceSummaryQuery, Result<InvoiceSummaryDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoiceSummaryQueryHandler(IInvoiceRepository invoiceRepository) =>
        _invoiceRepository = invoiceRepository;

    public async Task<Result<InvoiceSummaryDto>> Handle(
        GetInvoiceSummaryQuery query,
        CancellationToken cancellationToken)
    {
        InvoiceSummaryDto? summary = await _invoiceRepository
            .GetSummaryByIdAsync(InvoiceId.From(query.InvoiceId), cancellationToken);

        return summary is null
            ? Result.Failure<InvoiceSummaryDto>(Invoice.Errors.NotFound)
            : Result.Success(summary);
    }
}
```

---

### 7. DI Registration (Infrastructure Layer)

Register repositories and the Unit of Work in the Infrastructure layer's DI composition root. Use `internal sealed` on all implementations — they are never exposed beyond the Infrastructure assembly boundary.

```csharp
// Infrastructure/DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }
}
```

---

### 8. Database Migrations & Seeding

- Manage all schema changes exclusively via the `dotnet ef migrations` CLI tool. Never use `EnsureCreated()` in non-test environments.
- Apply Npgsql retry-on-failure within `DbContextOptions` (shown above) — never in repository methods.
- Use strongly-typed JSONB columns (`jsonb`) for unstructured document fragments or semi-structured auditing payloads where appropriate.

---

### 9. Data Seeding Strategy

Data seeding is not a single concern — three distinct categories of data require different entry points, lifecycle rules, and CI/CD behaviours. Never mix categories in the same seeder. Each category has a mandatory location, trigger, and idempotency contract.

| Category | Description | Entry point | Environment | Idempotent |
|---|---|---|---|---|
| **Master / Reference Data** | Lookup tables, enumerations, country codes, currency codes, status types | EF Core `HasData` in `IEntityTypeConfiguration<T>` | All | Yes — baked into migrations |
| **Administrative / Bootstrap Data** | Initial admin user, default tenant, system roles, feature flags | `IDataSeeder` executed at startup via `DatabaseInitialiser` | All (conditionally) | Yes — guarded by existence check |
| **Mock / Test Data** | Realistic sample data for development and integration testing | `IDataSeeder` executed at startup, gated strictly to `Development` or `Testing` environment | Dev / Test only | Yes — idempotent by design; never runs in Production |

---

#### Category 1 — Master / Reference Data

Master data is **stable, environment-agnostic, and version-controlled** alongside schema changes. It belongs in EF Core's `HasData` API inside `IEntityTypeConfiguration<T>` configurations. This guarantees it is applied atomically with the migration that introduced the table — no separate startup logic, no timing risks.

```csharp
// Infrastructure/Persistence/Configurations/CurrencyConfiguration.cs
internal sealed class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("currencies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        // Master data — seeded directly into the migration
        builder.HasData(
            new Currency(CurrencyId.From(new Guid("00000000-0000-0000-0000-000000000001")), "GBP", "British Pound Sterling"),
            new Currency(CurrencyId.From(new Guid("00000000-0000-0000-0000-000000000002")), "USD", "United States Dollar"),
            new Currency(CurrencyId.From(new Guid("00000000-0000-0000-0000-000000000003")), "EUR", "Euro")
        );
    }
}
```

**Rules for master data `HasData`:**
- Always use **hardcoded, deterministic GUIDs** — never `Guid.NewGuid()`. GUIDs generated at migration time are baked in and will drift on every `dotnet ef migrations add`, producing false diffs.
- Never include mutable or environment-specific values (e.g., URLs, connection strings, tenant-specific IDs) in `HasData`.
- If a master data row changes (e.g., a currency name is corrected), produce a new migration with an `UpdateData` call — never edit a previous migration's `HasData`.
- Enumeration `Id` values must mirror `HasData` rows exactly. If `InvoiceStatus.Draft` has `Id = 1`, the seed row must persist `1` as the stored value. Never seed by `Name` — always seed by `Id`.

---

#### Category 2 — Administrative / Bootstrap Data

Bootstrap data is the minimum configuration the application needs to be functional — an initial admin user, a default tenant, system roles, or default feature flags. It is **conditional** (only insert if the record does not already exist) and runs at application startup via a dedicated `IDataSeeder` pipeline.

**Define the seeder contract in the Domain layer**, alongside the other persistence abstractions:

```csharp
// Domain/Abstractions/IDataSeeder.cs
public interface IDataSeeder
{
    /// <summary>
    /// Execution order relative to other seeders. Lower values run first.
    /// </summary>
    int Order { get; }
    Task SeedAsync(CancellationToken cancellationToken = default);
}
```

**Implement bootstrap seeders in the Infrastructure layer:**

```csharp
// Infrastructure/Persistence/Seeders/DefaultTenantSeeder.cs
internal sealed class DefaultTenantSeeder : IDataSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<DefaultTenantSeeder> _logger;

    public int Order => 10; // Tenants before users

    public DefaultTenantSeeder(AppDbContext dbContext, ILogger<DefaultTenantSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        Guid defaultTenantId = new("10000000-0000-0000-0000-000000000001");

        bool exists = await _dbContext.Tenants
            .AnyAsync(t => t.Id == TenantId.From(defaultTenantId), cancellationToken);

        if (exists)
        {
            _logger.LogDebug("Default tenant already exists. Skipping seed.");
            return;
        }

        Tenant tenant = Tenant.Create(
            TenantId.From(defaultTenantId),
            "Techbiquity",
            "admin@techbiquity.com").Value;

        await _dbContext.Tenants.AddAsync(tenant, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Default tenant seeded successfully.");
    }
}

// Infrastructure/Persistence/Seeders/DefaultAdminUserSeeder.cs
internal sealed class DefaultAdminUserSeeder : IDataSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DefaultAdminUserSeeder> _logger;

    public int Order => 20; // Users after tenants

    public DefaultAdminUserSeeder(
        AppDbContext dbContext,
        IConfiguration configuration,
        ILogger<DefaultAdminUserSeeder> logger)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        string adminEmail = _configuration["Seeding:AdminEmail"]
            ?? throw new InvalidOperationException("Seeding:AdminEmail is not configured.");

        bool exists = await _dbContext.Users
            .AnyAsync(u => u.Email.Value == adminEmail, cancellationToken);

        if (exists)
        {
            _logger.LogDebug("Admin user already exists. Skipping seed.");
            return;
        }

        // Create via domain factory — invariants enforced, domain event raised
        Result<User> result = User.Create(
            UserId.New(),
            adminEmail,
            UserRole.SystemAdministrator);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to seed admin user: {Error}", result.Error.Message);
            return;
        }

        await _dbContext.Users.AddAsync(result.Value, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Admin user seeded successfully: {Email}", adminEmail);
    }
}
```

**The `DatabaseInitialiser` — the single startup orchestrator:**

```csharp
// Infrastructure/Persistence/DatabaseInitialiser.cs
internal sealed class DatabaseInitialiser : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DatabaseInitialiser> _logger;
    private readonly IHostEnvironment _environment;

    public DatabaseInitialiser(
        IServiceScopeFactory scopeFactory,
        ILogger<DatabaseInitialiser> logger,
        IHostEnvironment environment)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _environment = environment;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        AppDbContext dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        // 1. Apply all pending EF Core migrations atomically at startup
        _logger.LogInformation("Applying database migrations...");
        await dbContext.Database.MigrateAsync(stoppingToken);
        _logger.LogInformation("Migrations applied.");

        // 2. Run bootstrap seeders (all environments)
        IEnumerable<IDataSeeder> bootstrapSeeders = scope.ServiceProvider
            .GetServices<IDataSeeder>()
            .Where(s => s is not IMockDataSeeder)
            .OrderBy(s => s.Order);

        foreach (IDataSeeder seeder in bootstrapSeeders)
        {
            _logger.LogInformation("Running seeder: {Seeder}", seeder.GetType().Name);
            await seeder.SeedAsync(stoppingToken);
        }

        // 3. Run mock/test seeders only in Development or Testing environments
        if (_environment.IsDevelopment() ||
            _environment.IsEnvironment("Testing"))
        {
            IEnumerable<IMockDataSeeder> mockSeeders = scope.ServiceProvider
                .GetServices<IMockDataSeeder>()
                .OrderBy(s => s.Order);

            foreach (IMockDataSeeder seeder in mockSeeders)
            {
                _logger.LogInformation("Running mock seeder: {Seeder}", seeder.GetType().Name);
                await seeder.SeedAsync(stoppingToken);
            }
        }
    }
}
```

---

#### Category 3 — Mock / Test Data

Mock data provides realistic, volume-appropriate data for local development and integration test runs. It is **strictly gated** to `Development` and `Testing` environments. It must never execute in Staging or Production under any circumstances.

Mark mock seeders with a dedicated marker interface to enforce the environment gate in `DatabaseInitialiser`:

```csharp
// Domain/Abstractions/IMockDataSeeder.cs
public interface IMockDataSeeder : IDataSeeder { }
```

```csharp
// Infrastructure/Persistence/Seeders/MockInvoiceSeeder.cs
internal sealed class MockInvoiceSeeder : IMockDataSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<MockInvoiceSeeder> _logger;

    public int Order => 100;

    public MockInvoiceSeeder(AppDbContext dbContext, ILogger<MockInvoiceSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // Idempotency guard — skip if mock data already present
        bool alreadySeeded = await _dbContext.Invoices
            .AnyAsync(i => i.Tags.Contains("mock"), cancellationToken);

        if (alreadySeeded)
        {
            _logger.LogDebug("Mock invoices already seeded. Skipping.");
            return;
        }

        CustomerId customerId = CustomerId.From(new Guid("20000000-0000-0000-0000-000000000001"));

        List<Invoice> invoices = [];

        for (int i = 1; i <= 20; i++)
        {
            Result<Money> money = Money.Create(100m * i, "GBP");
            Result<Invoice> invoice = Invoice.Create(
                customerId,
                money.Value,
                DateTimeOffset.UtcNow.AddDays(-i));

            if (invoice.IsSuccess)
            {
                invoice.Value.AddTag("mock"); // Domain method to tag mock data
                invoices.Add(invoice.Value);
            }
        }

        await _dbContext.Invoices.AddRangeAsync(invoices, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded {Count} mock invoices.", invoices.Count);
    }
}
```

---

#### Integration Test Seeding

For integration tests using Testcontainers, seeding is handled by a dedicated `WebApplicationFactory` base class that runs the `DatabaseInitialiser` against a fresh containerised PostgreSQL instance. Mock seeders run automatically because the environment is set to `Testing`.

```csharp
// Tests/Integration/Infrastructure/IntegrationTestWebApplicationFactory.cs
public sealed class IntegrationTestWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Replace production DbContext with test container connection string
            ServiceDescriptor? descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_postgres.GetConnectionString()));
        });
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        // DatabaseInitialiser runs automatically as a BackgroundService on startup
        // Migrations + bootstrap seeders + mock seeders all execute here
        using IServiceScope scope = Services.CreateScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync() => await _postgres.DisposeAsync();
}
```

---

#### DI Registration

```csharp
// Infrastructure/DependencyInjection.cs (additions)
public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // ... existing DbContext, repositories, UoW registrations ...

    // Startup orchestrator — runs migrations + seeders on boot
    services.AddHostedService<DatabaseInitialiser>();

    // Bootstrap seeders — all environments
    services.AddScoped<IDataSeeder, DefaultTenantSeeder>();
    services.AddScoped<IDataSeeder, DefaultAdminUserSeeder>();

    // Mock seeders — DatabaseInitialiser gates these to Dev/Testing only
    services.AddScoped<IMockDataSeeder, MockInvoiceSeeder>();
    services.AddScoped<IMockDataSeeder, MockCustomerSeeder>();

    return services;
}
```

---

#### CI/CD Pipeline Behaviour

| Pipeline stage | Migrations | Bootstrap seeders | Mock seeders |
|---|---|---|---|
| PR / unit test run | Not applied (no DB) | Not applied | Not applied |
| Integration test run (Testcontainers) | ✅ Applied by `DatabaseInitialiser` | ✅ Run automatically | ✅ Run (`Testing` environment) |
| Staging deployment | ✅ Applied at startup | ✅ Run (idempotent) | ❌ Skipped (environment gate) |
| Production deployment | ✅ Applied at startup | ✅ Run (idempotent) | ❌ Never runs |

**Key rules for CI/CD-safe seeding:**
- `DatabaseInitialiser` calls `MigrateAsync()` — never `EnsureCreated()`. `MigrateAsync` is safe to call on a database that is already up to date; it is a no-op when no pending migrations exist.
- All bootstrap seeders must be **fully idempotent** — calling them twice on the same database must produce zero writes on the second invocation. Always guard with an existence check before inserting.
- Never use `HasData` for bootstrap or mock data — it is tracked by EF Core migrations and will generate spurious `DeleteData`/`InsertData` migration entries if the data changes.
- Seed data configuration values (admin email, default tenant name) must come from `IConfiguration` / environment variables, never hard-coded in the seeder — this allows different values per environment without code changes.
- Mock seeders must always include an idempotency guard (e.g., a tag, a flag column, or a count check) so re-running `DatabaseInitialiser` in a long-lived dev container does not duplicate data.

---

#### Seeding — Banned Patterns Checklist

| Banned | Correct alternative |
|---|---|
| `Guid.NewGuid()` inside `HasData` | Hardcoded deterministic `Guid` literals |
| `HasData` for bootstrap or mock data | `IDataSeeder` implementations |
| `HasData` for anything mutable or environment-specific | `IDataSeeder` reading from `IConfiguration` |
| Seeding logic inside `OnModelCreating` beyond `HasData` | Dedicated `IDataSeeder` / `IMockDataSeeder` class |
| Mock seeders without an environment gate | `DatabaseInitialiser` guards by `IHostEnvironment` |
| `EnsureCreated()` in any non-test path | `MigrateAsync()` always |
| Seeder inserting without an idempotency guard | Existence check before every insert |
| Admin credentials hard-coded in seeder source | Read from `IConfiguration` / environment variables |
| Running `dotnet ef database update` in production CD pipeline | Startup `MigrateAsync()` in `DatabaseInitialiser` |

---

### Data Access — Banned Patterns Checklist

| Banned | Correct alternative |
|---|---|
| `IQueryable<T>` in any Application or Domain type | Named query method on the typed repository interface |
| `DbContext` injected into an Application handler | Domain-defined `IRepository<T>` and `IUnitOfWork`, injected via DI |
| `.Include()` called outside a repository | Inside the repository implementation only |
| `.AsNoTracking()` called outside a repository | Inside the repository implementation only |
| `using Microsoft.EntityFrameworkCore` in Application layer | Zero EF Core namespaces in Application or Domain |
| `repository.SaveChangesAsync()` on the repository itself | `IUnitOfWork.SaveChangesAsync()` called by the handler |
| Generic filter parameters (`Expression<Func<T, bool>>`) on interfaces | Explicit named methods per query intent |
| Returning `IEnumerable<T>` from a repository and filtering in the handler | Filter inside the repository; return `IReadOnlyList<T>` |
| `new DbContext(...)` or `DbContextFactory` in Application layer | Domain-defined `IRepository<T>` injected via DI only |

## Authentication and Authorization (Keycloak & PostgreSQL)

- Guide the implementation of centralized Identity and Access Management (IAM) strictly utilizing **Keycloak** (targeting version 26+), completely bypassing Microsoft Entra ID or native .NET Identity tables.
- Mandate the containerization and orchestration of Keycloak via Docker Compose, leveraging the existing **PostgreSQL** instance as its persistent backing store:
  - Enforce production configuration flags: set `KC_DB=postgres` and map the database connection parameters using JDBC formatting: `KC_DB_URL=jdbc:postgresql://<db_host>:5432/<db_name>`.
  - Maintain absolute environment isolation between the application's business schemas and Keycloak's identity tracking schemas within the PostgreSQL instance.
- Direct token validation within the WebAPI using the standard **Microsoft.AspNetCore.Authentication.JwtBearer** library:
  - Configure `JwtBearerOptions.Authority` to point directly to the localized OpenID Connect realm endpoint: `http://<keycloak_host>:<port>/realms/<realm_name>`.
  - Ensure token signature validation is performed locally against Keycloak's JSON Web Key Set (JWKS) advertised via its `.well-known/openid-configuration` metadata endpoint.
- Enforce strict structural mapping of Keycloak Roles into standard .NET Security Claims:
  - Implement a custom `IClaimsTransformation` or use custom token claim mappings to extract nested roles from the Keycloak access token payload (specifically parsing the `realm_access.roles` and `resource_access.{client_id}.roles` JSON arrays).
  - Map extracted strings directly to `ClaimTypes.Role` so they satisfy native `[Authorize(Roles = "...")]` attributes, policy-based authorization handlers, and Minimal API `.RequireAuthorization()` chain filters.
- Enforce short-lived access token lifetimes (5–15 minutes) with refresh token rotation enabled in Keycloak realm settings. Revoke refresh tokens on use to prevent token replay attacks.
- Secure both traditional controller-based endpoints and Minimal API endpoint groups consistently, ensuring that Swagger UI acts as a public OAuth 2.0 Authorization Code Flow client with PKCE enabled (`S256`) when communicating with the Keycloak server.

## Security Hardening

- **Secrets Management:** Never hard-code connection strings, API keys, or credentials in source files or `appsettings.json`. Use .NET User Secrets for local development (`dotnet user-secrets`), environment variables for containers, and a dedicated secrets vault (HashiCorp Vault or Azure Key Vault via `Azure.Extensions.AspNetCore.Configuration.Secrets`) for staging and production environments.
- **Rate Limiting:** Enforce ASP.NET Core's built-in rate limiting middleware (`Microsoft.AspNetCore.RateLimiting`) on all public-facing API endpoints. Define distinct policies per endpoint sensitivity:
  - Use a fixed window or sliding window limiter on general API routes.
  - Apply a stricter concurrency or token bucket limiter on authentication, password reset, and OTP endpoints.
  ```csharp
  builder.Services.AddRateLimiter(options =>
  {
      options.AddFixedWindowLimiter("api", o =>
      {
          o.PermitLimit = 100;
          o.Window = TimeSpan.FromMinutes(1);
          o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
          o.QueueLimit = 5;
      });
  });
  ```
- **CORS Policy:** Define an explicit, named CORS policy — never use `.AllowAnyOrigin()` in non-development environments. Restrict allowed origins to a whitelist sourced from configuration:
  ```csharp
  builder.Services.AddCors(options =>
  {
      options.AddPolicy("AllowedOrigins", policy =>
          policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()!)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
  });
  ```
- **HTTP Security Headers:** Enforce the following headers via middleware or Nginx/reverse proxy configuration in all non-development environments:
  - `Strict-Transport-Security: max-age=63072000; includeSubDomains; preload`
  - `X-Content-Type-Options: nosniff`
  - `X-Frame-Options: DENY`
  - `Content-Security-Policy` — define a strict policy per application requirements.
  - `Referrer-Policy: strict-origin-when-cross-origin`
- **HTTPS Enforcement:** Call `app.UseHttpsRedirection()` and `app.UseHsts()` unconditionally in non-development environments. Configure `HstsOptions.MaxAge` to at least 365 days.
- **OWASP Awareness:** Treat the OWASP Top 10 as a baseline security checklist. Copilot must not generate code that introduces SQL injection (use parameterized EF Core queries only), mass assignment vulnerabilities (never bind request bodies directly to domain entities), or insecure direct object references (always authorize resource ownership before returning data).
- **Sensitive Data:** Never log personally identifiable information (PII), authentication tokens, passwords, or payment data. Mask sensitive fields in Serilog using destructuring policies.

## Multi-Tenancy

- Enforce a **row-level multi-tenancy** strategy as the default isolation model using EF Core global query filters. Every tenant-scoped entity must include a `TenantId` property (`Guid`), and the corresponding `DbContext` must apply a global filter automatically:
  ```csharp
  modelBuilder.Entity<Invoice>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
  ```
- Resolve the current tenant via a scoped `ICurrentTenantService` implementation that extracts the `TenantId` from the validated JWT claims (`tenant_id` claim mapped via Keycloak) during request initialization.
- Enforce strict separation: never allow cross-tenant data leakage. Write architecture tests (NetArchTest) asserting that no query bypasses the global filter via `.IgnoreQueryFilters()` outside of explicitly whitelisted admin-only services.
- When a schema-per-tenant strategy is required (higher isolation, compliance-driven), bypass EF Core global filters entirely and use a connection string resolver pattern that swaps the `IDbContextFactory<T>` connection string per request based on `ICurrentTenantService`.
- Propagate `TenantId` through all background jobs, event handlers, and message consumers to ensure tenant context is preserved in async and out-of-process execution paths.

## Resilience & Fault Tolerance

- Mandate resilience pipelines on all outbound I/O using **Microsoft.Extensions.Resilience** (the modern Polly v8 wrapper shipped as part of .NET):
  ```csharp
  builder.Services.AddHttpClient<IKeycloakClient, KeycloakClient>()
      .AddStandardResilienceHandler(options =>
      {
          options.Retry.MaxRetryAttempts = 3;
          options.Retry.Delay = TimeSpan.FromMilliseconds(200);
          options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
          options.CircuitBreaker.FailureRatio = 0.5;
          options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
      });
  ```
- Apply resilience pipelines to all named `HttpClient` registrations (external APIs, Keycloak token introspection, webhook sinks). Never use `new HttpClient()` directly — always use `IHttpClientFactory`.
- Wrap EF Core database calls in resilience-aware retry strategies using the Npgsql retry-on-failure extension: `options.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null)`.
- Define explicit **timeout policies** for all integration boundaries. Default timeouts: 5 seconds for internal service calls, 10 seconds for external API calls, 30 seconds for batch or report generation operations.
- Implement the **Bulkhead Pattern** for resource-constrained downstream dependencies (e.g., a third-party payment gateway) using `ResiliencePipeline` with `ConcurrencyLimiterStrategyOptions` to prevent cascading failures from thread exhaustion.
- Implement a **fallback strategy** for non-critical degraded paths (e.g., returning a cached response when a recommendation service is unavailable) rather than propagating exceptions to the client.

## Background Jobs & Messaging

- **In-Process Background Jobs:** Use `IHostedService` or `BackgroundService` for lightweight, in-process recurring tasks (health sweeps, cache warming). Register via `builder.Services.AddHostedService<T>()`.
- **Durable Job Scheduling:** For persistent, retry-capable, and schedulable background jobs (report generation, email dispatching, data exports), use **Hangfire** with the PostgreSQL storage provider (`Hangfire.PostgreSql`). Enforce the Hangfire dashboard behind an authorization policy — never expose it publicly.
- **Message-Driven Architecture:** For cross-module or cross-service communication, enforce **MassTransit** with RabbitMQ as the default message broker. Never publish domain events via direct method calls across module boundaries — always use a message bus contract:
  ```csharp
  await _publishEndpoint.Publish<InvoicePaidEvent>(new
  {
      InvoiceId = invoice.Id,
      PaidAt = DateTimeOffset.UtcNow,
      TenantId = invoice.TenantId
  }, cancellationToken);
  ```
- **Outbox Pattern:** Enforce the Transactional Outbox Pattern for all domain event publishing to guarantee at-least-once delivery and prevent message loss on application crashes. Persist outbox messages within the same EF Core transaction as the domain state change, and use a background poller (or MassTransit's built-in outbox) to relay them to the broker asynchronously.
- **Idempotency:** All message consumers MUST be idempotent. Use a deduplication table or MassTransit's `InMemoryInboxState` / `EntityFrameworkInboxState` to detect and discard duplicate deliveries.
- **CancellationToken propagation:** Always forward the `CancellationToken` from the hosting environment (`stoppingToken` in `BackgroundService`) through to all async database and HTTP calls inside job handlers.

## Validation, Domain Guards, and Error Handling (RFC 9457)

### Core Philosophy — Errors as Values, Exceptions for the Exceptional

**Throwing exceptions for predictable failure states is banned.** Exceptions carry significant runtime overhead, obscure control flow, and make failure paths invisible to callers. Reserve `throw` exclusively for conditions that represent a programmer error or a truly unrecoverable system fault (e.g., a null argument that should never be null, an unresolvable infrastructure failure, or a concurrency violation detected by the database).

All predictable failure states — validation failures, business rule violations, not-found conditions, permission denials, invalid state transitions — must be modelled as **values** using the `Result<T>` / `Error` pattern.

| Situation | Correct approach |
|---|---|
| Input validation fails | `Result.Failure(Error.Validation(...))` |
| Entity not found | `Result.Failure(Error.NotFound(...))` |
| Business rule violated | `Result.Failure(Error.BusinessRule(...))` |
| State transition invalid | `Result.Failure(Error.Conflict(...))` |
| General / unclassified failure | `Result.Failure(Error.Failure(...))` |
| Null argument (programmer error) | `throw new ArgumentNullException(...)` |
| Database connection lost | Allow to propagate; caught by `IExceptionHandler` |
| Unhandled concurrency conflict | Allow to propagate; caught by `IExceptionHandler` |

---

### 1. The Error Type

`Error` and `ErrorType` are shipped in the **`Techbiquity.Utilities.Functional`** package. Every failure in the system is expressed as an `Error` instance — never a raw string. The key property is **`Message`** (human-readable), not `Description`.

```csharp
// Techbiquity.Utilities.Functional — ErrorType
public enum ErrorType
{
    Failure    = 0,   // General / unclassified failure — the default
    Validation = 1,   // Input validation failure
    NotFound   = 2,   // Resource not found
    Conflict   = 3,   // Conflict with existing state
    BusinessRule = 4  // Domain business rule violated
}

// Techbiquity.Utilities.Functional — Error
public sealed record Error(string Code, string Message, ErrorType Type)
{
    // Factory methods — always prefer these over calling the constructor directly
    public static Error Failure(string code, string message)     => new(code, message, ErrorType.Failure);
    public static Error Validation(string code, string message)  => new(code, message, ErrorType.Validation);
    public static Error NotFound(string code, string message)    => new(code, message, ErrorType.NotFound);
    public static Error Conflict(string code, string message)    => new(code, message, ErrorType.Conflict);
    public static Error BusinessRule(string code, string message) => new(code, message, ErrorType.BusinessRule);

    // Creates a general Failure error from an unstructured message string
    public static Error FromString(string message) => new("General.Error", message ?? string.Empty, ErrorType.Failure);

    // Implicitly converts to its Message string — useful for logging and display
    public static implicit operator string(Error e) => e?.Message ?? string.Empty;
}
```

**Key API rules:**
- The property is **`Error.Message`** — never `Error.Description`. Using `Description` will not compile.
- `ErrorType` has **no `Forbidden`, `Unexpected`, or `None` members** — use `ErrorType.Failure` for unclassified failures.
- Use `Error.FromString(message)` to wrap raw exception messages into a structured `Error` at infrastructure boundaries.
- The implicit `operator string` means an `Error` can be passed anywhere a `string` is expected — useful in logging: `_logger.LogError(error)`.

Domain-specific errors are declared as `static readonly` fields on a nested `Errors` class co-located with the entity they belong to:

```csharp
// Domain/Invoicing/Invoice.cs (partial — errors declaration)
public sealed class Invoice : AggregateRoot<InvoiceId>
{
    public static class Errors
    {
        public static readonly Error NotFound =
            Error.NotFound("Invoice.NotFound", "The invoice with the specified ID was not found.");

        public static readonly Error AlreadyPaid =
            Error.Conflict("Invoice.AlreadyPaid", "This invoice has already been paid and cannot be modified.");

        public static readonly Error InvalidAmount =
            Error.Validation("Invoice.InvalidAmount", "Invoice total must be greater than zero.");

        public static readonly Error InvalidTransition =
            Error.BusinessRule("Invoice.InvalidTransition", "The requested status transition is not permitted.");

        public static readonly Error CannotCancelPaid =
            Error.BusinessRule("Invoice.CannotCancelPaid", "A paid invoice cannot be cancelled.");
    }
}
```

---

### 2. The Result Type

`Result` and `Result<T>` are shipped in the **`Techbiquity.Utilities.Functional`** package. Do not use `Ardalis.Result` or any third-party result library. The API surface is as follows — reference this when generating handlers, domain methods, and pipeline behaviours.

```csharp
// Techbiquity.Utilities.Functional — Result (non-generic, for void operations)
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }                    // Primary error
    public IReadOnlyList<Error> Errors { get; }    // All errors (supports multi-error failures)

    // ── Factory methods ──────────────────────────────────────────────────────
    public static Result Success();
    public static Result Failure(Error error);
    public static Result Failure(IEnumerable<Error> errors); // Aggregates multiple errors

    // Convenience factories — delegate to Result<T> overloads
    public static Result<T> Success<T>(T value);
    public static Result<T> Failure<T>(Error error);
    public static Result<T> Failure<T>(IEnumerable<Error> errors);

    // ── Combinators ──────────────────────────────────────────────────────────
    // Combines multiple results — returns success if ALL succeed; aggregates all errors on any failure
    public static Result Combine(params Result[] results);

    // ── Functional helpers ───────────────────────────────────────────────────
    public T Match<T>(Func<T> onSuccess, Func<Error, T> onFailure);
    public Result OnSuccess(Action action);        // Executes action only on success; returns this
    public Result OnFailure(Action<Error> action); // Executes action only on failure; returns this
}

// Techbiquity.Utilities.Functional — Result<T> (carries a value on success)
public class Result<T> : Result
{
    public T Value { get; }  // Throws InvalidOperationException if IsFailure

    // ── Railway-oriented chaining — built-in instance methods ────────────────
    public Result<TOutput> Map<TOutput>(Func<T, TOutput> mapper);       // Transform value; propagate errors
    public Result<TOutput> Bind<TOutput>(Func<T, Result<TOutput>> binder); // Chain result-returning ops
    public TOutput Match<TOutput>(Func<T, TOutput> onSuccess, Func<Error, TOutput> onFailure);
    public T ValueOrDefault(T defaultValue = default!); // Safe access — returns default on failure

    // ── Implicit conversion ──────────────────────────────────────────────────
    public static implicit operator Result<T>(T value); // Wraps value in Result.Success<T>(value)
}
```

**Key API rules:**
- `Result<T>` inherits from `Result` — methods on the non-generic `Result` (e.g. `OnSuccess`, `OnFailure`, `Match`) are available on `Result<T>` too.
- `Errors` (plural) is always populated on failure — use it when you need all aggregated validation errors, not just the primary one.
- `Result.Combine(...)` is the correct way to validate multiple independent conditions before proceeding — do not chain `.Bind()` for independent validations that should all be evaluated.
- `Map` and `Bind` are **instance methods on `Result<T>`** — no external `ResultExtensions` class is needed.
- `ValueOrDefault(T)` is the safe alternative to `.Value` when a fallback is acceptable.

---

### 3. Railway-Oriented Chaining

Avoid deeply nested `if (result.IsFailure) return ...` checks. `Result<T>` provides built-in instance methods for chaining — no external `ResultExtensions` class is needed. Do not create a custom `ResultExtensions` class; use the methods provided by `Techbiquity.Utilities.Functional`.

```csharp
// ── Single-path chain — each step depends on the previous ───────────────────
// Map and Bind are instance methods — call them directly on Result<T>
Result<InvoiceResponse> response = await Money.Create(command.Amount, command.Currency)
    .Bind(money => Invoice.Create(CustomerId.From(command.CustomerId), money, DateTimeOffset.UtcNow))
    .Map(InvoiceResponse.FromDomain);

// ── Multi-error aggregation — use Combine for independent validations ────────
// Do NOT chain .Bind() for independent conditions — all must be evaluated
Result nameResult    = ValidateName(command.Name);
Result addressResult = ValidateAddress(command.Address);
Result emailResult   = ValidateEmail(command.Email);

Result combined = Result.Combine(nameResult, addressResult, emailResult);

if (combined.IsFailure)
    return Result.Failure<CustomerResponse>(combined.Errors); // All errors aggregated

// ── Match — eliminate if/else on result state ────────────────────────────────
string message = result.Match(
    onSuccess: value  => $"Invoice {value.Id} created successfully.",
    onFailure: error  => $"Failed: {error.Message}");

// ── OnSuccess / OnFailure — side effects without breaking the chain ──────────
result
    .OnSuccess(() => _logger.LogInformation("Invoice created."))
    .OnFailure(error => _logger.LogWarning("Invoice creation failed: {Error}", error.Message));

// ── ValueOrDefault — safe access with a fallback ─────────────────────────────
InvoiceResponse safeResponse = result.ValueOrDefault(InvoiceResponse.Empty);
```

---

### 4. Input Validation (Application Layer — FluentValidation)

Enforce input validation using **FluentValidation** on all incoming command and query DTOs. Do not mix Data Annotations with FluentValidation. Validators are registered automatically via `AddValidatorsFromAssembly` and intercepted by a MediatR pipeline behaviour before the handler executes.

```csharp
// Application/Invoicing/Commands/CreateInvoiceCommandValidator.cs
internal sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithErrorCode("Invoice.CustomerIdRequired")
            .WithMessage("A valid customer ID is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithErrorCode("Invoice.InvalidAmount")
            .WithMessage("Invoice amount must be greater than zero.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .WithErrorCode("Invoice.InvalidCurrency")
            .WithMessage("Currency must be a valid 3-letter ISO 4217 code.");
    }
}

// Application/Behaviours/ValidationPipelineBehaviour.cs
internal sealed class ValidationPipelineBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehaviour(IEnumerable<IValidator<TRequest>> validators) =>
        _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        ValidationResult[] results = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(request, cancellationToken)));

        Error[] errors = results
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .Select(f => Error.Validation(f.ErrorCode, f.ErrorMessage))
            .Distinct()
            .ToArray();

        if (errors.Length != 0)
            return CreateValidationResult<TResponse>(errors);

        return await next();
    }

    private static TResponse CreateValidationResult<TResponse>(Error[] errors)
        where TResponse : class
    {
        // Construct a Result<T> carrying all validation errors
        return (TResponse)typeof(ValidationResult<>)
            .GetGenericTypeDefinition()
            .MakeGenericType(typeof(TResponse).GenericTypeArguments[0])
            .GetMethod(nameof(ValidationResult<object>.WithErrors))!
            .Invoke(null, [errors])!;
    }
}
```

---

### 5. Mapping Results to HTTP Responses

Map `ErrorType` to HTTP status codes in a single place — a MediatR pipeline behaviour or a dedicated extension method used by all controllers and minimal API endpoints. Never scatter `if (result.IsFailure)` status-code logic across individual endpoint handlers.

```csharp
// Presentation/Extensions/ResultExtensions.cs
public static class ResultExtensions
{
    public static IResult ToProblemResult(this Result result) =>
        result.Error.Type switch
        {
            ErrorType.NotFound    => Results.NotFound(CreateProblem(result.Error, 404)),
            ErrorType.Validation  => Results.BadRequest(CreateProblem(result.Error, 400)),
            ErrorType.BusinessRule => Results.UnprocessableEntity(CreateProblem(result.Error, 422)),
            ErrorType.Conflict    => Results.Conflict(CreateProblem(result.Error, 409)),
            _                     => Results.Problem(CreateProblem(result.Error, 500))
        };

    public static IResult ToProblemResult<T>(this Result<T> result) =>
        result.IsSuccess
            ? Results.Ok(result.Value)
            : ((Result)result).ToProblemResult();

    private static ProblemDetails CreateProblem(Error error, int statusCode) =>
        new()
        {
            Title = error.Code,
            Detail = error.Message,   // Error.Message — not Description
            Status = statusCode
        };
}

// Minimal API endpoint usage
app.MapPost("/api/v1/invoices", async (
    CreateInvoiceCommand command,
    ISender sender,
    CancellationToken ct) =>
{
    Result<InvoiceResponse> result = await sender.Send(command, ct);
    return result.ToProblemResult();
});
```

---

### 6. Global Exception Handler (Truly Unexpected Failures)

The `IExceptionHandler` pipeline is the **last line of defence** for exceptions that escaped the Result pattern — infrastructure faults, unhandled concurrency conflicts, programmer errors. It must never be relied upon for normal business flow.

```csharp
// Infrastructure/Exceptions/GlobalExceptionHandler.cs
internal sealed class GlobalExceptionHandler
    : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) =>
        _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Unhandled exception {ExceptionType} on {Method} {Path}. TraceId: {TraceId}",
            exception.GetType().Name,
            httpContext.Request.Method,
            httpContext.Request.Path,
            httpContext.TraceIdentifier);

        ProblemDetails problemDetails = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Type = "https://tools.ietf.org/html/rfc9457#section-15.6.1",
            Instance = httpContext.Request.Path,
            Extensions = { ["traceId"] = httpContext.TraceIdentifier }
        };

        // Only expose detail in Development — never in Staging or Production
        if (httpContext.RequestServices
                .GetRequiredService<IHostEnvironment>()
                .IsDevelopment())
        {
            problemDetails.Detail = exception.ToString();
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}

// Program.cs registration
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

app.UseExceptionHandler();
```

---

### 7. RFC 9457 Problem Details Compliance

Enforce RFC 9457 (Problem Details) for **all** non-2xx HTTP responses across both Controller-based and Minimal API endpoints.

All error payloads must include: `type` (URI), `title` (error code), `status` (HTTP code), `detail` (human-readable description), and `instance` (mapped to `HttpContext.TraceIdentifier` for log correlation).

Multi-property FluentValidation failures are serialized into a standardized `errors` extension:

```json
{
  "type": "https://tools.ietf.org/html/rfc9457#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "instance": "/api/v1/invoices",
  "traceId": "00-84c1fd4063c38d9f3900d0673f1c2264-2e5a4ba1f0de25f0-00",
  "extensions": {
    "errors": {
      "Amount": ["Invoice amount must be greater than zero."],
      "Currency": ["Currency must be a valid 3-letter ISO 4217 code."]
    }
  }
}
```

---

### Error Handling — Banned Patterns Checklist

| Banned | Correct alternative |
|---|---|
| `throw new BusinessRuleException(...)` for predictable failures | `Result.Failure(Error.BusinessRule(...))` |
| `throw new NotFoundException(...)` | `Result.Failure(Error.NotFound(...))` |
| Returning `null` to signal absence | `Result.Failure(Entity.Errors.NotFound)` |
| Raw string error messages in `Result.Failure("some message")` | `Result.Failure(Error.Validation(code, message))` — always wrap in a typed `Error` |
| Status-code logic scattered across endpoint handlers | Centralised `result.ToProblemResult()` extension |
| Catching exceptions inside domain methods to return a value | Let infrastructure exceptions propagate; use Result only for business failures |
| `try/catch` wrapping happy-path handler logic | Result chaining — no try/catch in application handlers |

## API Versioning and OpenAPI (Scalar / Swagger)

- Mandate strict API Versioning utilizing the modern **Asp.Versioning.Http** and **Asp.Versioning.Mvc.ApiExplorer** ecosystems.
- Enforce **URL-based Versioning** as the default strategy (e.g., `/api/v{version:apiVersion}/[controller]`), while simultaneously enabling fallback Header-based detection (`x-api-version`) via an aggregate `ApiVersionReader.Combine()` definition.
- Standardize version configuration across execution styles consistently:
  - **Controller-Based:** Explicitly decorate classes using `[ApiVersion("1.0")]` and `[Route("api/v{version:apiVersion}/[controller]")]`.
  - **Minimal APIs:** Group endpoints systematically using `.NewApiVersionSet()` and bind version parameters explicitly using `.HasApiVersion()`.
- Enforce automated OpenAPI metadata generation using Microsoft's built-in OpenAPI support (`Microsoft.AspNetCore.OpenApi`) or Swashbuckle, combined with an API Explorer option that formats versions as `'v'VVV` (e.g., `v1`, `v2.0`).
- Use UI generation tools (such as **Scalar** or Swagger UI) that natively support modern OAuth 2.0 PKCE authentication authorization loops, forcing Copilot to extract descriptions directly from C# triple-slash XML documentation elements (`/// <summary>`) for endpoints, schemas, parameters, and response types.

## Logging, Tracing, and Observability (OpenTelemetry & Serilog)

- Enforce **Structured Logging** using **Serilog.AspNetCore** integrated strictly with the `Microsoft.Extensions.Logging.ILogger` abstract interface in application services.
- Mandate a **Two-Stage Bootstrap Initialization** loop within `Program.cs`. Configure a process-wide `Log.Logger` wrapped in a `try/catch/finally` block to capture initial startup or configuration-level host crashes safely before the DI container materializes.
- Standardize on **OpenTelemetry (OTel)** protocols for all telemetry collection (Traces, Metrics, and Logs) rather than coupling directly to vendor-specific SDKs (like Application Insights or Datadog). Export data cleanly via the OpenTelemetry Protocol (**OTLP**) exporter sink.
- Eliminate custom, manual correlation ID middleware pipelines; instead, leverage .NET's native, built-in **System.Diagnostics.Activity** framework:
  - Ensure that Serilog automatically enriches log events with native OpenTelemetry context fields (`TraceId` and `SpanId`).
  - Configure automatic W3C Trace Context propagation across boundary lines by tracking outbound communication calls inside `HttpClient` and Entity Framework Core pipelines.
- Standardize log density and formatting across targets. Output compact, queryable JSON schemas to standard console or file streams in high-throughput environments. Restrict log filtering levels strictly across distinct runtime profiles:
  - `Development`: `Information` level logs, with full database command rendering (`Microsoft.EntityFrameworkCore.Database.Command=Information`).
  - `Production`: `Warning` or `Error` baseline filters, overriding explicit business logic namespaces to `Information` while completely silencing framework-level telemetry noise (`Microsoft=Warning`, `System=Warning`).
- Never log PII, credentials, authentication tokens, or payment-sensitive data. Apply Serilog destructuring policies to mask sensitive properties at the sink level before any log event is emitted.

## Testing and Test-Driven Development (TDD)

- Enforce a strict **Test-Driven Development (TDD)** workflow using the Red-Green-Refactor cycle for all business logic, utilizing **xUnit**, **Shouldly** (for fluent open-source assertions), and **NSubstitute** for mocking.
- Structurally divide testing strategies into four distinct, isolated layers across the solution:

  1. **Architecture Tests (Static / Solution-Wide):**
     - Implement a dedicated test project using **NetArchTest.eNet** to programmatically enforce Clean Architecture and Modular Monolith boundaries.
     - Assert rules such as: "The Domain layer must not have dependencies on the Infrastructure, Application, or WebApi layers", "The Application layer is the only layer permitted to hold a direct project reference to Domain", "Infrastructure must not hold a direct project reference to Domain — it accesses Domain types transitively through Application", and "Modules must not directly reference internal types of other modules."
     - Assert that no query bypasses EF Core global tenant filters via `.IgnoreQueryFilters()` outside of whitelisted admin-only services.

  2. **Unit Tests (Fast / Per-Module):**
     - Test Domain Entities, Domain Guards, and Application Handlers in absolute isolation.
     - Mock all external dependencies (e.g., repositories, identity providers) using NSubstitute.

  3. **Functional / Integration Tests (Subcutaneous / Per-Module):**
     - Validate distinct business capabilities and EARS requirements by treating the module as a black box using `WebApplicationFactory<T>`.
     - Test the full HTTP pipeline execution (routing, model binding, FluentValidation, filter pipelines, and security policies).
     - Isolate external network dependencies or third-party gateways using fakes or NSubstitute, while utilizing **Testcontainers** with PostgreSQL for local database persistence layers. Do not use SQLite or In-Memory providers. Ensure mock JWT bearer tokens are injected to simulate Keycloak roles.

  4. **End-to-End (E2E) / System Tests (Black-Box / Solution-Wide Project):**
     - Validate sequential, multi-step customer journeys across the entire unified system from a pure user perspective.
     - Implement an isolated, solution-wide E2E test project that treats the fully deployed system as a complete black box.
     - Use an HTTP client to orchestrate complete, multi-step business transactions against a fully running container stack (the WebAPI, the Keycloak instance, and the live PostgreSQL DB initialized via Docker Compose) to verify real network, security, and state synchronization loops.
     - Mandate that zero mocking occurs; the execution loop must hit a live, fully running container deployment to guarantee data integrity, state serialization, and network-level security configurations pass holistically.

- Maintain clean test layout mechanics:
  - Utilize the **AAA (Arrange, Act, Assert)** structural pattern implicitly separated by single blank lines. **Never emit explicit `// Arrange`, `// Act`, or `// Assert` code comment markers.**
  - Standardize test method naming schemas using a strict `MethodUnderTarget_Condition_ExpectedBehavior` pattern (e.g., `SubmitInvoice_AmountIsZero_ThrowsDomainGuardException`).

## Performance Optimization

- Enforce proper orchestration of Asynchronous programming: mandate that all I/O-bound operations (database queries, network calls, file system updates) utilize `async` and `await` keywords top-to-bottom, ensuring proper propagation of `CancellationToken` instances across all execution layers to free up thread-pool resources during request cancellations.
- Standardize Enterprise Caching Strategies using a **Cache-Aside Pattern** via `IDistributedCache` (backed by Redis) or hybrid memory caching solutions:
  - Never implement raw caching logic inside API Controllers or Application Handlers. Wrap caching mechanics inside specialized Infrastructure Decorators or MediatR pipeline behaviors.
  - Require explicit cache invalidation mechanisms (Time-To-Live / TTL and sliding expirations) and define strict concurrency protection flags to prevent cache stampedes.
- Mandate strict data-retrieval boundaries on all collection query APIs:
  - Enforce keyset pagination or standardized offset pagination parameters (e.g., using a unified `PagedList<T>` structure containing `PageNumber` and `PageSize`).
  - Cap maximum allowable page sizes explicitly to prevent malicious or accidental unbound memory consumption.
- Optimize payload transmission pipelines: enable native ASP.NET Core response compression middleware using Brotli compression for public APIs, and enforce explicit stream-based processing (`System.Text.Json` IAsyncEnumerable streaming) when exporting large tabular datasets.
- Implement Micro-Benchmarking practices: utilize **BenchmarkDotNet** configurations when documenting or optimizing highly repetitive utility algorithms, sorting mechanisms, or parsing functions inside the core architecture.

## Deployment, Containerization, and DevOps

- Enforce proper multi-container **Dockerization** mechanics utilizing optimized multi-stage `Dockerfiles` rather than relying on native .NET CLI container publishing flags:
  - Base production runtime layers on minimalist, security-hardened images (such as **Chiseled Ubuntu** runtimes: `mcr.microsoft.com/dotnet/runtime-deps:10.0-noble-chiseled`).
  - Configure the runtime container environment to execute strictly as a **Non-Root User** to mitigate security privileges (`USER $APP_USER`).
- Mandate the orchestration of local development, dependencies, and testing pipelines using unified **Docker Compose** blueprints:
  - Isolate distinct network bridges to securely wire the core WebAPI container, the PostgreSQL database, the Redis cache, and the Keycloak Identity provider together.
  - Inject environment-specific variables, database connection strings, and OAuth2 discovery configurations seamlessly via external `.env` property sheets. Never commit `.env` files to source control — commit `.env.example` with placeholder values only.
- Standardize on **Cloud-Agnostic Container Orchestration** deployment paths:
  - Prioritize **Kubernetes** deployment manifests or **Docker Swarm** blueprints rather than coupling directly to vendor-specific platforms (like Azure App Service or AWS Elastic Beanstalk).
  - Explicitly define production-grade resource footprints inside configurations using strict container limit allocations (e.g., `resources: limits: memory: "512Mi", cpu: "1"`).
- Implement robust Liveness, Readiness, and Startup Probes utilizing the native **Microsoft.Extensions.Diagnostics.HealthChecks** ecosystem:
  - Map unique query verification endpoints consistently: `/health/live` (basic process pulse) and `/health/ready` (active network dependencies verification).
  - Inject explicit, non-blocking check dependencies to evaluate the real-time connectivity status of the PostgreSQL cluster (`Npgsql.HealthChecks`) and the Redis instance (`AspNetCore.HealthChecks.Redis`).
- Standardize automated CI/CD Workflows utilizing **GitHub Actions** or **GitLab CI**:
  - **Continuous Integration (CI):** Automate steps to trigger on every pull request to compile the code, enforce NetArchTest boundary validations, and spin up a transient docker-based integration pipeline utilizing **Testcontainers** for PostgreSQL.
  - **Continuous Deployment (CD):** Automate staging pipelines to securely package production images, stamp them with semver-compliant tags derived directly from git commits, and push them to a secure Container Registry (e.g., GitHub Packages or Docker Hub).
  - **Secret Scanning:** Integrate a secret scanning step (e.g., `trufflesecurity/trufflehog` or GitHub's native secret scanning) into every CI pipeline run to detect accidentally committed credentials before merge.

## Copilot Behavioral Guidelines

These rules govern how Copilot should behave when generating, modifying, or reviewing code in this repository.

### When to ask for clarification

- **Ask before proceeding** when a task involves modifying a public API contract, changing a database schema (adding, renaming, or dropping columns), altering authentication or authorization logic, or touching shared infrastructure configuration.
- **Ask before proceeding** when the intent of a requirement is genuinely ambiguous and two valid but mutually exclusive implementations exist.
- **Proceed without asking** for localized changes that are self-contained within a single layer and do not break existing interfaces (e.g., adding a private method, extracting a local variable, improving a log message).

### Code generation standards

- Generate the minimum code that satisfies the requirement. Do not introduce abstractions, interfaces, or design patterns that are not yet needed.
- When adding a new feature, always generate the corresponding unit test in the same response.
- When modifying existing behavior, update the existing test(s) — do not leave orphaned passing tests that no longer reflect reality.
- Never silently remove or comment out existing code during a refactor. Explicitly note what was removed and why.

### Commit message convention

Follow **Conventional Commits** format strictly:

```
<type>(<scope>): <short summary in imperative mood>

[body explaining why, not what. Use bullet points for clarity.]

[footer: Changes made by: <author>]
```

Replace `<author>` with the actual author name in the footer.

Valid types: `feat`, `fix`, `refactor`, `test`, `docs`, `chore`, `perf`, `ci`, `build`.

Example:

```
feat(invoicing): add keyset pagination to invoice list query

- Offset pagination caused full table scans on large invoice datasets,
  degrading query performance as row counts grew beyond 100k records.
- Keyset pagination anchors on the last seen InvoiceId and CreatedAt,
  eliminating the OFFSET clause and enabling index-only seeks.
- PagedList<T> updated to carry the next cursor value alongside the
  existing PageNumber and PageSize metadata.

Changes made by: John Smith
```

### Branch naming convention

```
<type>/<short-kebab-description>
```

Examples: `feat/invoice-pdf-export`, `fix/keycloak-jwks-refresh`, `chore/update-dotnet-10`.

### Pull request standards

Every PR description must include:
- **What changed** — a one-paragraph plain-English summary.
- **Why it changed** — the business or technical motivation.
- **How to test** — manual verification steps or reference to automated test coverage.
- **Breaking changes** — explicitly state `None` if not applicable.

### Working with this architecture

- The **Domain layer has zero external dependencies**. Copilot must never add NuGet package references to the Domain project that pull in infrastructure concerns.
- The **Application layer is the only layer that directly references Domain**. FluentValidation and MediatR abstractions are permitted; EF Core, Npgsql, and HTTP clients are not. Application must never define repository or unit-of-work interfaces — these are defined in Domain and consumed here via DI. Infrastructure reaches Domain only through this Application reference — it must never add its own direct Domain project reference.
- **Infrastructure references Application directly and Domain transitively** — never the other way around. Infrastructure implements the repository, unit-of-work, and seeder interfaces that are defined in the Domain layer and visible through the Application project reference. Infrastructure must never hold a direct project reference to Domain, must never define its own repository contracts, and must never expose EF Core types to Application or Domain. If Copilot adds a direct Domain project reference to Infrastructure, or generates a repository interface inside Infrastructure or Application, that is a layer violation.
- When generating EF Core migrations, always review the generated SQL in the migration snapshot before committing. Flag any migration that drops a column, renames a table, or changes a column type as a **Breaking Migration** in the PR description.
