namespace SchoolAccount.SharedKernel;

public interface IUserContext
{
    string? AuthenticationType { get; }
    bool IsAuthenticated { get; }
    string? Id { get; }
    string? EmailAddress { get; }
    string? Name { get; }
}
