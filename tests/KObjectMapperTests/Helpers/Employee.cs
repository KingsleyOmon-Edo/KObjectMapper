namespace KObjectMapperTests.Helpers
{
    public class Employee
    {
        public long EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public decimal Salary { get; set; }
        public DateTimeOffset HireDate { get; set; }
    }
}