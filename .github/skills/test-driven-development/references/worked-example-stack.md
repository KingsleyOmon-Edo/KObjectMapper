# Worked Example — Building a Stack Test-First

This walks through the full loop from the main `SKILL.md` against a single concrete unit: a simple, unbounded last-in-first-out stack with `Push`, `Pop`, `Peek`, and `Count`. Each Green-phase step below is explicitly labeled with which of the three strategies from `SKILL.md`'s GREEN section it uses — `Fake It`, `Triangulate`, or `Obvious Implementation` — so you can see all three in context, not just in the abstract. The names and exact code below are illustrative — adapt the assertion library, naming convention, and test framework specifics to whatever this project actually uses.

## Behavior list (written before any code)

- An empty stack reports a count of zero.
- Pushing one item makes the count one.
- Pushing several items makes the count match how many were pushed.
- Popping an item reduces the count.
- Popping an item returns the item that was pushed.
- Popping several items returns them most-recently-pushed-first.
- Popping an empty stack fails in some well-defined way (to be decided).
- (Added mid-stream, after Push/Pop existed) Peek returns the top item without removing it.
- (Added mid-stream) Peek doesn't change the count.
- (Added mid-stream) Peek is idempotent — calling it repeatedly returns the same item each time.
- (Added mid-stream) Peeking an empty stack fails the same way Pop does.

## Step 1 — empty stack, count zero

Test first, written as though `Stack` and `Count` already exist:

```csharp
[Fact]
public void NewStack_CountIsZero()
{
    var stack = new Stack<int>();

    var count = stack.Count;

    Assert.Equal(0, count);
}
```

This doesn't compile — there's no `Stack<T>` type yet. The smallest change that makes it compile *and* pass:

```csharp
public class Stack<T>
{
    public int Count { get; private set; }
}
```

That's the entire implementation for this step. Nothing about storage yet — no test has demanded it.

## Step 2 — pushing one item (Fake It)

```csharp
[Fact]
public void PushOnce_CountIsOne()
{
    var stack = new Stack<int>();

    stack.Push(7);

    Assert.Equal(1, stack.Count);
}
```

Doesn't compile (`Push` doesn't exist). Smallest step to compile:

```csharp
public void Push(T item) { }
```

Now it compiles but fails — `Count` is still zero. The smallest change that passes *this specific test* is to hard-code the outcome — this is `Fake It`, used deliberately:

```csharp
public void Push(T item) => Count = 1;
```

This looks obviously wrong for a general stack, and that's fine — no test yet distinguishes "always 1" from "increments." The next test will force the distinction via `Triangulate`.

## Step 3 — pushing several items forces a real counter (Triangulate)

```csharp
[Fact]
public void PushThreeTimes_CountIsThree()
{
    var stack = new Stack<int>();

    stack.Push(1);
    stack.Push(2);
    stack.Push(3);

    Assert.Equal(3, stack.Count);
}
```

This fails immediately (`Count` hard-codes to 1). The hard-coded shortcut from Step 2 is exactly the kind of thing a second, differently-shaped test exists to break — two examples with different expected results now triangulate the real implementation:

```csharp
public void Push(T item) => Count++;
```

Both tests pass now. Notice this could also have been written directly via `Obvious Implementation` — incrementing a counter on `Push` is obvious enough that `Fake It`/`Triangulate` wasn't strictly necessary here. Both steps are shown to illustrate the technique; in practice, skip straight to `Obvious Implementation` whenever you're this confident.

## Step 4 — popping reduces the count (Obvious Implementation)

```csharp
[Fact]
public void PushThenPop_CountReturnsToZero()
{
    var stack = new Stack<int>();
    stack.Push(7);

    stack.Pop();

    Assert.Equal(0, stack.Count);
}
```

This is confident, obvious territory — no need to fake anything first:

```csharp
public void Pop() => Count--;
```

## Step 5 — popping must return the item (Fake It, with a twist)

```csharp
[Fact]
public void PushThenPop_ReturnsThePushedItem()
{
    var stack = new Stack<int>();
    stack.Push(7);

    var result = stack.Pop();

    Assert.Equal(7, result);
}
```

Doesn't compile — `Pop` currently returns `void`. Smallest change:

```csharp
public T Pop()
{
    Count--;
    return default!; // clearly a placeholder — no test has forced real storage yet
}
```

Note this doesn't pass for a generic `default!` against an expected `7` — at this point you'd temporarily hard-code the literal single value under test if the language/type allows it, mirroring Step 2's approach, and let the next test force real storage. For a generic `Stack<T>` this shortcut is less natural than in a concrete `int`-only stack — which is itself a useful observation: sometimes the "smallest possible fake" is smaller for a concrete type than a generic one, and that's fine; do whichever is genuinely smallest for the code you're actually writing.

## Step 6 — multiple pushes/pops force real storage (Triangulate)

```csharp
[Fact]
public void PushThree_PopThree_ReturnsThemMostRecentFirst()
{
    var stack = new Stack<int>();
    stack.Push(1);
    stack.Push(2);
    stack.Push(3);

    Assert.Equal(3, stack.Pop());
    Assert.Equal(2, stack.Pop());
    Assert.Equal(1, stack.Pop());
}
```

No hard-coded return value can satisfy this — three different expected values come back from the same method with no arguments. This is the test that earns its keep by making a fake implementation impossible to sustain. Real storage, finally:

```csharp
public class Stack<T>
{
    private readonly List<T> _items = new();

    public int Count => _items.Count;

    public void Push(T item) => _items.Add(item);

    public T Pop()
    {
        var lastIndex = Count - 1;
        var item = _items[lastIndex];
        _items.RemoveAt(lastIndex);
        return item;
    }
}
```

All six tests pass. Notice `Count` is now a pass-through to `_items.Count` rather than a manually tracked field — this single change satisfies every existing test without any of them needing to be rewritten, which is exactly what should happen when tests describe behavior rather than implementation.

## Step 7 — deciding how Pop should fail on an empty stack

Calling `Pop()` on an empty stack right now throws whatever `List<T>` throws for a negative index — an implementation detail leaking through, with a message that means nothing to a caller of `Stack<T>`. This is the error-condition design decision called out in the main skill file: throw, return a nullable, or a `TryPop` pattern. Suppose the decision is to throw a specific, well-defined exception.

```csharp
[Fact]
public void PopEmptyStack_ThrowsWithClearMessage()
{
    var stack = new Stack<int>();

    var ex = Record.Exception(() => stack.Pop());

    Assert.IsType<InvalidOperationException>(ex);
    Assert.Equal("Cannot pop an empty stack.", ex.Message);
}
```

This fails — the real exception type and message don't match. Fix:

```csharp
public T Pop()
{
    if (Count == 0)
        throw new InvalidOperationException("Cannot pop an empty stack.");

    var lastIndex = Count - 1;
    var item = _items[lastIndex];
    _items.RemoveAt(lastIndex);
    return item;
}
```

## Step 8 onward — Peek, added after real usage surfaces the need

Imagine consumers of this stack come back wanting to look at the top item without removing it. New behaviors get added to the list, and each goes through the same loop: smallest compile, smallest pass, force generality with a second contradicting test, then a dedicated empty-stack failure test — exactly steps 2 through 7 repeated for `Peek`. The interesting new test is the one that forces `Peek` to actually read storage rather than fake it:

```csharp
[Fact]
public void Peek_CalledRepeatedly_AlwaysReturnsTheSameTopItem()
{
    var stack = new Stack<int>();
    stack.Push(1);
    stack.Push(2);

    var first = stack.Peek();
    var second = stack.Peek();

    Assert.Equal(2, first);
    Assert.Equal(2, second);
}
```

A hard-coded `Peek` returning a constant would pass this particular test — it's the earlier "does `Peek` return what was actually pushed" test, run against varied pushed values, that rules out a constant. This is a reminder that a single test rarely proves an implementation is real; look at the *set* of tests together.

## Step 9 — refactoring once behaviors are complete (Reconcile Differences + Extract Method)

With `Pop` and `Peek` both implemented, they duplicate three things: checking for empty, computing the last index, and returning the item. Only `Pop` removes it. With the full test suite passing, this can be consolidated:

```csharp
public T Pop() => TakeTopItem(remove: true);
public T Peek() => TakeTopItem(remove: false);

private T TakeTopItem(bool remove)
{
    var lastIndex = Count - 1;
    if (lastIndex < 0)
        throw new InvalidOperationException("Cannot access an empty stack.");

    var item = _items[lastIndex];
    if (remove)
        _items.RemoveAt(lastIndex);

    return item;
}
```

Run the whole suite. If it's still Green, the refactor was legitimate — behavior is unchanged, structure improved, duplication removed. If anything failed, this wasn't a refactor; back it out and reconsider as a deliberate behavior change starting from a new test.

## Step 10 — organizing the tests by shared context

Once there are enough tests, group them by the state they start from rather than leaving them as one flat list:

```csharp
public class StackTests
{
    public class GivenAnEmptyStack
    {
        private readonly Stack<int> _stack = new();

        [Fact]
        public void CountIsZero() => Assert.Equal(0, _stack.Count);

        [Fact]
        public void PopThrows() =>
            Assert.Throws<InvalidOperationException>(() => _stack.Pop());
    }

    public class GivenAStackWithThreeItems
    {
        private readonly Stack<int> _stack = new();

        public GivenAStackWithThreeItems()
        {
            _stack.Push(1);
            _stack.Push(2);
            _stack.Push(3);
        }

        [Fact]
        public void CountIsThree() => Assert.Equal(3, _stack.Count);

        [Fact]
        public void PopReturnsItemsMostRecentFirst()
        {
            Assert.Equal(3, _stack.Pop());
            Assert.Equal(2, _stack.Pop());
            Assert.Equal(1, _stack.Pop());
        }
    }
}
```

Each nested class's constructor establishes the shared starting state; each `[Fact]` inside only needs to express what's different about that one behavior. When a future feature is added, revisit each nested context in turn and decide what the new behavior should do there — the grouping doubles as a checklist.
