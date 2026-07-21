# TDD for NuGet Libraries - Practical Supplement

> This supplement adapts Test-Driven Development practices for NuGet class library projects. Refer to `.github/skills/test-driven-development/SKILL.md` for the core TDD philosophy. This document focuses on library-specific test patterns, API validation, and backward compatibility concerns.

## Library-Specific Test Priorities

### 1. Public API Contracts

**Every public method must be tested for:**

- **Valid inputs:** Happy path, normal usage
- **Null/invalid inputs:** Parameter validation and exception handling
- **Edge cases:** Empty collections, boundary values, extreme inputs
- **Return correctness:** Output types, values, side effects

**Example test structure:**

```csharp
public class MapperTests
{
	// Happy path
	[Fact]
	public void MapTo_WithValidObjects_CopiesAllMatchingPublicProperties()
	{
		// Arrange
		var mapper = new Mapper();
		var source = new Customer { Id = 1, Name = "Alice", Email = "alice@example.com" };
		var target = new Customer();

		// Act
		mapper.MapTo(source, target);

		// Assert
		Assert.Equal(1, target.Id);
		Assert.Equal("Alice", target.Name);
		Assert.Equal("alice@example.com", target.Email);
	}

	// Null handling
	[Fact]
	public void MapTo_WhenSourceIsNull_ThrowsArgumentNullException()
	{
		var mapper = new Mapper();
		var target = new Customer();

		var exception = Assert.Throws<ArgumentNullException>(() => mapper.MapTo(null, target));
		Assert.Equal("source", exception.ParamName);
	}

	[Fact]
	public void MapTo_WhenTargetIsNull_ThrowsArgumentNullException()
	{
		var mapper = new Mapper();
		var source = new Customer();

		var exception = Assert.Throws<ArgumentNullException>(() => mapper.MapTo(source, null));
		Assert.Equal("target", exception.ParamName);
	}

	// Edge cases
	[Fact]
	public void MapTo_WithEmptySourceObject_DoesNotThrow()
	{
		var mapper = new Mapper();
		var source = new { };
		var target = new Customer();

		mapper.MapTo(source, target); // Should not throw
	}

	// Behavior verification
	[Fact]
	public void MapTo_BetweenDifferentTypes_MapsMatchingPropertyNames()
	{
		var mapper = new Mapper();
		var source = new Employee { Id = 5, FirstName = "Bob", LastName = "Smith" };
		var target = new Customer { FirstName = "", LastName = "" };

		mapper.MapTo(source, target);

		Assert.Equal(5, target.Id);
		Assert.Equal("Bob", target.FirstName);
		Assert.Equal("Smith", target.LastName);
	}
}
```

### 2. Error Handling and Validation

**Always validate public method parameters:**

```csharp
public void PublicMethod(object parameter)
{
	parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
	// ... implementation
}
```

**Test each validation scenario:**

```csharp
[Fact]
public void PublicMethod_WithNullParameter_ThrowsArgumentNullException()
{
	var instance = new MyClass();

	var exception = Assert.Throws<ArgumentNullException>(() => instance.PublicMethod(null));
	Assert.Equal("parameter", exception.ParamName);
}

[Theory]
[InlineData(0)]
[InlineData(-1)]
public void PublicMethod_WithInvalidCount_ThrowsArgumentException(int invalidCount)
{
	var instance = new MyClass();

	var exception = Assert.Throws<ArgumentException>(() => instance.PublicMethod(invalidCount));
	Assert.Contains("count must be greater than zero", exception.Message);
}
```

### 3. Collection and Enumerable Handling

**Test collection methods thoroughly:**

```csharp
[Fact]
public void MapCollection_WithEmptyCollection_ReturnsEmptyResult()
{
	var mapper = new Mapper();
	var emptySource = new List<Customer>();

	var result = mapper.Map<Customer, CustomerDto>(emptySource);

	Assert.Empty(result);
}

[Fact]
public void MapCollection_WithMultipleItems_MapsAllItems()
{
	var mapper = new Mapper();
	var source = new List<Customer>
	{
		new() { Id = 1, Name = "Alice" },
		new() { Id = 2, Name = "Bob" }
	};

	var result = mapper.Map<Customer, CustomerDto>(source).ToList();

	Assert.Equal(2, result.Count);
	Assert.All(result, item => Assert.NotNull(item.Name));
}

[Fact]
public void MapCollection_WhenSourceIsNull_ThrowsArgumentNullException()
{
	var mapper = new Mapper();

	Assert.Throws<ArgumentNullException>(() => 
		mapper.Map<Customer, CustomerDto>(null)
	);
}
```

### 4. Interface and Extension Method Testing

**For extension methods on public interfaces:**

```csharp
[Fact]
public void MapTo_AsExtensionMethod_MapsPropertiesCorrectly()
{
	var source = new Customer { Id = 1, Name = "Charlie" };
	var target = new Customer();

	source.MapTo(target);

	Assert.Equal(1, target.Id);
	Assert.Equal("Charlie", target.Name);
}

[Fact]
public void MapTo_CalledOnNull_ThrowsNullReferenceException()
{
	Customer nullSource = null;
	var target = new Customer();

	Assert.Throws<NullReferenceException>(() => nullSource.MapTo(target));
}
```

### 5. Backward Compatibility Checks

**When modifying existing public methods, test old behavior alongside new:**

```csharp
// Original behavior must still work after modification
[Fact]
public void MapTo_OriginalSignature_StillWorks()
{
	var mapper = new Mapper();
	var source = new Customer { Id = 1 };
	var target = new Customer();

	mapper.MapTo(source, target); // Original call style

	Assert.Equal(1, target.Id);
}

// New behavior also works
[Fact]
public void MapTo_WithNewConfiguration_UsesNewBehavior()
{
	var mapper = new Mapper(new MapperOptions { });
	var source = new Customer { Id = 1 };
	var target = new Customer();

	mapper.MapTo(source, target); // New call style with options

	Assert.Equal(1, target.Id);
}
```

## Test Organization for Libraries

### File Structure

```
tests/
├── KObjectMapperTests/
│   ├── AbstractionsTests/
│   │   └── IObjectMapperTests.cs
│   ├── ExtensionsTests/
│   │   ├── MapperExtensionsTests.cs
│   │   └── LinqExtensionsTests.cs
│   ├── MapperTests.cs
│   ├── MappingServiceTests.cs
│   └── Helpers/
│       ├── Customer.cs
│       ├── CustomerDto.cs
│       └── ObjectMother.cs
```

### Naming Conventions

**Class names:** `{ClassUnderTest}Tests`
- `MapperTests`
- `MappingServiceTests`
- `MapperExtensionsTests`

**Method names:** `{MethodUnderTest}_{Condition}_{ExpectedBehavior}`
- `MapTo_WhenSourceIsNull_ThrowsArgumentNullException`
- `Map_WithEmptyCollection_ReturnsEmptyList`
- `MapFrom_BetweenDifferentTypes_MapsMatchingProperties`

### Assertion Style

Use xUnit with FluentAssertions for readability:

```csharp
// Less readable
Assert.NotNull(result);
Assert.Equal(5, result.Count);
Assert.True(result.All(x => x.IsValid));

// More readable
result.Should().NotBeNull();
result.Should().HaveCount(5);
result.Should().AllSatisfy(x => x.IsValid.Should().BeTrue());
```

## Test Data Management

### Using ObjectMother for Common Test Data

```csharp
public static class ObjectMother
{
	public static Customer CreateValidCustomer(int id = 1, string name = "TestCustomer")
	{
		return new Customer { Id = id, Name = name, Email = "test@example.com" };
	}

	public static List<Customer> CreateCustomerList(int count = 3)
	{
		return Enumerable.Range(1, count)
			.Select(i => CreateValidCustomer(id: i, name: $"Customer{i}"))
			.ToList();
	}
}

// Usage in test
[Fact]
public void MapCollection_WithTypicalData_MapsSuccessfully()
{
	var mapper = new Mapper();
	var source = ObjectMother.CreateCustomerList(5);

	var result = mapper.Map<Customer, CustomerDto>(source);

	result.Should().HaveCount(5);
}
```

## Public API Coverage Checklist

For each public type and method, ensure tests cover:

- [ ] **Null inputs:** All reference parameters
- [ ] **Invalid inputs:** Wrong types, negative numbers, empty strings
- [ ] **Edge cases:** Empty collections, single items, extreme values
- [ ] **Normal behavior:** Happy path with realistic data
- [ ] **Return values:** Correct types, expected values
- [ ] **Exceptions:** Correct exception type and message
- [ ] **Extension methods:** Both as methods and as extensions
- [ ] **Generics:** Different type combinations (if applicable)

## Running Tests

```bash
# Run all tests
dotnet test

# Run tests in a specific project
dotnet test tests/KObjectMapperTests

# Run tests matching a pattern
dotnet test --filter "MapperTests"

# Run with code coverage
dotnet test /p:CollectCoverage=true
```

## Test Naming Examples

### Happy Path
- `MapTo_WithValidObjects_MapsAllMatchingProperties`
- `Map_WithTypicalSource_ReturnsValidTarget`
- `MapCollection_WithMultipleItems_MapsAllSuccessfully`

### Error Handling
- `MapTo_WhenSourceIsNull_ThrowsArgumentNullException`
- `MapTo_WhenTargetIsNull_ThrowsArgumentNullException`
- `Map_WithIncompatibleTypes_ThrowsInvalidOperationException`

### Edge Cases
- `Map_WithEmptyCollection_ReturnsEmptyResult`
- `MapTo_WithPropertiesHavingNullValues_CopiesNullsSuccessfully`
- `Map_WithSingleItem_MapsCorrectly`

### Behavior Verification
- `MapTo_BetweenDifferentTypes_MapsOnlyMatchingPropertyNames`
- `Map_WhenTargetPropertyIsReadOnly_SkipsProperty`
- `MapFrom_AfterMapTo_ResultsAreConsistent`

---

_Apply these library-specific practices alongside the core TDD workflow for robust, well-tested NuGet libraries._
