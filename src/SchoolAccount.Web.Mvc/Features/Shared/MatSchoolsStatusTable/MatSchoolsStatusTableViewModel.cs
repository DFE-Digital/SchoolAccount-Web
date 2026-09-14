namespace SchoolAccount.Web.Mvc.Features.Shared.MatSchoolsStatusTable;

public class MatSchoolsStatusTableViewModel
{
    public string Title => "Schools in your trust";

    public IReadOnlyList<SchoolStatus> SchoolStatuses { get; init; } = [];
}

public class SchoolStatus
{
    public string Name { get; init; }

    public string Status { get; init; }
}
