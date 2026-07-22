namespace KObjectMapperTests.Helpers;

internal sealed class ReadOnlySource
{
    public string Name { get; set; } = string.Empty;

    public int Age { get; set; }
}

internal sealed class ReadOnlyTarget
{
    public string Name { get; } = "Before";

    public int Age { get; set; }
}

internal sealed class NestedCustomerSource
{
    public string Name { get; set; } = string.Empty;

    public Address Address { get; set; } = new();
}

internal sealed class NestedCustomerTarget
{
    public string Name { get; set; } = string.Empty;

    public Address Address { get; set; } = new();
}

internal sealed class Address
{
    public string Street { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;
}

internal sealed class ConvertibleSource
{
    public int Quantity { get; set; }
}

internal sealed class ConvertibleTarget
{
    public long Quantity { get; set; }
}

internal sealed class NoDefaultConstructorSource
{
    public string Name { get; set; } = string.Empty;
}

internal sealed class NoDefaultConstructorTarget
{
    public string Name { get; set; }

    public NoDefaultConstructorTarget(string name)
    {
        Name = name;
    }
}

internal sealed class ConstructorNameMismatchSource
{
    public string FirstName { get; set; } = string.Empty;
}

internal sealed class ConstructorNameMismatchTarget
{
    public string Name { get; }

    public ConstructorNameMismatchTarget(string name)
    {
        Name = name;
    }
}

internal sealed class ConstructorConversionFailureSource
{
    public string Quantity { get; set; } = string.Empty;
}

internal sealed class ConstructorConversionFailureTarget
{
    public int Quantity { get; }

    public ConstructorConversionFailureTarget(int quantity)
    {
        Quantity = quantity;
    }
}
