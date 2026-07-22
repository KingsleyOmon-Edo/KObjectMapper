using KObjectMapper;
using KObjectMapperTests.Helpers;
using Shouldly;

namespace KObjectMapperTests.CharacterizationTests;

public class CreationOverloadSadPathTests
{
    [Fact]
    public void MapCreationSingleObject_WhenSourceIsNull_ThrowsArgumentNullException()
    {
        Mapper sut = Mapper.Create();
        Customer? source = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => sut.Map<Customer?, CustomerDto>(source));

        exception.ParamName.ShouldBe("source");
    }

    [Fact]
    public void MapCreationCollection_WhenSourcesIsNull_ThrowsArgumentNullException()
    {
        Mapper sut = Mapper.Create();
        IEnumerable<Customer>? sources = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => sut.Map<Customer, CustomerDto>(sources!));

        exception.ParamName.ShouldBe("sources");
    }

    [Fact]
    public void MapCreationCollection_WhenSourceElementIsNull_ThrowsArgumentNullException()
    {
        Mapper sut = Mapper.Create();
        List<Customer?> sources =
        [
            ObjectMother.SampleCustomer,
            null
        ];

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => sut.Map<Customer?, CustomerDto>(sources));

        exception.ParamName.ShouldBe("source");
    }

    [Fact]
    public void MapCreationSingleObject_WhenTargetConstructorParametersCannotBeMatched_ThrowsInvalidOperationException()
    {
        Mapper sut = Mapper.Create();
        ConstructorNameMismatchSource source = new()
        {
            FirstName = "James"
        };

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            sut.Map<ConstructorNameMismatchSource, ConstructorNameMismatchTarget>(source));

        exception.Message.ShouldContain(nameof(ConstructorNameMismatchTarget));
    }

    [Fact]
    public void MapCreationSingleObject_WhenTargetConstructorParameterConversionFails_ThrowsInvalidOperationException()
    {
        Mapper sut = Mapper.Create();
        ConstructorConversionFailureSource source = new()
        {
            Quantity = "not-an-int"
        };

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            sut.Map<ConstructorConversionFailureSource, ConstructorConversionFailureTarget>(source));

        exception.Message.ShouldContain(nameof(ConstructorConversionFailureTarget));
    }
}
