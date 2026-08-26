using SchoolAccount.Application.Collect.CensusStatuses;
using SchoolAccount.SharedKernel;
using Action = SchoolAccount.Application.Collect.CensusStatuses.Action;

namespace SchoolAccount.IntegrationTests.Common.Builders;

public class CensusStatusesResponseBuilder
{
    private readonly List<Action> _actions = [];
    private string _id = "Test-id";
    private bool _interesting = true;

    public static CensusStatusesResponseBuilder Create() => new();

    public CensusStatusesResponseBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public CensusStatusesResponseBuilder NotInteresting()
    {
        _interesting = false;
        return this;
    }

    public CensusStatusesResponseBuilder WithAction(string name, string status)
    {
        _actions.Add(
            new Action
            {
                Name = name,
                Status = new Status { Name = status },
            }
        );
        return this;
    }

    private GetCensusStatusesResponse Build()
    {
        return new GetCensusStatusesResponse
        {
            Id = _id,
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
