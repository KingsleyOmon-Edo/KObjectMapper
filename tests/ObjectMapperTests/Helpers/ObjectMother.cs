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

        public static List<CustomerDto> SampleCustomerDtoData =>
            new()
            {
                new CustomerDto{Id = 0, FirstName = "Zim", PhoneNumber = "5555555980"},
                new CustomerDto{Id = 1, FirstName = "Brandy", PhoneNumber = "7777777980"},
                new CustomerDto{Id = 2, FirstName = "James", PhoneNumber = "5609123456"},
                new CustomerDto{Id = 3, FirstName = "Sean", PhoneNumber = "9837957583"},
            };

        public static List<Customer> SampleCustomerData =>
            new()
            {
                new Customer() { Id = 0, FirstName = "Benjamin", LastName = "Fezzi", PhoneNumber = "4443256775" },
                new Customer() { Id = 1, FirstName = "Sascha", LastName = "Oleg", PhoneNumber = "7896453219" },
                new Customer() { Id = 2, FirstName = "Remington", LastName = "Quanta", PhoneNumber = "81234567890" },
                new Customer() { Id = 3, FirstName = "Ferodo", LastName = "Tellizi", PhoneNumber = "5679274623" },
                new Customer() { Id = 4, FirstName = "Enrico", LastName = "Fantaz", PhoneNumber = "98732537290" },
            };

        public static List<Employee> SampleEmployeeData =>
            new()
            {
                new Employee()
                {
                    EmployeeId = 1_000, FirstName = "Yan", LastName = "Dahl", Salary = 1_000_000, HireDate = DateTimeOffset.UtcNow.AddYears(-2)
                },new Employee()
                {
                    EmployeeId = 1_001, FirstName = "Felix", LastName = "Ramus", Salary = 1_500_000, HireDate = DateTimeOffset.UtcNow.AddYears(-5)
                },new Employee()
                {
                    EmployeeId = 1_002, FirstName = "Bridget", LastName = "ALcove", Salary = 1_500_000, HireDate = DateTimeOffset.UtcNow.AddYears(-3)
                },new Employee()
                {
                    EmployeeId = 1_003, FirstName = "Franco", LastName = "Jimson", Salary = 200_000, HireDate = DateTimeOffset.UtcNow.AddMonths(9)
                },new Employee()
                {
                    EmployeeId = 1_004, FirstName = "Shawna", LastName = "Orwell", Salary = 200_000, HireDate = DateTimeOffset.UtcNow.AddYears(-2)
                }
            };
    }
}