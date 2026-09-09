using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.Web.Mvc.Features.Shared.MatSchoolsStatusTable;

public class MatSchoolsStatusTableViewModel
{
    public bool IsLocalAuthority { get; init; }

    public string MatOrLocalAuthorityString => IsLocalAuthority ? "local authority" : "trust";

    public string Title => $"Schools in your {MatOrLocalAuthorityString}";

    public IReadOnlyList<Organisation> Organisations { get; init; } = Array.Empty<Organisation>();
}
