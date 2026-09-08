using System.Text.Json.Serialization;

namespace SchoolAccount.Application.Features.Academies;

public class GetAcademyTrustResponse
{
    public string? Name { get; set; }

    public string? Ukprn { get; set; }

    public GetAcademyNameAndCodeResponse? Type { get; set; }

    public string? GroupUid { get; set; }

    public IReadOnlyList<GetAcademyEstablishmentResponse> Establishments { get; init; } = [];
}
