using SchoolAccount.SharedKernel.Authentication;
using static SchoolAccount.SharedKernel.Authentication.OrganisationCategory;

namespace SchoolAccount.Web.Mvc.Features.Shared.MultipleSchoolsStatusTable;

public class MultipleSchoolsStatusTableViewModel
{
    public OrganisationCategory Category { get; init; }

    public IReadOnlyList<SchoolStatus> SchoolStatuses { get; init; } = [];

    public string Title => $"Schools in your {OrganisationCategoryName}";

    public bool IsAcademyTrust => Category is MultiAcademyTrust or SingleAcademyTrust;

    public string OrganisationCategoryName => IsAcademyTrust ? "trust" : "local authority";
}

public class SchoolStatus
{
    public string Name { get; init; }

    public string Status { get; init; }
}
