using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.Application.Collect.CensusStatus;

public class GetCensusStatusRequestModel
{
    public string Id { get; init; }

    public string Email { get; init; }

    public List<Organisation> Organisations { get; init; }
}
