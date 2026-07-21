# Done

This file contains completed features and tasks consolidated from the original backlog.

## COMPLETED FEATURES

- POC: Basic mapping between two objects of same type. --- DONE
- Test strongly types mapping --- DONE
- Alternative signatures
  1 - MapTo<T>(T target)    --- DONE
  2 - MapFrom<T>(T source) --- DONE
  3 - Map(object source, object Target) --- DONE
  4 - MapTo(object source, object target) --- DONE
  5 - MapFrom(object source) --- DONE
  6 - Surface MapTo<T> and MapFrom<T> methods to any arbitrary object via extension method for the generic variants --- DONE

- Map between two objects of dissimilar types. --- DONE
- MapTo(), MapFrom() --- DONE
- Surface Map methods to any arbitrary object via extension methods --- DONE
- Refactoring: Group functionally related test methods together refactor to separate classes --- DONE
- Separate MapToOfT from the lot into their own class --- DONE
- Remove extra fluff --- DONE
- POC of mapping to new object instances. DONE
- POC of mapping to to new IEnumerable<T> instances. DONE

## COMPLETED TASKS

- Test when sourceProp is null (DONE)
- Test when targetProp is null (DONE)
- Test when sourceProp.Name is not equal to targetProp.Name (DONE)
- Test when sourcePropType is not equal to targetPropType (DONE)
- Test when source object is null (DONE)
- Test when target object is null (DONE)
- Isolate predicate function for reuse. (DONE)
- POC: Setup a sample, install the nuget and test. Fix any issues. DONE


_Last updated: consolidated from docs/ObjectMapperBacklog.txt and docs/ObjectMapperTestPlan.txt_
