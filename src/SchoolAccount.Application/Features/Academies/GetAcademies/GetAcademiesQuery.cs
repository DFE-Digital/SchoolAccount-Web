using SchoolAccount.Application.Abstractions.Messaging;

namespace SchoolAccount.Application.Features.Academies.GetAcademies;

public record GetAcademiesQuery(string ukprn) : IQuery<GetAcademyTrustResponse>;
