using KObjectMapper;
using KObjectMapper.UnitTests.Helpers;

namespace KObjectMapper.UnitTests.ExplicitMapping;

public class MapFromOverloadTests
{
    private readonly CommonAsserts _commonAsserts = CommonAsserts.Create();

    [Fact]
    public void MapFrom_ObjectOverload_WithCompatibleTypes_MapsValuesIntoFirstArgument()
    {
        Mapper sut = Mapper.Create();
        CustomerDto firstArgument = ObjectMother.SampleCustomerDto;
        Customer secondArgument = ObjectMother.SampleCustomer;

        sut.MapFrom(firstArgument, secondArgument);

        _commonAsserts.AssertCustomerDtoIsCorrectlyMappedFromCustomer(firstArgument, secondArgument);
    }

    [Fact]
    public void MapFrom_ObjectOverload_WhenSourceIsNull_ThrowsArgumentNullException()
    {
        Mapper sut = Mapper.Create();
        CustomerDto? source = null;
        Customer target = ObjectMother.SampleCustomer;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => sut.MapFrom(source!, target));

        Assert.Equal("source", exception.ParamName);
    }

    [Fact]
    public void MapFrom_ObjectOverload_WhenTargetIsNull_ThrowsArgumentNullException()
    {
        Mapper sut = Mapper.Create();
        CustomerDto source = ObjectMother.SampleCustomerDto;
        Customer? target = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => sut.MapFrom(source, target!));

        Assert.Equal("target", exception.ParamName);
    }

    [Fact]
    public void MapFrom_GenericOverload_WithCompatibleTypes_MapsValuesIntoSecondArgument()
    {
        Mapper sut = Mapper.Create();
        CustomerDto source = ObjectMother.SampleCustomerDto;
        Customer target = ObjectMother.SampleCustomer;

        sut.MapFrom<CustomerDto, Customer>(source, target);

        _commonAsserts.AssertCustomerIsCorrectlyMappedFromCustomerDto(target, source);
    }

    [Fact]
    public void MapFrom_GenericOverload_WhenSourceIsNull_ThrowsArgumentNullException()
    {
        Mapper sut = Mapper.Create();
        CustomerDto? source = null;
        Customer target = ObjectMother.SampleCustomer;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => sut.MapFrom<CustomerDto?, Customer>(source, target));

        Assert.Equal("source", exception.ParamName);
    }

    [Fact]
    public void MapFrom_GenericOverload_WhenTargetIsNull_ThrowsArgumentNullException()
    {
        Mapper sut = Mapper.Create();
        CustomerDto source = ObjectMother.SampleCustomerDto;
        Customer? target = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => sut.MapFrom<CustomerDto, Customer?>(source, target));

        Assert.Equal("target", exception.ParamName);
    }

    [Fact]
    public void MapFrom_GenericOverload_WhenSourceTypeDoesNotMatchGenericArgument_ThrowsArgumentException()
    {
        Mapper sut = Mapper.Create();
        object source = ObjectMother.SampleCustomerDto;
        Customer target = ObjectMother.SampleCustomer;

        ArgumentException exception = Assert.Throws<ArgumentException>(() => sut.MapFrom<object, Customer>(source, target));

        Assert.Contains("safeSource", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void MapFrom_GenericOverload_WhenTargetTypeDoesNotMatchGenericArgument_ThrowsArgumentException()
    {
        Mapper sut = Mapper.Create();
        CustomerDto source = ObjectMother.SampleCustomerDto;
        object target = ObjectMother.SampleCustomer;

        ArgumentException exception = Assert.Throws<ArgumentException>(() => sut.MapFrom<CustomerDto, object>(source, target));

        Assert.Contains("safeTarget", exception.Message, StringComparison.Ordinal);
    }
}
