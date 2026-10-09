using SchoolAccount.Application.Features.Collect.CensusStatuses;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.TestCommon.Builders;

public class CensusStatusesResponseBuilder
{
    private readonly List<CensusAction> _actions = [];
    private readonly string _id = "Test-id";
    private string _name = "Test School";
    private readonly bool _interesting = true;

    public static CensusStatusesResponseBuilder ACensusStatusResponse() => new();

    public CensusStatusesResponseBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CensusStatusesResponseBuilder WithAction(
        string name,
        string status,
        int? errors = null,
        int? queries = null,
        DateTime? updatedAt = null
    )
    {
        _actions.Add(
            new CensusAction
            {
                Name = name,
                Status = new CensusStatus { Name = status },
                Errors = errors,
                Queries = queries,
                UpdatedAt = updatedAt,
            }
        );
        return this;
    }

    public GetCensusStatusesResponse Build()
    {
        return new GetCensusStatusesResponse
        {
            Id = _id,
            SchoolName = _name,
            Interesting = _interesting,
            Actions = _actions,
        };
    }

    public Result<List<GetCensusStatusesResponse>> AsSuccess()
    {
        return Result.Success<List<GetCensusStatusesResponse>>([Build()]);
    }

    public static readonly Error FetchFailed = Error.Failure(
        "CensusStatuses.Test",
        "Census statuses could not be fetched"
    );

    public static Result<List<GetCensusStatusesResponse>> AsFailure(Error? error = null)
    {
        return Result.Failure<List<GetCensusStatusesResponse>>(error ?? FetchFailed);
    }
}
