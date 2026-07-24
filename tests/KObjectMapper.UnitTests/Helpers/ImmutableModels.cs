namespace KObjectMapper.UnitTests.Helpers;

public sealed record ImmutableCustomerSource(long Id, string FirstName, string PhoneNumber);

public sealed record ImmutableCustomerDestination(long Id, string FirstName, string PhoneNumber);
