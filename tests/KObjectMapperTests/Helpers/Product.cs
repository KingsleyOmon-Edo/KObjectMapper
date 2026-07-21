// Copyright (c) KObjectMapper contributors. All rights reserved.

namespace KObjectMapperTests.Helpers;

internal sealed class Product
{
    public long Id { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
