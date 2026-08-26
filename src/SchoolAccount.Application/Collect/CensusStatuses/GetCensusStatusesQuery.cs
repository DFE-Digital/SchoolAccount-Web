using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.Application.Collect.CensusStatuses;

public record GetCensusStatusesQuery : IQuery<List<GetCensusStatusesResponse>>
{
    public string Id { get; init; }

    public string EmailAddress { get; init; }

    public List<Organisation> Organisations { get; init; } = [];
}
