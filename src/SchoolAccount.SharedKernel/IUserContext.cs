using SchoolAccount.Web.Mvc.Authentication;

namespace SchoolAccount.SharedKernel;

public interface IUserContext
{
    bool IsAuthenticated { get; }
    string? Id { get; }
    string? AuthenticationType { get; }
    string? EmailAddress { get; }
    string? Name { get; }
    Organisation? Organisation { get; }
}
