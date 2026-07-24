using System.Reflection;
using KObjectMapper;
using KObjectMapper.UnitTests.Helpers;
using Shouldly;

namespace KObjectMapper.UnitTests;

public class MappingServiceTests
{
    [Fact]
    public void ApplyDiffs_DifferentPropertyValues_UpdatesTargetInstance()
    {
        MappingService mappingService = MappingService.Create();
        Customer source = ObjectMother.SampleCustomer;
        Customer target = new()
        {
            Id = source.Id,
            FirstName = "Before",
            LastName = source.LastName,
            PhoneNumber = source.PhoneNumber
        };

        mappingService.ApplyDiffs(source, target);

        target.FirstName.ShouldBe(source.FirstName);
    }

    [Fact]
    public void GetPropertyDiffs_DifferentPropertyValues_ReturnsTheDifferentProperties()
    {
        MappingService mappingService = MappingService.Create();
        Customer source = ObjectMother.SampleCustomer;
        Customer target = new()
        {
            Id = source.Id,
            FirstName = "Before",
            LastName = source.LastName,
            PhoneNumber = source.PhoneNumber
        };

        List<PropertyInfo> diffs = mappingService.GetPropertyDiffs(source, target);

        diffs.Select(diff => diff.Name).ShouldContain(nameof(Customer.FirstName));
    }
}
