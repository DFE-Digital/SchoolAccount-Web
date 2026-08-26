using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.Infrastructure.Collect.CensusStatuses;

public class GetCensusStatusesApiRequest
{
    public string Id { get; init; }

    public string Email { get; init; }

    public List<Organisation> Organisations { get; init; }
}
