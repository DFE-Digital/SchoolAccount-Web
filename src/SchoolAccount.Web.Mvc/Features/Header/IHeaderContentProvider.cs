using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Features.Header;

public interface IHeaderContentProvider
{
    string? OrganisationName { get; }
}

public class HeaderContentProvider : IHeaderContentProvider
{
    public string? OrganisationName { get; }

    public HeaderContentProvider(IUserContext userContext)
    {
        OrganisationName = userContext.Organisation?.Name;
    }
}
