namespace SchoolAccount.Application.Features.Academies.GetAcademies;

public class GetAcademyTrustResponse
{
    public required string Name { get; init; }

    public required string Ukprn { get; init; }

    public GetAcademyNameAndCodeResponse? Type { get; init; }

    public string? GroupUid { get; init; }

    public IReadOnlyList<GetAcademyEstablishmentResponse>? Establishments { get; init; } = [];
}
