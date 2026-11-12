namespace SchoolAccount.E2ETests.Common.Database;

public sealed record CollectReturnStatus
{
    public required string SchoolName { get; init; }

    public required string LAEStab { get; init; }

    public required int ReturnStatusCode { get; init; }

    public int? Errors { get; init; }

    public int? Queries { get; init; }

    public int? OkdErrorsQueries { get; init; }

    public required string Hash { get; init; }

    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    public required int DCID { get; init; }

    public required string Collection { get; init; }

    public required int DataReturnID { get; init; }
}
