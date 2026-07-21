# Refactoring for NuGet Libraries

> This guide adapts refactoring principles for NuGet class library projects, with emphasis on maintaining backward compatibility, preserving the public API, and improving code clarity without breaking user code.

## When to Refactor

**Good times to refactor:**
- After making tests pass (the third step of Red-Green-Refactor)
- When you notice duplicated code or logic
- When a method becomes too complex or too long (>50 lines)
- When variable/method names no longer reflect their purpose
- When internal structure can be simplified without changing behavior

**Avoid refactoring when:**
- There are failing tests (fix those first)
- You're not sure what the code does (write tests first)
- You're trying to fix a bug (write a failing test, then fix it)
- The refactoring would change the public API

## Fundamental Principle: Never Change Public Behavior

**The Core Rule:** Refactoring must not alter the observable behavior of public methods or types.

```csharp
// ❌ WRONG: This is not refactoring; this is changing behavior
public void MapTo(object source, object target)
{
	// Changed: skips properties that don't exist in target
	// This breaks existing code expecting an exception
}

// ✅ CORRECT: Same behavior, clearer code
public void MapTo(object source, object target)
{
	ValidateInputs(source, target);
	ApplyPropertyMapping(source, target);
}
```

## Safe Refactoring Techniques

### 1. Extract Method

**Goal:** Break a large method into smaller, named helper methods.

**Before:**
```csharp
public void MapTo(object source, object target)
{
	source = source ?? throw new ArgumentNullException(nameof(source));
	target = target ?? throw new ArgumentNullException(nameof(target));

	if (source.GetType() != target.GetType())
		throw new ArgumentException("Types must match");

	PropertyInfo[] sourceProps = source.GetType().GetProperties();
	PropertyInfo[] targetProps = target.GetType().GetProperties();

	foreach (PropertyInfo sourceProp in sourceProps)
	{
		PropertyInfo targetProp = null;
		foreach (PropertyInfo tp in targetProps)
		{
			if (tp.Name == sourceProp.Name)
			{
				targetProp = tp;
				break;
			}
		}

		if (targetProp != null && targetProp.CanWrite)
		{
			object value = sourceProp.GetValue(source);
			targetProp.SetValue(target, value);
		}
	}
}
```

**After:**
```csharp
public void MapTo(object source, object target)
{
	ValidateInputs(source, target);
	ApplyPropertyMapping(source, target);
}

private static void ValidateInputs(object source, object target)
{
	source = source ?? throw new ArgumentNullException(nameof(source));
	target = target ?? throw new ArgumentNullException(nameof(target));

	if (source.GetType() != target.GetType())
		throw new ArgumentException("Types must match");
}

private static void ApplyPropertyMapping(object source, object target)
{
	PropertyInfo[] sourceProps = source.GetType().GetProperties();
	PropertyInfo[] targetProps = target.GetType().GetProperties();

	foreach (PropertyInfo sourceProp in sourceProps)
	{
		PropertyInfo targetProp = FindMatchingProperty(targetProps, sourceProp);
		if (targetProp is not null && targetProp.CanWrite)
		{
			object value = sourceProp.GetValue(source);
			targetProp.SetValue(target, value);
		}
	}
}

private static PropertyInfo FindMatchingProperty(PropertyInfo[] targetProps, PropertyInfo sourceProp)
{
	foreach (PropertyInfo targetProp in targetProps)
	{
		if (targetProp.Name == sourceProp.Name)
			return targetProp;
	}
	return null;
}
```

**Why:** Makes the code more readable, easier to test, and easier to understand at a glance.

### 2. Rename for Clarity

**Goal:** Give variables, parameters, and methods names that better reflect their purpose.

**Before:**
```csharp
public void Map(object s, object t)
{
	var sp = s.GetType().GetProperties();
	var tp = t.GetType().GetProperties();
	// ...
}
```

**After:**
```csharp
public void MapTo(object source, object target)
{
	PropertyInfo[] sourceProperties = source.GetType().GetProperties();
	PropertyInfo[] targetProperties = target.GetType().GetProperties();
	// ...
}
```

**⚠️ Note:** Public method names are part of the API contract. Renaming them is a breaking change. Only rename internal helpers.

### 3. Replace Magic Numbers and Strings

**Goal:** Use named constants to replace "magic" literals.

**Before:**
```csharp
if (items.Count > 1000)
{
	// Handle large collection
}

string message = $"Error mapping {items.Count} items";
```

**After:**
```csharp
private const int MaxItemsForDirectProcessing = 1000;

if (items.Count > MaxItemsForDirectProcessing)
{
	// Handle large collection
}

string message = $"Error mapping {items.Count} items";
```

### 4. Simplify Complex Conditions

**Goal:** Use pattern matching, early returns, or guard clauses to flatten nested conditions.

**Before:**
```csharp
public bool CanMap(object source, object target)
{
	if (source != null)
	{
		if (target != null)
		{
			if (source.GetType() == target.GetType())
			{
				return true;
			}
		}
	}
	return false;
}
```

**After:**
```csharp
public bool CanMap(object source, object target)
{
	if (source is null || target is null)
		return false;

	return source.GetType() == target.GetType();
}

// Or, even more concise:
public bool CanMap(object source, object target) =>
	source is not null &&
	target is not null &&
	source.GetType() == target.GetType();
```

### 5. Use Switch Expressions Over Complex If-Else

**Goal:** Replace multi-branch if-else logic with switch expressions for clarity.

**Before:**
```csharp
public string GetMappingStrategy(object source)
{
	if (source is null)
		return "Error";
	if (source is IEnumerable)
		return "CollectionMapping";
	if (source.GetType().IsValueType)
		return "ValueTypeMapping";
	return "DefaultMapping";
}
```

**After:**
```csharp
public string GetMappingStrategy(object source) => source switch
{
	null => "Error",
	IEnumerable => "CollectionMapping",
	{ } when source.GetType().IsValueType => "ValueTypeMapping",
	_ => "DefaultMapping"
};
```

### 6. Cache Repeated Calculations

**Goal:** Store the result of expensive operations in a variable.

**Before:**
```csharp
foreach (PropertyInfo prop in source.GetType().GetProperties())
{
	if (prop.PropertyType.IsPublic)
	{
		// ...
	}
}

// Later in the same method
foreach (PropertyInfo prop in source.GetType().GetProperties())
{
	// Process again
}
```

**After:**
```csharp
PropertyInfo[] sourceProps = source.GetType().GetProperties();

foreach (PropertyInfo prop in sourceProps)
{
	if (prop.PropertyType.IsPublic)
	{
		// ...
	}
}

// Later, reuse the cached value
foreach (PropertyInfo prop in sourceProps)
{
	// Process again
}
```

### 7. Extract Constants to Named Fields

**Goal:** Replace inline values with descriptive constant names.

**Before:**
```csharp
private void LogMapping(string operation)
{
	Console.WriteLine($"[MAP] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} {operation}");
}
```

**After:**
```csharp
private const string LogTimestampFormat = "yyyy-MM-dd HH:mm:ss";
private const string LogPrefix = "[MAP]";

private void LogMapping(string operation)
{
	Console.WriteLine($"{LogPrefix} {DateTime.UtcNow.ToString(LogTimestampFormat)} {operation}");
}
```

## Refactoring Workflow

### Step 1: Verify Tests Pass

```bash
dotnet test
```

**If any tests fail, stop and fix them first. Never refactor with failing tests.**

### Step 2: Choose a Refactoring Technique

Identify which technique applies to the code you want to improve (Extract Method, Rename, etc.).

### Step 3: Apply the Refactoring

Make the change carefully. Use your IDE's refactoring tools when available; they're less error-prone than manual edits.

### Step 4: Run Tests Immediately

```bash
dotnet test
```

**If tests fail, revert the refactoring and try a different approach.**

### Step 5: Verify Behavior Hasn't Changed

- No public method signatures changed.
- No public types removed or renamed.
- All tests still pass.
- Performance characteristics similar or improved.

## Refactoring Anti-Patterns

### ❌ Don't Mix Refactoring with Feature Development

```csharp
// WRONG: Refactoring AND adding new parameter
public void MapTo(object source, object target, bool includeReadOnlyProperties)
{
	// Old code refactored + new behavior
}
```

Better: First refactor the old code, then (in a separate step/commit) add the new feature.

### ❌ Don't Change Public API During "Refactoring"

```csharp
// WRONG: This is not refactoring; this is a breaking change
public void Map(object source, object target) // Was MapTo()
{
}
```

### ❌ Don't Refactor Before Tests Exist

```csharp
// WRONG: No tests to verify behavior is preserved
public void ComplexMethod() { /* ... */ }
// Then you refactor without test coverage → risk of hidden bugs
```

Better: Write tests first (`MapperTests`), then refactor with confidence.

### ❌ Don't Optimize Prematurely

```csharp
// WRONG: Added caching without profile evidence
private Dictionary<string, PropertyInfo[]> _propertyCache = new();
public void MapTo(object source, object target)
{
	// Uses cache now
}
```

Better: Profile first, identify bottleneck, then optimize if needed.

## Refactoring Checklist

Before and after each refactoring:

- [ ] All tests pass before starting.
- [ ] Change is isolated to one technique (one extract, one rename, etc.).
- [ ] All tests pass after the change.
- [ ] No public API changed.
- [ ] Code is clearer or simpler than before.
- [ ] Performance is same or improved.
- [ ] Commit message describes what was refactored, not why (implementation detail).

## Example: Full Refactoring Cycle

**Starting point:** Tests pass, but method is too long and hard to understand.

```csharp
public void MapTo(object source, object target)
{
	source = source ?? throw new ArgumentNullException(nameof(source));
	target = target ?? throw new ArgumentNullException(nameof(target));

	PropertyInfo[] sourceProps = source.GetType().GetProperties();
	PropertyInfo[] targetProps = target.GetType().GetProperties();

	foreach (PropertyInfo sourceProp in sourceProps)
	{
		PropertyInfo targetProp = null;
		foreach (PropertyInfo tp in targetProps)
		{
			if (tp.Name == sourceProp.Name && tp.CanWrite)
			{
				targetProp = tp;
				break;
			}
		}

		if (targetProp != null)
		{
			targetProp.SetValue(target, sourceProp.GetValue(source));
		}
	}
}
```

**Step 1:** Extract validation into a helper method.

```csharp
public void MapTo(object source, object target)
{
	ValidateInputs(source, target);
	ApplyPropertyMapping(source, target);
}

private static void ValidateInputs(object source, object target)
{
	source = source ?? throw new ArgumentNullException(nameof(source));
	target = target ?? throw new ArgumentNullException(nameof(target));
}

private static void ApplyPropertyMapping(object source, object target)
{
	// ... rest of logic
}
```

**Step 2:** Extract the inner loop into a helper and rename variables for clarity.

```csharp
private static void ApplyPropertyMapping(object source, object target)
{
	PropertyInfo[] sourceProperties = source.GetType().GetProperties();
	PropertyInfo[] targetProperties = target.GetType().GetProperties();

	foreach (PropertyInfo sourceProperty in sourceProperties)
	{
		PropertyInfo targetProperty = FindWritableTargetProperty(targetProperties, sourceProperty);
		if (targetProperty is not null)
		{
			CopyPropertyValue(sourceProperty, targetProperty, source, target);
		}
	}
}

private static PropertyInfo FindWritableTargetProperty(PropertyInfo[] targetProperties, PropertyInfo sourceProperty)
{
	return targetProperties.FirstOrDefault(tp =>
		tp.Name == sourceProperty.Name && tp.CanWrite);
}

private static void CopyPropertyValue(PropertyInfo sourceProperty, PropertyInfo targetProperty, object source, object target)
{
	object value = sourceProperty.GetValue(source);
	targetProperty.SetValue(target, value);
}
```

**Result:** Clearer, more testable, easier to understand, all tests still pass.

## References

- **Refactoring techniques:** https://refactoring.com/catalog/
- **Clean Code principles:** Robert C. Martin, "Clean Code" (Prentice Hall, 2008)

---

_Refactor regularly in small, safe steps to keep your library clean, maintainable, and trustworthy._
