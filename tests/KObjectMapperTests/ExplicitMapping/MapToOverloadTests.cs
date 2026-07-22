using KObjectMapper;
using KObjectMapperTests.Helpers;

namespace KObjectMapperTests.ExplicitMapping;

public class MapToOverloadTests
{
    private readonly CommonAsserts _commonAsserts = CommonAsserts.Create();

    [Fact]
    public void MapTo_ObjectOverload_WithCompatibleTypes_MapsValuesToTarget()
    {
        Mapper sut = Mapper.Create();
        Customer source = ObjectMother.SampleCustomer;
        CustomerDto target = new();

        sut.MapTo(source, target);

        _commonAsserts.AssertCustomerDtoIsCorrectlyMappedFromCustomer(target, source);
    }

    [Fact]
    public void MapTo_ObjectOverload_WhenSourceIsNull_ThrowsArgumentNullException()
    {
        Mapper sut = Mapper.Create();
        Customer? source = null;
        CustomerDto target = new();

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => sut.MapTo(source!, target));

        Assert.Equal("source", exception.ParamName);
    }

    [Fact]
    public void MapTo_ObjectOverload_WhenTargetIsNull_ThrowsArgumentNullException()
    {
        Mapper sut = Mapper.Create();
        Customer source = ObjectMother.SampleCustomer;
        CustomerDto? target = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => sut.MapTo(source, target!));

        Assert.Equal("target", exception.ParamName);
    }

    [Fact]
    public void MapTo_GenericOverload_WithCompatibleTypes_MapsValuesToTarget()
    {
        Mapper sut = Mapper.Create();
        Customer source = ObjectMother.SampleCustomer;
        CustomerDto target = new();

        sut.MapTo<Customer, CustomerDto>(source, target);

        _commonAsserts.AssertCustomerDtoIsCorrectlyMappedFromCustomer(target, source);
    }

    [Fact]
    public void MapTo_GenericOverload_WhenSourceIsNull_ThrowsArgumentNullException()
    {
        Mapper sut = Mapper.Create();
        Customer? source = null;
        CustomerDto target = new();

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => sut.MapTo<Customer?, CustomerDto>(source, target));

        Assert.Equal("source", exception.ParamName);
    }

    [Fact]
    public void MapTo_GenericOverload_WhenTargetIsNull_ThrowsArgumentNullException()
    {
        Mapper sut = Mapper.Create();
        Customer source = ObjectMother.SampleCustomer;
        CustomerDto? target = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => sut.MapTo<Customer, CustomerDto?>(source, target));

        Assert.Equal("target", exception.ParamName);
    }

    [Fact]
    public void MapTo_GenericOverload_WhenSourceTypeDoesNotMatchGenericArgument_ThrowsArgumentException()
    {
        Mapper sut = Mapper.Create();
        object source = ObjectMother.SampleCustomer;
        CustomerDto target = new();

        ArgumentException exception = Assert.Throws<ArgumentException>(() => sut.MapTo<object, CustomerDto>(source, target));

        Assert.Contains("safeSource", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void MapTo_GenericOverload_WhenTargetTypeDoesNotMatchGenericArgument_ThrowsArgumentException()
    {
        Mapper sut = Mapper.Create();
        Customer source = ObjectMother.SampleCustomer;
        object target = new CustomerDto();

        ArgumentException exception = Assert.Throws<ArgumentException>(() => sut.MapTo<Customer, object>(source, target));

        Assert.Contains("safeTarget", exception.Message, StringComparison.Ordinal);
    }
}
