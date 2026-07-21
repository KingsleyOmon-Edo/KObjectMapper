# Backlog

## MVP

### Features

- Update README
- Setup issues list
- Code samples and examples
- Support for .NET 6.0

- Mapping to new, non-existent instances - SINGLE OBJECTS
- Mapping to new, non-existent instances - COLLECTIONS (IEnumerable<T>)
  - Do more robust implementations following the POCs - Single and collection instances.
  - Unit test thoroughly
  - As we are still in prerelease, review or redesign the APIs if needed

## Product Backlog - Pending Features

- Review code and fix any issues in files.
- Review and cover ALL object extension methods with protective tests.
- Mapping collections (Detailed implementation)
- Mapping between collections of dissimilar types
- Keep Extensions local
- API: return the mapped instance instead of mutating invisibly
  - var target = mapper.Map<T>(source);
  - var target = mapper.MapFrom<T>(source);
  - var target = mapper.MapFrom(source);
- Review the semantics of the MapTo<T>() and MapFrom<T>() methods
- Mapping collections (detailed)
- Support for .NET 5.0, 3.1, 4.0

## Outstanding TODOs / Action Items

- Tests for MapFrom<TSource>(TSource source)
- Tests for MapFrom<TSource, TTarget>(TSource source, TTarget target)
- Tests for MapTo(object source, object target)
- Tests for MapTo<TSource, TTarget>(TSource source, TTarget target)
- Review the ApplyDiffs signature to support two different type arguments.
- By TDD implement guard clause for null source object for MapTo().
- By TDD implement guard clause for null target object for MapTo().
- REFACTOR: Push some of ObjectExtension methods into a service class and consume by object composition.
- Validate types of source objects
- Validate types of target objects
- Validate both types when different types are supplied for each Map<TSource, TTarget>(TSource source, TTarget target)
- Validate the "type ignorance" still works.
- Tests for MapFrom<TSource>(TSource source)

## Test Plan

### Data integrity Tests
For each new method identify and test:
- Happy paths
- Sad paths (invalid inputs)
- Edge cases
- Where applicable test the types of supplied arguments

### Methods to test (ObjectExtensions / MappingService)
- ArePropertyValuesDifferent
- PropertyTypeCheck
- PropertyNameCheck
- NullCheck
- ValidateParameters
- TypeChecks
- ApplyDiffs
- WriteToProperties

TODOs for tests:
- Test predicate function happy path
- Test predicate function sad path
- Test predicate function edge cases
- Substitute the now functional GetPropertyDiffs method for the implementations used in the mutation methods.


---

_Last updated: consolidated from docs/ObjectMapperBacklog.txt and docs/ObjectMapperTestPlan.txt_
