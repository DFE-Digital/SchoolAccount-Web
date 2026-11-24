using static SchoolAccount.Web.Mvc.Helpers.StatusFormatter;

namespace SchoolAccount.Web.Mvc.Features.Shared.MultipleSchoolsStatusTable;

public class MultipleSchoolsStatusTableViewModel
{
    public string OrganisationName { get; init; }

    public IReadOnlyList<SchoolStatus> SchoolStatuses { get; init; } = [];

    public string Title => "Schools in your trust";

    public bool AllStatusesUnavailable =>
        SchoolStatuses.Any() && SchoolStatuses.All(x => x.Status is StatusUnavailableLabel);

    public class SchoolStatus
    {
        public string Name { get; init; }

        public string Status { get; init; }

        public int? Errors { get; init; }

        public int? Queries { get; init; }

        public string UpdatedAt { get; init; }
    }
}
