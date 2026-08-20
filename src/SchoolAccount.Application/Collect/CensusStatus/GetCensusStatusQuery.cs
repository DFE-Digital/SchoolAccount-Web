using SchoolAccount.Application.Abstractions.Messaging;

namespace SchoolAccount.Application.Collect.CensusStatus;

public record GetCensusStatusQuery(GetCensusStatusRequestModel request)
    : IQuery<GetCensusStatusResponse>;
