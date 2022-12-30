namespace ExtensionsTests.Helpers
{
    internal class ObjectMother
    {
        public static Product SampleProduct => new Product
        {
            Id = 0,
            Description = "A nice bag for your stuff",
            Price = 100.00M,
            Quantity = 10
        };

        public static Customer SampleCustomer => new Customer
        {
            Id = 0,
            FirstName = "James",
            LastName = "Ono",
            PhoneNumber = "5555550009"
        };

    }
}
