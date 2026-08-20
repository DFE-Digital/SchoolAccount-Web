using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Features.Header;

public interface IHeaderContentProvider
{
    bool IsAuthenticated { get; }
    string? OrganisationName { get; }
}

public class HeaderContentProvider : IHeaderContentProvider
{
    public bool IsAuthenticated { get; }
    public string? OrganisationName { get; }

    public HeaderContentProvider(IUserContext userContext)
    {
        IsAuthenticated = userContext.IsAuthenticated;
        OrganisationName = userContext.Organisation?.Name;
    }
}
