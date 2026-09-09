using SchoolAccount.Application.Abstractions.Messaging;

namespace SchoolAccount.Application.Features.Academies.GetAcademies;

public record GetAcademiesQuery : IQuery<GetAcademyTrustResponse>
{
    public string Ukprn { get; init; }
}
