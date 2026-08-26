using SchoolAccount.Application.Abstractions.Messaging;

namespace SchoolAccount.Application.Collect.CensusStatuses;

public record GetCensusStatusesQuery(GetCensusStatusesRequest Request)
    : IQuery<List<GetCensusStatusesResponse>>;
