using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.Application.Collect.CensusStatuses;

public class GetCensusStatusesRequest
{
    public string Id { get; init; }

    public string Email { get; init; }

    public List<Organisation> Organisations { get; init; }
}
