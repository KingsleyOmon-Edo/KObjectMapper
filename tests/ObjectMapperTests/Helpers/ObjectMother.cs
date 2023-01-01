namespace ObjectMapperTests.Helpers
{
    internal class ObjectMother
    {
        public static Product SampleProduct => new()
        {
            Id = 0,
            Description = "A nice bag for your stuff",
            Price = 100.00M,
            Quantity = 10
        };

        public static Customer SampleCustomer => new()
        {
            Id = 0,
            FirstName = "James",
            LastName = "Ono",
            PhoneNumber = "5555550009"
        };

        public static CustomerDto SampleCustomerDto => new()
        {
            Id = 10,
            FirstName = "Rudolph",
            PhoneNumber = "5555550009"
        };

        public static Employee SampleEmployee => new()
        {
            EmployeeId = 10,
            FirstName = "Sam",
            LastName = "Williams",
            Salary = 100_000.00M,
            HireDate = new DateTimeOffset(2021, 3, 15, 6, 30, 25, TimeSpan.Zero)
        };
    }
}