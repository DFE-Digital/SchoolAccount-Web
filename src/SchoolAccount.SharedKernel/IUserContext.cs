namespace SchoolAccount.SharedKernel;

public interface IUserContext
{
    bool IsAuthenticated { get; }
    string? Id { get; }
    string? AuthenticationType { get; }
    string? EmailAddress { get; }
    string? Name { get; }
    string? OrganisationName { get; }
}
