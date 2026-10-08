namespace SchoolAccount.Web.Mvc.Features.Shared.MultipleSchoolsStatusTable;

public class MultipleSchoolsStatusTableViewModel
{
    public IReadOnlyList<SchoolStatus> SchoolStatuses { get; init; } = [];

    public string Title => "Schools in your trust";

    public class SchoolStatus
    {
        public string Name { get; init; }

        public string Status { get; init; }

        public int? Errors { get; set; }

        public int? Queries { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string UpdatedAtMessage =>
            UpdatedAt.HasValue
                ? $"{Math.Floor((DateTime.UtcNow - UpdatedAt.Value).TotalDays)} days ago"
                : string.Empty;
    }
}
