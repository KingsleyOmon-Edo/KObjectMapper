namespace ExtensionsTests.Helpers
{
    internal class ObjectMother
    {
        public static Product ArbitraryProduct => new Product
        {
            Id = 0,
            Description = "A nice bag for your stuff",
            Price = 100.00M,
            Quantity = 10
        };

        public static Customer ArbitraryCustomer => new Customer
        {
            Id = 0,
            FirstName = "James",
            LastName = "Ono",
            PhoneNumber = "5555550009"
        };

    }
}
